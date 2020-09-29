using System;
using System.Configuration;
using WFSTestFramework.Interfaces;
using WFSTestFramework.Settings;

namespace WFSTestFramework.Configuration
{
    public class AppConfigReader : IConfig
    {
        public BrowserType GetBrowser()
        {
            string browser = ConfigurationManager.AppSettings.Get(AppConfigKeys.Browser);
            return (BrowserType)Enum.Parse(typeof(BrowserType), browser);
        }

        public string GetWebsite() => ConfigurationManager.AppSettings.Get(AppConfigKeys.Site);

        public bool GetHeadless() => bool.Parse(ConfigurationManager.AppSettings.Get(AppConfigKeys.Headless));
    }
}