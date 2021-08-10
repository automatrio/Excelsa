using OpenQA.Selenium;

namespace Excelsa.Core
{
    public enum SelectorType
    {
        Id,
        TagName,
        ClassName,
        CssSelector,
        XPath,
        LinkText
    }
    public class WebDriver
    {
        public static IWebDriver Driver;
    }
}