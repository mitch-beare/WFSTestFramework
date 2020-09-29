using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Threading;
using WFSTestFramework.Settings;

namespace WFSTestFramework.ComponentHelper
{
    internal class BrowserHelper
    {
        public static void BrowserMaximize()
        {
            ObjectRepository.Driver.Manage().Window.Maximize();
        }

        public static void GoBack()
        {
            ObjectRepository.Driver.Navigate().Back();
        }

        public static void GoForward()
        {
            ObjectRepository.Driver.Navigate().Forward();
        }

        public static void RefreshPage()
        {
            ObjectRepository.Driver.Navigate().Refresh();
        }

        public static void SwitchToWindow(int index)
        {
            ReadOnlyCollection<string> windows = ObjectRepository.Driver.WindowHandles;
            if (windows.Count < index)
            {
                throw new NoSuchWindowException("Invalid browser window index " + index);
            }
            ObjectRepository.Driver.SwitchTo().Window(windows[index]);
            Thread.Sleep(500);
        }

        public static void SwitchToParent()
        {
            var windowids = ObjectRepository.Driver.WindowHandles;
            for (int i = windowids.Count; i > 1; i--)
            {
                ObjectRepository.Driver.SwitchTo().Window(windowids[i - 1]);
                ObjectRepository.Driver.Close();
            }
        }
    }
}