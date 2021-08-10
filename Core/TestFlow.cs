using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Text;

namespace Excelsa.Core
{
    public class TestFlow
    {
        [SetUp]
        public void Login()
        {


            WebDriver.Driver = new ChromeDriver();

            var urls = GlobalVariables.FallbackUrls;
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
        public void EndOfTest()
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
    }
}
