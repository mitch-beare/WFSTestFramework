using WFSTestFramework.Configuration;

namespace WFSTestFramework.Interfaces
{
    public interface IConfig
    {
        BrowserType GetBrowser();

        string GetWebsite();

        bool GetHeadless();
    }
}