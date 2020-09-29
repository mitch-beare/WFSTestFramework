using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using WFSTestFramework.Configuration;
using WFSTestFramework.CustomException;
using WFSTestFramework.Settings;

namespace WFSTestFramework.Base
{
    [SetUpFixture]
    public class BaseClass
    {
        private static FirefoxOptions GetFirefoxOptions()
        {
            FirefoxOptions options = new FirefoxOptions { AcceptInsecureCertificates = true };
            if (ObjectRepository.Config.GetHeadless())
            {
                options.AddArgument("--headless");
            }
            return options;
        }

        private static IWebDriver GetFireFoxDriver()
        {
            IWebDriver driver = new FirefoxDriver(GetFirefoxOptions());
            return driver;
        }

        private static ChromeOptions GetChromeOptions()
        {
            ChromeOptions options = new ChromeOptions { AcceptInsecureCertificates = true };
            if (ObjectRepository.Config.GetHeadless())
            {
                options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-gpu");
            }
            return options;
        }

        private static IWebDriver GetChromeDriver()
        {
            IWebDriver driver = new ChromeDriver(GetChromeOptions());
            return driver;
        }

        private static EdgeOptions GetEdgeOptions()
        {
            EdgeOptions options = new EdgeOptions { AcceptInsecureCertificates = true };
            return options;
        }

        private static IWebDriver GetEdgeDriver()
        {
            IWebDriver driver = new EdgeDriver(GetEdgeOptions());
            return driver;
        }

        [OneTimeSetUp]
        public void InitializeWebDriver()
        {
            ObjectRepository.Config = new AppConfigReader();
            BrowserType browser;
            if (string.IsNullOrEmpty(TestContext.Parameters.Get("browser")))
            {
                browser = ObjectRepository.Config.GetBrowser();
            }
            else
            {
                browser = (BrowserType)Enum.Parse(typeof(BrowserType), TestContext.Parameters.Get("browser"));
            }

            switch (browser)
            {
                case BrowserType.Firefox:
                    ObjectRepository.Driver = GetFireFoxDriver();
                    break;

                case BrowserType.Chrome:
                    ObjectRepository.Driver = GetChromeDriver();
                    break;

                case BrowserType.Edge:
                    ObjectRepository.Driver = GetEdgeDriver();
                    break;

                default:
                    throw new NoSuitableDriverFound("Driver Not Found : " + browser.ToString());
            }
            ObjectRepository.ActionManager = new Actions(ObjectRepository.Driver);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (ObjectRepository.Driver == null) return;
            ObjectRepository.Driver.Close();
            ObjectRepository.Driver.Quit();
        }
    }
}