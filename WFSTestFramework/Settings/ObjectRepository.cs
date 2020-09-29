using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WFSTestFramework.Interfaces;

namespace WFSTestFramework.Settings
{
    public class ObjectRepository
    {
        public static IConfig Config { get; set; }

        public static IWebDriver Driver { get; set; }
        public static Actions ActionManager { get; set; }
    }
}