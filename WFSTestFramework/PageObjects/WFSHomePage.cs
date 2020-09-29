using OpenQA.Selenium;
using WFSTestFramework.Settings;

namespace WFSTestFramework.PageObjects
{
    public class WFSHomePage
    {
        // Possibly Page Objects should be scoped to the test they are relevant to? Sharepoint is very dynamic rendering a generic page model difficult

        // Sharepoint Page Model is Master Page + Page Layout + Page Content = Rendered Page
        // Query with design team to find out if we have any static assets, master page possibly?

        #region WebElement

        private static By Logo = By.Name("siteCollectionLogo");
        private static By SearchBox = By.XPath("//div[@id=\"SearchBox\"]/div/input");
        private static By LoginButton = By.ClassName("user noindex");

        #endregion WebElement

        #region Actions

        public void QuickSearch(string text)
        {
            ObjectRepository.Driver.FindElement(SearchBox).SendKeys(text);
        }

        #endregion Actions

        #region Navigation

        public void NavigateToSiteMap()
        {
            ObjectRepository.Driver.FindElement(LoginButton).Click();
        }

        #endregion Navigation
    }
}