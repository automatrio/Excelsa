using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Excelsa.Core
{
    public static class GlobalVariables
    {
        public static string Url { get; set; }
        public static List<string> FallbackUrls { get; set; }
        public static bool EmbeddedLogin { get; set; }
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static bool TakeScreenshot { get; set; }  
        public static string ScreenshotFolderPath { get; set; }
        public static string LogFolderPath { get; set; }
        public static float WaitTimePadding { get; set; } = 1.00f;

        public static StringBuilder MainLog { get; set; } = new StringBuilder();
        public static Dictionary<string, string> GetProjectPaths(Type currentInstanceType, bool onlyCorePath = false)
        {
            var paths = new Dictionary<string, string>();

            // Project Path
            string path = Assembly.GetAssembly(currentInstanceType).Location;
            var pathSplit = path.Split('\\').ToList();
            pathSplit = pathSplit.GetRange(0, pathSplit.Count() - 3);
            var projectPath = "";
            foreach (var folder in pathSplit) projectPath += folder + '\\';
            paths.Add("ProjectPath", projectPath);

            if (onlyCorePath) return paths;

            // Components Path
            var componentsPath = Path.Combine(projectPath, "Domain\\Components");
            paths.Add("Components", componentsPath);

            // Pages Path
            var pagesPath = Path.Combine(projectPath, "Domain\\Pages");
            paths.Add("Pages", pagesPath);

            // Tests path
            var testsPath = Path.Combine(projectPath, "Main\\Tests");
            paths.Add("Tests", testsPath);

            // Templates
            var componentTemplatePath = Path.Combine(projectPath, "Core\\Templates\\ComponentTemplate");
            var pageTemplatePath = Path.Combine(projectPath, "Core\\Templates\\PageTemplate");
            var testTemplatePath = Path.Combine(projectPath, "Core\\Templates\\TestTemplate");
            paths.Add("ComponentTemplate", componentTemplatePath);
            paths.Add("PageTemplate", pageTemplatePath);
            paths.Add("TestTemplate", testTemplatePath);

            // Add_ Templates
            var addComponentTemplatePath = Path.Combine(projectPath, "Core\\Templates\\AddComponentTemplate");
            var addPageTemplatePath = Path.Combine(projectPath, "Core\\Templates\\AddPageTemplate");
            paths.Add("AddComponentTemplate", addComponentTemplatePath);
            paths.Add("AddPageTemplate", addPageTemplatePath);

            return paths;
        }
    }
}