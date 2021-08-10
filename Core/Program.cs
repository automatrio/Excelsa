using Excelsa.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Excelsa.Core
{
    public class Program
    {
        private static int CurrentOption { get; set; } = 0;

        private static readonly int _headerLength = 6;

        private static List<SelectOption> AvailableComponents { get; set; } = new List<SelectOption>();
        private static List<SelectOption> AvailablePages { get; set; } = new List<SelectOption>();

        private static Dictionary<string, string> Paths { get; set; } = new Dictionary<string, string>();

        private static readonly List<SelectOption> _menuOptions = new List<SelectOption>()
        {
            new SelectOption(" 1) Create component"),
            new SelectOption(" 2) Create page"),
            new SelectOption(" 3) Create test"),
            new SelectOption(" 4) Configure"),
            new SelectOption(" 5) Import Excel"),
            new SelectOption(" 6) Exit")
        };
        private static readonly List<SelectOption> _yesOrNoOptions = new List<SelectOption>()
        {
            new SelectOption("  Yes\n"),
            new SelectOption("  No\n")
        };
        private static readonly Option[] _componentOptions =
        {
            new Option(" (1/1) Name your component (ex.: MonthlyIncomeChart): ", AnswerType.Name)
        };
        private static readonly Option[] _pageOptions =
        {
            new Option(" (1/3) Name your page (ex.: TotalIncome): ", AnswerType.Name),
            new Option(" (2/3) Give your page a numeric ID (ex.: 12): ", AnswerType.Name),
            new Option(" (3/3) Choose the components for this page (SPACEBAR to select): ", AnswerType.Menu, AvailableComponents)
        };
        private static readonly Option[] _testOptions =
{
            new Option(" (1/2) Name your test (ex.: FinancialSector): ", AnswerType.Name),
            new Option(" (1/2) Choose the pages for this test (SPACEBAR to select): ", AnswerType.Menu, AvailablePages)
        };
        private static readonly Option[] _configureOptions =
{
            new Option(" (1/9) Write the URL for the website you will be testing: ", AnswerType.Path),
            new Option(" (2/9) What fallback URLs would you like to input? (separate the with a pipe, \'|\')", AnswerType.List),
            new Option(" (3/9) Is login embedded in the URL? (login via browser popup)", AnswerType.YesOrNo),
            new Option(" (4/9) Name the login to access the given URL: ", AnswerType.Name),
            new Option(" (5/9) Name the password to access the given URL: ", AnswerType.Name),
            new Option(" (6/9) Do you want the test to take screenshots?", AnswerType.YesOrNo),
            new Option(" (7/9) Where do you want to save your screenshots?", AnswerType.Path),
            new Option(" (8/9) Where do you want to save your logs?", AnswerType.Path),
            new Option(" (9/9) How much would you like to pad the waiting time? (good for slow connections, ex.: 1.5)", AnswerType.Number)
        };
        private static readonly Option[] _importExcelOptions =
        {
            new Option("Please provide the path to the .xlsx file: ", AnswerType.Path)
        };

        public static async Task Main()
        {
            GetProjectPaths();

            var currentOption = Console.CursorTop;

            while (CurrentOption != 6)
            {
                WriteHeaderToConsole();

                WriteMenuOptions(currentOption);
                var (choice, _) = MoveAcrossMenuOptions(_menuOptions, true);

                WriteHeaderToConsole();

                await WriteOptionMenu(choice);

                Console.ReadKey();
            }
        }


        // Application functions
        #region
        private static void WriteMenuOptions(int currentOption)
        {
            for (int i = 0; i < _menuOptions.Count; i++)
            {
                CenterAlignText(_menuOptions[i].Text);
            }
        }

        private static void WriteHeaderToConsole()
        {
            Console.Clear();

            WriteFullLine();

            Console.ForegroundColor = ConsoleColor.Red;
            CenterAlignText("::: EXCELSA ::::");

            WriteFullLine();

            Console.ResetColor();

            Console.WriteLine();

            switch (CurrentOption)
            {
                case 0:
                    CenterAlignText("MENU");
                    break;
                case 1:
                    CenterAlignText("COMPONENT CREATION");
                    break;
                case 2:
                    CenterAlignText("PAGE CREATION");
                    break;
                case 3:
                    CenterAlignText("TEST CREATION");
                    break;
                case 4:
                    CenterAlignText("SETTINGS");
                    break;
                case 5:
                    CenterAlignText("IMPORT EXCEL FILE");
                    break;
                case 6:
                    CenterAlignText("Thanks for using Excelsa!");
                    break;
                default:
                    break;
            }

            Console.WriteLine();
        }

        #endregion

        // Console styling functions
        #region

        private static void WriteFullLine(string value = "")
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(value.PadRight(Console.WindowWidth - value.Length));
            Console.ResetColor();
        }

        private static void CenterAlignText(string text)
        {
            Console.Write(text.PadLeft(Console.WindowWidth / 2 + text.Length / 2).PadRight(Console.WindowWidth));
        }

        private static void LeftAlignText(string text)
        {
            Console.Write("   " + text);
        }

        private static void HighlightOption(string text, int index, bool centerAlignText)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (centerAlignText)
                CenterAlignText(text);
            else
                LeftAlignText(text);
            Console.ResetColor();
        }

        private static void HighlightSelection(string option)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            LeftAlignText(option);
            Console.ResetColor();
        }

        private static void WriteInfo(string message, InfoType type)
        {
            switch (type)
            {
                case InfoType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    CenterAlignText(message);
                    Console.ResetColor();
                    break;
                case InfoType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    CenterAlignText(message);
                    Console.ResetColor();
                    break;
                case InfoType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    CenterAlignText(message);
                    Console.ResetColor();
                    break;
                default:
                    break;
            }
        }

        private static string ReadInput()
        {
            Console.Write("   -> ");
            return Console.ReadLine();
        }
        #endregion

        // Menu control functions
        #region
        private static (int, bool) MoveAcrossMenuOptions(
            List<SelectOption> options,
            bool centerAlignText)
        {
            int choice = -1;
            bool isSelection = false;
            int cursorPosition = Console.CursorTop;
            int minRange = cursorPosition - options.Count;
            int maxRange = cursorPosition;

            while (choice == -1)
            {
                try
                {
                    (choice, isSelection) = CycleOptions(
                        options,
                        minRange,
                        maxRange,
                        centerAlignText
                        );
                }
                catch
                {
                    Console.SetCursorPosition(0, cursorPosition);
                }
            }

            CurrentOption = choice;
            return (choice, isSelection);
        }

        private static (int, bool) CycleOptions(
            List<SelectOption> options,
            int minRange,
            int maxRange,
            bool centerAlignText)
        {
            var cursor = Console.CursorTop;
            var input = Console.ReadKey();

            if (input.Key == ConsoleKey.UpArrow)
            {
                if (cursor > minRange)
                {
                    if (cursor < maxRange)
                    {
                        Console.SetCursorPosition(0, cursor);
                        string text = options[cursor - minRange].Text;
                        if (centerAlignText)
                            CenterAlignText(text);
                        else if(options[cursor - minRange].Selected == true)
                        {
                            HighlightSelection(text);
                        }
                        else
                        {
                            LeftAlignText(text);
                        }     
                    }

                    cursor--;
                }

                Console.SetCursorPosition(0, cursor);

                HighlightOption(options[cursor - minRange].Text, cursor - minRange, centerAlignText);

                Console.SetCursorPosition(0, cursor);
            }
            else if (input.Key == ConsoleKey.DownArrow)
            {
                if (cursor < maxRange)
                {
                    if (cursor >= minRange)
                    {
                        Console.SetCursorPosition(0, cursor);
                        string text = options[cursor - minRange].Text;
                        if (centerAlignText)
                            CenterAlignText(text);
                        else if (options[cursor - minRange].Selected == true)
                        {
                            HighlightSelection(text);
                        }
                        else
                        {
                            LeftAlignText(text);
                        }
                    }

                    cursor++;
                }

                Console.SetCursorPosition(0, cursor);

                HighlightOption(options[cursor - minRange].Text, cursor - minRange, centerAlignText);

                Console.SetCursorPosition(0, cursor);
            }
            else if (input.Key == ConsoleKey.Enter)
            {
                if (cursor >= minRange && cursor < maxRange)
                    return (Console.CursorTop - minRange + 1, false);
                else
                    return (-1, false);
            }
            else if (input.Key == ConsoleKey.Spacebar)
            {
                if (cursor >= minRange && cursor < maxRange)
                {
                    var choice = Console.CursorTop - minRange + 1;
                    return (choice, true);
                }
                else
                    return (-1, false);
            }
            else
            {
                Console.SetCursorPosition(0, cursor);
            }
            return (-1, false);
        }


        private static async Task WriteOptionMenu(int choice)
        {
            GetAvailablePagesAndComponents();
            var answers = new List<object>();

            switch (choice)
            {
                case 1: // component 
                    ShowSubMenuOptions(_componentOptions, ref answers);
                    await GenerateComponent(answers);
                    break;
                case 2: // page
                    ShowSubMenuOptions(_pageOptions, ref answers);
                    await GeneratePage(answers);
                    break;
                case 3: // test
                    ShowSubMenuOptions(_testOptions, ref answers);
                    await GenerateTest(answers);
                    break;
                case 4: // config
                    ShowSubMenuOptions(_configureOptions, ref answers);
                    await WriteAppSettings(answers);
                    break;
                case 5: // excel
                    ShowSubMenuOptions(_importExcelOptions, ref answers);
                    ImportExcelFile(answers);
                    break;
                default: // leave
                    break;
            }
        }

        private static void ShowSubMenuOptions(Option[] options, ref List<object> answers)
        {
            foreach (var opt in options)
            {
                Console.WriteLine(opt.Text.PadRight(Console.WindowWidth));
                answers.Add(
                    ReadAnswer(opt)
                    );
                WriteHeaderToConsole();
                Console.SetCursorPosition(0, _headerLength);
            }
        }

        #endregion

        // Input parsing functions
        #region
        private static object ReadAnswer(Option option)
        {
            switch (option.Type)
            {
                case AnswerType.YesOrNo:
                    return ParseYesOrNo();
                case AnswerType.Name:
                    return ReadInput();
                case AnswerType.Path:
                    return ReadInput();
                case AnswerType.Number:
                    return ParseNumericAnswer();
                case AnswerType.List:
                    return ParseList();
                case AnswerType.Menu:
                    return ParseInnerMenu(option.SelectOptions);
                default:
                    return null;
            }
        }

        private static bool ParseYesOrNo()
        {
            int choice = 0;

            foreach (var opt in _yesOrNoOptions) LeftAlignText(opt.Text);

            while (choice == 0)
            {
                (choice, _ ) = MoveAcrossMenuOptions(_yesOrNoOptions, false);
            }

            return (choice == 1);
        }

        private static object ParseNumericAnswer()
        {
            string answer = ReadInput();

            if (Int32.TryParse(answer, out int intResult) == false)
            {
                if (Decimal.TryParse(answer, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalResult) == false)
                {
                    Console.WriteLine("This is not a valid number.");
                    return null;
                }
                return decimalResult;
            }
            return intResult;
        }

        private static List<string> ParseList()
        {
            string input = ReadInput();
            return input.Split('|').ToList();
        }

        private static List<string> ParseInnerMenu(List<SelectOption> options)
        {
            var answers = new List<string>();
            bool isSelection;
            int choice = 0;
            int initialCursorPosition = Console.CursorTop;

            while (choice != options.Count - 1)
            {
                foreach (var opt in options)
                {
                    if (opt.Selected)
                    {
                        HighlightSelection(opt.Text);
                    }
                    else
                    {
                        LeftAlignText(opt.Text);
                    }
                }

                (choice, isSelection) = MoveAcrossMenuOptions(
                    options, 
                    false
                    );

                if (isSelection && choice != options.Count)
                {
                    options[choice - 1].Selected = true;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    HighlightSelection(options[choice - 1].Text);
                    string textValue = options[choice - 1].Text.Replace("\n", string.Empty);
                    answers.Add(textValue);
                    choice = -1;
                }
                else if (choice == options.Count)
                {
                    return answers;
                }

                Console.SetCursorPosition(0, initialCursorPosition);
            }
            return answers;
        }

        #endregion

        // Code generation and app configuration
        #region
        private static async Task WriteAppSettings(List<object> answers)
        {
            var type = typeof(GlobalVariables);
            var properties = type.GetProperties();
            var mappedAnswers = new Dictionary<string, object>();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name != "MainLog")
                    mappedAnswers.Add(properties[i].Name, answers[i]);
            }

            var appsettingsPath = Path.Combine(Paths["ProjectPath"], "Core\\appsettings.json");

            using (var writer = new StreamWriter(appsettingsPath))
            {
                var json = JsonConvert.SerializeObject(mappedAnswers);
                await writer.WriteAsync(json);
            }
        }

        private static async Task GenerateComponent(List<object> answers)
        {
            var service = new CodeGeneratorService();

            bool success = await service.GenerateComponent(
                templatePath: Paths["ComponentTemplate"],
                destinationPath: Paths["Components"],
                values: answers);

            if (success)
                WriteInfo("Component created successfully!", InfoType.Success);
            else
                WriteInfo("Something went wrong...", InfoType.Error);
        }

        private static async Task GeneratePage(List<object> answers)
        {
            var service = new CodeGeneratorService();

            bool success = await service.GeneratePage(
                templatePath: Paths["PageTemplate"],
                internalTemplatePath: Paths["AddComponentTemplate"],
                destinationPath: Paths["Pages"],
                values: answers);

            if (success)
                WriteInfo("Page created successfully!", InfoType.Success);
            else
                WriteInfo("Something went wrong...", InfoType.Error);
        }

        private static async Task GenerateTest(List<object> answers)
        {
            var service = new CodeGeneratorService();

            bool success = await service.GenerateTest(
                templatePath: Paths["TestTemplate"],
                internalTemplatePath: Paths["AddPageTemplate"],
                destinationPath: Paths["Tests"],
                values: answers);

            if (success)
                WriteInfo("Test created successfully!", InfoType.Success);
            else
                WriteInfo("Something went wrong...", InfoType.Error);
        }

        private static void ImportExcelFile(List<object> answers)
        {

        }

        #endregion

        // Initialization
        #region
        private static void GetProjectPaths()
        {
            // Project Path
            string path = Assembly.GetAssembly(typeof(Program)).Location;
            var pathSplit = path.Split('\\').ToList();
            pathSplit = pathSplit.GetRange(0, pathSplit.Count() - 3);
            var projectPath = "";
            foreach (var folder in pathSplit) projectPath += folder + '\\';
            Paths.Add("ProjectPath", projectPath);

            // Components Path
            var componentsPath = Path.Combine(projectPath, "Domain\\Components");
            Paths.Add("Components", componentsPath);

            // Pages Path
            var pagesPath = Path.Combine(projectPath, "Domain\\Pages");
            Paths.Add("Pages", pagesPath);

            // Tests path
            var testsPath = Path.Combine(projectPath, "Main\\Tests");
            Paths.Add("Tests", testsPath);

            // Templates
            var componentTemplatePath = Path.Combine(projectPath, "Core\\Templates\\ComponentTemplate");
            var pageTemplatePath = Path.Combine(projectPath, "Core\\Templates\\PageTemplate");
            var testTemplatePath = Path.Combine(projectPath, "Core\\Templates\\TestTemplate");
            Paths.Add("ComponentTemplate", componentTemplatePath);
            Paths.Add("PageTemplate", pageTemplatePath);
            Paths.Add("TestTemplate", testTemplatePath);

            // Add_ Templates
            var addComponentTemplatePath = Path.Combine(projectPath, "Core\\Templates\\AddComponentTemplate");
            var addPageTemplatePath = Path.Combine(projectPath, "Core\\Templates\\AddPageTemplate");
            Paths.Add("AddComponentTemplate", addComponentTemplatePath);
            Paths.Add("AddPageTemplate", addPageTemplatePath);
        }

        private static List<SelectOption> GetAvailableFilesInProject(string filetype)
        {
            var filesFound = new List<SelectOption>();
            var files = Directory.GetFiles(Paths[filetype]);

            foreach(var file in files) {
                string[] filesplit = file.Split('\\');
                string filename = filesplit[filesplit.Length - 1].Replace(".cs", string.Empty) + "\n";
                filesFound.Add(
                    new SelectOption(
                            filename
                        )
                    );
            }

            return filesFound;
        }

        private static void GetAvailablePagesAndComponents()
        {
            AvailableComponents.Clear();
            AvailablePages.Clear();

            AvailableComponents.AddRange(
                GetAvailableFilesInProject("Components")
                );
            AvailablePages.AddRange(
                GetAvailableFilesInProject("Pages")
                );
            AvailableComponents.Add(
                new SelectOption("-- I'm done --\n")
                );
            AvailablePages.Add(
                new SelectOption("-- I'm done --\n")
                );
        }

        #endregion
    }

    public struct Option
    {
        public Option(string text, AnswerType type)
        {
            Text = text;
            Type = type;
            SelectOptions = null;
        }
        public Option(string text, AnswerType type, List<SelectOption> selectOptions)
        {
            Text = text;
            Type = type;
            SelectOptions = selectOptions;
        }

        public string Text { get; }
        public AnswerType Type { get; }
        public List<SelectOption> SelectOptions { get; set; }
    }

    public class SelectOption
    {
        public SelectOption(string text)
        {
            Text = text;
            Selected = false;
        }

        public string Text { get; }
        public bool Selected { get; set; }
    }

    public enum AnswerType
    {
        YesOrNo,
        Name,
        Path,
        Number,
        List,
        Menu
    }

    internal enum InfoType
    {
        Error,
        Success,
        Warning
    }
}