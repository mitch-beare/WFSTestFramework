using OpenQA.Selenium;

namespace WFSTestFramework.ComponentHelper
{
    public class RadioButtonHelper
    {
        private static IWebElement _element;

        public static void ClickRadioButton(By locator)
        {
            _element = GenericHelper.GetElement(locator);
            _element.Click();
        }

        public static bool IsRadioButtonSelected(By locator)
        {
            _element = GenericHelper.GetElement(locator);
            string flag = _element.GetAttribute("checked");
            if (flag == null)
                return false;
            return flag.Equals("true") || flag.Equals("checked");
        }
    }
}