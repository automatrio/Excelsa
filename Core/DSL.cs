using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Excelsa.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Excelsa.Core
{
    public class ExcelsaDSL
    {

        /// <summary>
        /// Writes some <paramref name="text"/>. An element must be focused, lest the written text
        /// is written nowhere.
        /// </summary>
        /// <param name="word"></param>
        public static void WriteText(string text)
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(text).Build().Perform();
            Wait(1000);
        }

        /// <summary>
        /// Presses the spacebar.
        /// </summary>
        public static void Space()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.Space).Build().Perform();
            Wait(2000);
        }

        /// <summary>
        /// Presses the PageUp key.
        /// </summary>
        public static void PageUp()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.PageUp).Build().Perform();
            Wait(2000);
        }

        /// <summary>
        /// Presses the PageDown key.
        /// </summary>
        public static void PageDown()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.PageDown).Build().Perform();
            Wait(150);
        }

        /// <summary>
        /// Presses the Enter key.
        /// </summary>
        public void Enter()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.Enter).Build().Perform();
        }

        /// <summary>
        /// Presses the escape key.
        /// </summary>
        public void Esc()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.Escape).Build().Perform();
            Wait(2000);
        }

        /// <summary>
        /// Presses the Arrow Up key.
        /// </summary>
        public static void ArrowUp()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.ArrowUp).Build().Perform();
            Wait(2000);
        }

        /// <summary>
        /// Presses the Arrow Down key.
        /// </summary>
        public static void ArrowDown()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.ArrowDown).Build().Perform();
        }

        /// <summary>
        /// Presses either the Arrow Up or the Arrow Down key, depending on whether
        /// a negative (up) or positive (down) value has been input into <paramref name="repeat"/>.
        /// </summary>
        /// <param name="repeat"></param>
        public static void ArrowScroll(int repeat)
        {
            var direction = (repeat > 0) ? Keys.ArrowDown : Keys.ArrowUp;

            for (int i = 0; i < Math.Abs(repeat); i++)
            {
                Actions act = new Actions(WebDriver.Driver);
                act.SendKeys(direction).Build().Perform();
                Wait(100);
            }
        }

        /// <summary>
        /// Presses the Arrow Left key.
        /// </summary>
        public void ArrowLeft()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.ArrowLeft).Build().Perform();
            Wait(2000);
        }

        /// <summary>
        /// Presses the Arrow Right key.
        /// </summary>
        public void ArrowRight()
        {
            Actions act = new Actions(WebDriver.Driver);
            act.SendKeys(Keys.ArrowRight).Build().Perform();
            Wait(2000);
        }

        /// <summary>
        /// Zooms the screen to a certain <paramref name="percentage"/>. Note that ChromeDriver is known to have bugs that might
        /// impair the test after this operation.
        /// </summary>
        /// <param name="zoom"></param>
        public void Zoom(string percentage) // Preencher porcentagem - Exemplo: 70%
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver.Driver;
            js.ExecuteScript("document.body.style.zoom = '" + percentage + "'");
        }

        /// <summary>
        /// Waits for a given <paramref name="time"/>, which will be multiplied
        /// by the WaitTimePadding property.
        /// </summary>
        /// <param name="tempo"></param>
        public static void Wait(int time)
        {
            Thread.Sleep((int)(time * GlobalVariables.WaitTimePadding));
        }

        /// <summary>
        /// Retrieves an element that matches the given <paramref name="selector"/>.
        /// This overload will attempt to obtain the method of a default minimum of 3 seconds.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="elementType"></param>
        /// <returns>
        /// An IWebElement, if found, or null.
        /// </returns>
        public static IWebElement GetElement(string selector, SelectorType elementType)
        {
            try
            {
                var wait = new WebDriverWait(WebDriver.Driver, new TimeSpan(0, 0, 3));

                switch (elementType)
                {
                    case SelectorType.Id:
                        return wait.Until(c => c.FindElement(By.Id(selector)));
                    case SelectorType. TagName:
                        return wait.Until(c => c.FindElement(By.Name(selector)));
                    case SelectorType.ClassName:
                        return wait.Until(c => c.FindElement(By.ClassName(selector)));
                    case SelectorType.CssSelector:
                        return wait.Until(c => c.FindElement(By.CssSelector(selector)));
                    case SelectorType.XPath:
                        return wait.Until(c => c.FindElement(By.XPath(selector)));
                    default:
                        return wait.Until(c => c.FindElement(By.LinkText(selector)));
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves an element that matches the given <paramref name="selector"/>,
        /// and will keep on trying for as long as the specified <paramref name="timeout"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="elementType"></param>
        /// <returns>
        /// An IWebElement, if found, or null.
        /// </returns>
        public static IWebElement GetElement(string element, SelectorType elementType, int timeout)
        {
            try
            {
                var wait = new WebDriverWait(WebDriver.Driver, new TimeSpan(0, 0, timeout));

                switch (elementType)
                {
                    case SelectorType.Id:
                        return wait.Until(c => c.FindElement(By.Id(element)));
                    case SelectorType.TagName:
                        return wait.Until(c => c.FindElement(By.Name(element)));
                    case SelectorType.ClassName:
                        return wait.Until(c => c.FindElement(By.ClassName(element)));
                    case SelectorType.CssSelector:
                        return wait.Until(c => c.FindElement(By.CssSelector(element)));
                    case SelectorType.XPath:
                        return wait.Until(c => c.FindElement(By.XPath(element)));
                    default:
                        return wait.Until(c => c.FindElement(By.LinkText(element)));
                }
            }
            catch
            {
                return null;
            }
        }
       
        /// <summary>
        /// Gets all elements that either match the provided <paramref name="selector"/>
        /// or one of the <paramref name="fallbackSelectors"/>.
        /// Because of its specificity, XPaths are not allowed in this method.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="type"></param>
        /// <returns>
        /// A list containing such elements, or null if there were no elements.
        /// </returns>
        public static List<IWebElement> GetHTMLCollection(
            string selector,
            SelectorType type,
            params string[] fallbackSelectors)
        {
            string script;
            object result = null;
            bool success = false;
            var selectors = fallbackSelectors.ToList();
            selectors.Insert(0, selector);

            foreach (var sel in selectors)
            {
                try
                {
                    switch (type)
                    {
                        case SelectorType.Id:
                            script = $"Array.from(document.querySelector('#{sel}'))";
                            result = WebDriver.Driver.Scripts().ExecuteScript(script);
                            break;
                        case SelectorType.TagName:
                            script = $"Array.from(document.getElementsByTagName('{sel}'))";
                            result = WebDriver.Driver.Scripts().ExecuteScript(script);
                            break;
                        case SelectorType.ClassName:
                            script = $"Array.from(document.getElementsByClassName('{sel}'))";
                            result = WebDriver.Driver.Scripts().ExecuteScript(script);
                            break;
                        default:
                            script = $"Array.from(document.querySelector('{sel}'))";
                            result = WebDriver.Driver.Scripts().ExecuteScript(script);
                            break;
                    }
                    success = true;
                }
                catch
                {
                    continue;
                }

                if (success) break;
            }

            if (success)
                return ((IEnumerable) result).Cast<IWebElement>().ToList();
            else
                return null;
        }

        /// <summary>
        /// Executes a sequence of scripts until one of them succeeds, or else it
        /// will return null.
        /// </summary>
        /// <param name="scripts"></param>
        /// <returns>
        /// An object boxing either an int, a long, a bool, a string, an IWebElement, or
        /// a list of such types, obtained by the script invoked.
        /// </returns>
        public static object ExecuteScript(params string[] scripts)
        {
            for (int i = 0; i < scripts.Length; i++)
            {
                try
                {
                    return WebDriver.Driver.Scripts().ExecuteScript(scripts[i]);
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }

        /// <summary>
        /// Given a general <paramref name="selector"/>, like "div" or "span", this methods searches across 
        /// the whole DOM tree for the element with the specified <paramref name="innerText"/>.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <returns>
        /// An IWebElement representing the first one of a collection of elements matching the selector
        /// and text provided.
        /// </returns>
        public static IWebElement GetElementByInnerText(string selector, string innerText, SelectorType type)
        {
            string script;

            switch (type)
            {
                case SelectorType.Id:
                    script = $"return Array.from(document.querySelector('#{selector}')).filter(el => el.innerText === '{innerText}')[0]";
                    return WebDriver.Driver.Scripts().ExecuteScript(script) as IWebElement;
                case SelectorType.TagName:
                    script = $"return Array.from(document.getElementsByTagName('{selector}')).filter(el => el.innerText === '{innerText}')[0]";
                    return WebDriver.Driver.Scripts().ExecuteScript(script) as IWebElement;
                case SelectorType.ClassName:
                    script = $"return Array.from(document.getElementsByClassName('{selector}')).filter(el => el.innerText === '{innerText}')[0]";
                    return WebDriver.Driver.Scripts().ExecuteScript(script) as IWebElement;
                default:
                    script = $"return Array.from(document.querySelector('{selector}')).filter(el => el.innerText === '{innerText}')[0]";
                    return WebDriver.Driver.Scripts().ExecuteScript(script) as IWebElement;
            }
            
        }


        /// <summary>
        /// Runs a <paramref name="script"/> as many times as <paramref name="maxAttempts"/>
        /// or until an element is found. If <paramref name="maxAttempts"/> is not provided,
        /// the method will keep on truing for a minimum of 5 seconds.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="maxAttempts"></param>
        /// <returns>
        /// The IWebElement obtained with the script, or else null.
        /// </returns>
        public static IWebElement GetElementByScript(string script, int maxAttempts = 10)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    return (IWebElement)ExecuteScript(script);
                }
                catch
                {
                    Wait(500);
                    continue;
                }
            }
            return null;
        }
    }
}