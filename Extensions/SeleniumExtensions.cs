using Excelsa.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Excelsa.Extensions
{
    public static class SeleniumExtensions
    {
        public static IJavaScriptExecutor Scripts(this IWebDriver driver)
        {
            return (IJavaScriptExecutor)driver;
        }

        /// <summary>
        /// Writes some <paramref name="text"/> to an input element. It's not necessary to have clicked on it first.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void WriteText(this IWebElement element, string text)
        {
            element.Click();
            element.SendKeys(text);
        }

        /// <summary>
        /// Accesses the given <paramref name="element"/> computed styles in order to figure
        /// out its current visibility.
        /// </summary>
        /// <param name="element"></param>
        public static bool IsVisible(this IWebElement element)
        {
            string script = "return getComputedStyle(arguments[0]).visibility === 'visible';";
            var result = WebDriver.Driver.Scripts().ExecuteScript(script, element);

            return Convert.ToBoolean(result);
        }

        public static bool HasChildren(this IWebElement element)
        {
            string script = "return arguments[0].childElementCount > 0";
            return (bool) WebDriver.Driver.Scripts().ExecuteScript(script, element);
        }

        /// <summary>
        /// Gets the child elements of the parent <paramref name="element"/> provided
        /// that match the given <paramref name="selector"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="selector"></param>
        /// <param name="type"></param>
        /// <returns>
        /// A list containing such elements, or null if the element has no children.
        /// </returns>
        public static List<IWebElement>GetChildren(
            this IWebElement element,
            string selector,
            SelectorType type)
        {
            string script;
            object result;

            try
            {
                switch (type)
                {
                    case SelectorType.Id:
                        script = $"return Array.from(arguments[0].getElementById('#{selector}'))";
                        result = WebDriver.Driver.Scripts().ExecuteScript(script, element);
                        break;
                    case SelectorType.TagName:
                        script = $"return Array.from(arguments[0].getElementsByTagName('{selector}'))";
                        result = WebDriver.Driver.Scripts().ExecuteScript(script, element);
                        break;
                    case SelectorType.ClassName:
                        script = $"return Array.from(arguments[0].getElementsByClassName('{selector}'))";
                        result = WebDriver.Driver.Scripts().ExecuteScript(script, element);
                        break;
                    default:
                        script = $"return Array.from(arguments[0].querySelectorAll('{selector}'))";
                        result = WebDriver.Driver.Scripts().ExecuteScript(script, element);
                        break;
                }

                return ((IEnumerable) result).Cast<IWebElement>().ToList();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the child element of the parent <paramref name="element"/> provided that matches
        /// the given <paramref name="selector"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="selector"></param>
        /// <param name="type"></param>
        /// <returns>
        /// An IWebElement representing the child element found, or null.
        /// </returns>
        public static IWebElement GetChild(
            this IWebElement element,
            string selector,
            SelectorType type)
        {
            string script;

            try
            {
                switch (type)
                {
                    case SelectorType.Id:
                        script = $"arguments[0].getElementById('#{selector}')";
                        return (IWebElement) WebDriver.Driver.Scripts().ExecuteScript(script, element);
                    case SelectorType.TagName:
                        script = $"return Array.from(arguments[0].getElementsByTagName('{selector}'))[0]";
                        return (IWebElement) WebDriver.Driver.Scripts().ExecuteScript(script, element);
                    case SelectorType.ClassName:
                        script = $"return Array.from(arguments[0].getElementsByClassName('{selector}'))[0]";
                        return (IWebElement) WebDriver.Driver.Scripts().ExecuteScript(script, element);
                    default:
                        script = $"return arguments[0].querySelector('{selector}')";
                        return (IWebElement) WebDriver.Driver.Scripts().ExecuteScript(script, element);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to click the <paramref name="element"/> provided. If it fails,
        /// it'll obtain instead the element's position and try to perform a click
        /// on the corresponding coordinates.
        /// </summary>
        /// <param name="element"></param>
        public static void TryClick(this IWebElement element)
        {
            try
            {
                element.Click();
            }
            catch
            {
                var (x, y) = element.GetCoordinates();
                var act = new Actions(WebDriver.Driver);
                act.MoveByOffset(x, y).Click().MoveByOffset(-x, -y).Build().Perform();
            }
        }

        /// <summary>
        /// Gets the coordinates of an element on screen.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns>
        /// A tuple containing the x and y coordinates of the element.
        /// </returns>
        public static (int x, int y) GetCoordinates(this IWebElement element)
        {
            var script = "arguments[0].getBoundingClientRect()";

            var x = (int)WebDriver.Driver.Scripts().ExecuteScript(script + ".x", element);
            var y = (int)WebDriver.Driver.Scripts().ExecuteScript(script + ".y", element);

            (int x, int y) point = (x, y);

            return point;
        }

        /// <summary>
        /// Gets the childElementCount property of a given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>
        /// An integer representing the number of children that element has.
        /// </returns>
        public static int CountChildren(this IWebElement element)
        {
            var script = $"return arguments[0].childElementCount";
            object countResult = WebDriver.Driver.Scripts().ExecuteScript(script, element);
            return Convert.ToInt32(countResult);
        }

        /// <summary>
        /// Converts the <paramref name="element"/> provided into a SelectElement
        /// and then clicks the option that has as innerText the given <paramref name="option"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="option"></param>
        public static void SelectDropDownOption(
            this IWebElement element,
            string option)
        {
            new SelectElement(element).SelectByText(option);
        }

        /// <summary>
        /// Clear whatsoever data or text that was previusly input to the given <paramref name="element"/>.
        /// </summary>
        /// <param name="element"></param>
        public static void ClearInput(this IWebElement element)
        {
            element.Clear();
        }

        /// <summary>
        /// Check if the <paramref name="element"/>'s text contains the <paramref name="value"/> provided.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasAsText(this IWebElement element, string value)
        {
            return element.Text.Contains(value);
        }

        /// <summary>
        /// Check if a parent <paramref name="element"/>, and its children altogether, contain
        /// the <paramref name="value"/> provided.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool AndChildrenHaveAsText(this IWebElement element, string value)
        {
            var script = "return arguments[0].innerText";
            var innerText = (string) WebDriver.Driver.Scripts().ExecuteScript(script, element);
            return innerText.Contains(value);
        }
    }
}
