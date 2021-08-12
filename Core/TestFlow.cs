using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Excelsa.Core
{
    [TestFixture]
    public class TestFlow
    {
        private readonly IConfigurationRoot _config;

        public TestFlow()
        {
            var projectPath = GlobalVariables.GetProjectPaths(typeof(TestFlow), onlyCorePath: true);

            var builder = new ConfigurationBuilder()
               .AddJsonFile(
                Path.Combine(projectPath["ProjectPath"], "Core\\appsettings.json"),
                optional: false,
                reloadOnChange: true);

            _config = builder.Build();
        }

        [SetUp]
        public void BeginTest()
        {
            LoadGlobalVariables(_config);

            WebDriver.Driver = new ChromeDriver();

            var urls = GlobalVariables.FallbackUrls ?? new List<string>();
            urls.Insert(0, GlobalVariables.Url);

            foreach(var currentUrl in urls)
            {
                try
                {
                    if (GlobalVariables.EmbeddedLogin)
                    {
                        WebDriver
                            .Driver
                            .Navigate()
                            .GoToUrl(
                                $"http://{GlobalVariables.Login}:{GlobalVariables.Password}" + "@" + currentUrl
                                );
                    }
                    else
                    {
                        WebDriver.Driver.Navigate().GoToUrl(currentUrl);
                    }
                    WebDriver.Driver.Manage().Window.Maximize();
                    break;
                }
                catch (Exception)
                {
                    continue;
                }
            }    
        }

        [TearDown]
        public void EndTest()
        {
            TakeScreenshotIfNeeded();

            string fileLog = GetType().Name;
            string filePathLog =
                Path.Combine(
                        GlobalVariables.LogFolderPath,
                        DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileLog + ".jpg"
                    );

            string HTMLpath = @"\Core\Templates\HTML.txt";
            string CSSpath = @"\Core\Templates\CSS.txt";
            string JSpath = @"\Core\Templates\JS.txt";

            using (var writer = new StreamWriter(filePathLog))
            {
                var document = new StringBuilder();

                var html = new StreamReader(HTMLpath);
                document.Append(html.ReadToEnd());
                html.Close();

                var css = new StreamReader(CSSpath);
                document.Replace("STYLES_CONTENT", css.ReadToEnd());
                css.Close();

                var js = new StreamReader(JSpath);
                document.Replace("JAVASCRIPT_CONTENT", js.ReadToEnd());
                js.Close();

                document.Replace("TESTS_CONTENT", GlobalVariables.MainLog.ToString());

                document.Replace("DATETIME_CONTENT", DateTime.Now.ToString());

                writer.WriteLine(document.ToString());
            }

            WebDriver.Driver.Quit();
        }

        private void TakeScreenshotIfNeeded()
        {
            if (GlobalVariables.TakeScreenshot)
            {
                string filename = GetType().Name;
                Screenshot image = ((ITakesScreenshot)WebDriver.Driver).GetScreenshot();
                string filePathPrint =
                    Path.Combine(
                            GlobalVariables.ScreenshotFolderPath,
                            DateTime.Now.ToString("yyyy-MM-dd") + "_" + filename + ".jpg"
                        );

                image.SaveAsFile(filePathPrint, ScreenshotImageFormat.Jpeg);
            }
        }

        private static void LoadGlobalVariables(IConfiguration config)
        {
            var properties = typeof(GlobalVariables).GetProperties();

            foreach(var prop in properties)
            {
                var value = config.GetSection(prop.Name).Value;
                var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(null, convertedValue);
            }
        }
    }
}
