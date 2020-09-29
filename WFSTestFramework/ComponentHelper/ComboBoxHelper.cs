using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;

namespace WFSTestFramework.ComponentHelper
{
    public class ComboBoxHelper
    {
        private static SelectElement _select;

        public static void SelectElement(By locator, int index)
        {
            _select = new SelectElement(GenericHelper.GetElement(locator));
            _select.SelectByIndex(index);
        }

        public static void SelectElement(By locator, string text)
        {
            _select = new SelectElement(GenericHelper.GetElement(locator));
            _select.SelectByValue(text);
        }

        public static IList<string> GetAllItem(By locator)
        {
            _select = new SelectElement(GenericHelper.GetElement(locator));
            return _select.Options.Select((x) => x.Text).ToList();
        }
    }
}