using OpenQA.Selenium;

namespace WFSTestFramework.ComponentHelper
{
    public class CheckBoxHelper
    {
        private static IWebElement _element;

        public void CheckBox(By locator)
        {
            _element = GenericHelper.GetElement(locator);
            _element.Click();
        }

        public static bool IsChecked(By locator)
        {
            _element = GenericHelper.GetElement(locator);
            string flag = _element.GetAttribute("checked");
            return (flag == null);
        }
    }
}