using WFSTestFramework.Settings;

namespace WFSTestFramework.ComponentHelper
{
    public class WindowHelper
    {
        public static string GetTitle()
        {
            return ObjectRepository.Driver.Title;
        }

        public static string GetActiveWindowId()
        {
            return ObjectRepository.Driver.CurrentWindowHandle;
        }
    }
}