using WFSTestFramework.Settings;

namespace WFSTestFramework.ComponentHelper
{
    public class NavigationHelper
    {
        public static void NavigateToUrl(string Url)
        {
            //todo: Add a url vefiication step here
            ObjectRepository.Driver.Navigate().GoToUrl(Url);
        }
    }
}