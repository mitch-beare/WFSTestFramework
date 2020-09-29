using OpenQA.Selenium;

namespace WFSTestFramework.ComponentHelper
{
    internal class TextBoxHelper
    {
        private static IWebElement element;

        public static void TypeInTextBox(By Locator, string text)
        {
            element = GenericHelper.GetElement(Locator);
            element.SendKeys(text);
        }

        public static void ClearTextBox(By Locator)
        {
            element = GenericHelper.GetElement(Locator);
            element.Clear();
        }
    }
}