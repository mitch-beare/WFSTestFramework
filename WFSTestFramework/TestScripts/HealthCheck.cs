using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WFSTestFramework.Base;
using WFSTestFramework.ComponentHelper;
using WFSTestFramework.Settings;

namespace WFSTestFramework.HealthCheck
{
    [TestFixture]
    public class HealthCheck : BaseClass
    {
        [SetUp]
        public void SetupHomePage()
        {
            var url = TestContext.Parameters.Get("url", "https://dakabinshs.eq.edu.au/");
            if (ObjectRepository.Driver.Url != url)
            {
                NavigationHelper.NavigateToUrl(url);
            }
        }

        [Test]
        [Description(@"Recent Events Webpart is present on the homepage")]
        public void RecentEventsPresent()
        {
            TestContext.Progress.WriteLine("Checking for Recent Events Webpart.");
            Assert.AreNotEqual(ObjectRepository.Driver.FindElement(By.XPath("//div[contains(@class,\"eventscalendar-app\")]")), null, "Failed to find Upcoming Events Webpart on Homepage");
        }

        [Test]
        [Description(@"Home page slider images are size 2048 x 600")]
        public void HomePageSliderSize()
        {
            TestContext.Progress.WriteLine("Checking Home Page Slider Image Sizes");
            ReadOnlyCollection<IWebElement> homeSliders = ObjectRepository.Driver.FindElements(By.XPath("//div[@class=\"slick-track\"]//div[not(contains(@class, \"slick-cloned\"))]/img  "));
            List<string> fails = new List<string>();

            foreach (var homeSlider in homeSliders)
            {
                int w = int.Parse(homeSlider.GetAttribute("naturalWidth"));
                int h = int.Parse(homeSlider.GetAttribute("naturalHeight"));

                if (w != 2048 || h != 600)
                {
                    string message = homeSlider.GetAttribute("src").Split('/').Last() + " has size" + w.ToString() + " x " + h.ToString();
                    fails.Add(message);
                }
            }
            Assert.IsTrue(fails.Count <= 0, String.Join("\n", fails));
        }

        [Test]
        [Description(@"Home page slider names are slide-0x.jpg")]
        public void HomePageSliderName()
        {
            TestContext.Progress.WriteLine("Checking Home Page Slider Image Names");
            string name;
            ReadOnlyCollection<IWebElement> homeSliders = ObjectRepository.Driver.FindElements(By.XPath("//div[@class=\"slick-track\"]//div[not(contains(@class, \"slick-cloned\"))]/img  "));
            List<string> fails = new List<string>();
            foreach (var homeSlider in homeSliders)
            {
                name = homeSlider.GetAttribute("src").Split('/').Last();
                Regex rx = new Regex("slide-0[1-6].jpg");
                if (!rx.IsMatch(name.Trim()))
                    fails.Add(name + " does not match format slide-0x.jpg");
            }
            Assert.IsTrue(fails.Count <= 0, String.Join("\n", fails));
        }

        [Test]
        [Description(@"Level 2 international page exists and the header disclaimer is accurate.")]
        public void InternationalDisclaimer()
        {
            try
            {
                ObjectRepository.Driver.FindElement(By.XPath("//ul/li/a/span/span[text()=\"International\"]"));
                TestContext.Progress.WriteLine("Checking International Disclaimer");
            }
            catch (NoSuchElementException)
            {
                TestContext.Progress.WriteLine("No International Section found. Do not need to check for disclaimer.");
                return;
            }

            string disclaimer = ObjectRepository.Driver
                .FindElement(By.XPath(
                    "//div[@class=\"dynamic-motto-title noindex\"]/div[@class=\"dynamic-motto noindex\"]"))
                .GetAttribute("innerText");
            StringAssert.Contains("Department of Education trading as Education Queensland International (EQI)", disclaimer, "Issue found in first line of disclaimer please review.");
            StringAssert.Contains("CRICOS Provider Code: 00608A", disclaimer, "Issue found in second line of disclaimer please review.");
        }

        [Test]
        [Description(@"Documents webpart exists on the /forms-and-documents/documents page and sorts documents in Ascending order")]
        public void DocumentsOrder()
        {
            TestContext.Progress.WriteLine("Checking Forms and Documents Sort order.");
            NavigationHelper.NavigateToUrl(TestContext.Parameters.Get("url") +
                                           "/support-and-resources/forms-and-documents/documents");
            ReadOnlyCollection<IWebElement> docs =
                ObjectRepository.Driver.FindElements(By.XPath("//div/h3[contains(@class, \"title\")]/a"));

            int howTrue = 0;
            for (int i = 2; i < docs.Count; i++)
            {
                howTrue += Convert.ToInt32(String.Compare(docs[i - 1].GetAttribute("innerText"),
                    docs[i].GetAttribute("innerText"), StringComparison.OrdinalIgnoreCase) < 0);
            }

            Assert.IsTrue(howTrue >=
                          docs.Count *
                          0.8, "Documents webpart is sorting by Descending order"); //Changes from file to docs can introduce fake false. Make sure Ascending is 80% true
        }

        [Test]
        [Description(@"The default news articles still exist somewhere in the news library.")]
        public void DefaultNewsPresent()
        {
            NavigationHelper.NavigateToUrl(TestContext.Parameters.Get("url") + "/calendar-and-news/news");
            GenericHelper.WaitForWebElement(By.XPath("//div[@class=\"icon-display noindex\"]/img[@name=\"siteCollectionLogo\"]"), TimeSpan.FromSeconds(60));

            List<string> defaultArticles = new List<string>()
            {
                "Every day counts",
                "QSchools app",
                "School watch",
                "QParents app"
            };

            TestContext.Progress.WriteLine(string.Format("Checking for default news: "));
            defaultArticles.ForEach(i => Console.Write("{0}\t", i));

            ReadOnlyCollection<IWebElement> pageCount =
                ObjectRepository.Driver.FindElements(By.XPath("//li[contains(@class, \"paging ng-scope\")]"));

            if (pageCount.Count > 0)
            {
                //Handle Searching multiple pages of news
                foreach (IWebElement pageButton in pageCount)
                {
                    pageButton.Click();

                    foreach (string article in defaultArticles.ToList())
                    {
                        try
                        {
                            GenericHelper.GetElement(By.LinkText(article));
                            defaultArticles.Remove(article);
                        }
                        catch (NoSuchElementException)
                        {
                        }
                    }

                    if (defaultArticles.Count == 0)
                        break;
                }
            }
            else
            // Only one page just search here with no navigation
            {
                foreach (string article in defaultArticles.ToList())
                {
                    try
                    {
                        GenericHelper.GetElement(By.LinkText(article));
                        defaultArticles.Remove(article);
                    }
                    catch (NoSuchElementException)
                    {
                        continue;
                    }
                    if (defaultArticles.Count == 0)
                        break;
                }
            }

            Assert.AreEqual(0, defaultArticles.Count, "Some default articles were not found on site.");
        }

        [Test]
        [Description(@"News is more recent than 4 months.")]
        public void RecentNews()
        {
            TestContext.Progress.WriteLine(string.Format("Checking Age of most recent news items"));

            ReadOnlyCollection<IWebElement> newsDates =
                ObjectRepository.Driver.FindElements(By.XPath("//span[@class=\"time ng-binding ng-scope\"]"));

            DateTime currentDate = DateTime.Now;
            foreach (IWebElement date in newsDates)
            {
                string strDate = date.GetAttribute("innerText");
                DateTime articleDate = DateTime.Parse(strDate);
                int age = (currentDate - articleDate).Days;
                Assert.IsTrue(age <= 112, "Most recent news is over 4 months old and needs to be reviewed.");
            }
        }

        [Test]
        [Description(@"Every level one page contains content of some kind.")]
        public void PageContent()
        {
            TestContext.Progress.WriteLine("Checking Level 1 Pages for the presence of content");
            ReadOnlyCollection<IWebElement> leveledPages = ObjectRepository.Driver.FindElements(By.XPath("//ul/li//a[contains(@class,\"hasdropdowntoggle\")]"));
            List<string> pageLinks = new List<string>();
            List<string> fails = new List<string>();
            foreach (IWebElement button in leveledPages)
            {
                GenericHelper.HighlightElement(button);
                pageLinks.Add(button.GetAttribute("href"));
                GenericHelper.UnhighlightElement(button);
            }

            foreach (string link in pageLinks)
            {
                NavigationHelper.NavigateToUrl(link);
                GenericHelper.WaitForWebElement(By.XPath("//div[@class=\"icon-display noindex\"]/img[@name=\"siteCollectionLogo\"]"), TimeSpan.FromSeconds(60));

                IWebElement content = null;
                IWebElement webParts = null;
                try
                {
                    content = ObjectRepository.Driver.FindElement(By.XPath("//div[@class=\"ms-rtestate-field\"]/*"));
                    GenericHelper.HighlightElement(content);
                }
                catch (NoSuchElementException)
                {
                    try
                    {
                        content = ObjectRepository.Driver.FindElement(
                            By.XPath("//div[@class=\"ms-rtestate-field\" and text()]"));
                        GenericHelper.HighlightElement(content);
                    }
                    catch (NoSuchElementException)
                    {
                    }
                }
                try
                {
                    webParts = GenericHelper.GetElement(By.XPath(
                        "//div[@class=\"wbp\"]/div[contains(@class, \"ms-webpart\") and not(contains(@style, \"none\"))]"));
                    GenericHelper.HighlightElement(webParts);
                }
                catch (NoSuchElementException)
                {
                }

                if (content == null && webParts == null)
                    fails.Add(link);
            }

            Assert.IsTrue(fails.Count <= 0, "No content was found on these pages {0}", String.Join(",", fails));
        }

        [Test]
        [Description(@"All links that are not javascript controlled actions are functioning correctly and their destinations exist.")]
        public void ZBrokenLinks()
        {
            TestContext.Progress.WriteLine(string.Format("Scanning entire site for links and verifying links status"));
            NavigationHelper.NavigateToUrl(ObjectRepository.Driver.Url + "/site-map");

            ReadOnlyCollection<IWebElement> mappedPages = ObjectRepository.Driver.FindElements(By.XPath("//li[@class=\"area\"]/a[not(contains(@class,\"expandarrow\"))]"));
            List<string> pages = new List<string>();
            List<string> fails = new List<string>();

            foreach (IWebElement pageLink in mappedPages)
            {
                string link = pageLink.GetAttribute("href");
                if (link != ObjectRepository.Driver.Url)
                    pages.Add(pageLink.GetAttribute("href"));
            }

            foreach (string page in pages)
            {
                try
                {
                    NavigationHelper.NavigateToUrl(page);
                    GenericHelper.WaitForWebElement(By.XPath("//div[@class=\"icon-display noindex\"]/img[@name=\"siteCollectionLogo\"]"), TimeSpan.FromSeconds(60));
                    IAlert alert = ExpectedConditions.AlertIsPresent().Invoke(ObjectRepository.Driver);
                    if (alert != null)
                    {
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    fails.Add(page + " Timed out after : 60 seconds.");
                }

                //if the news page is older then a year don't check it move on.
                ReadOnlyCollection<IWebElement> dateLine = ObjectRepository.Driver.FindElements(By.XPath("//div[@class='date-line']"));

                if (dateLine.Count > 0)
                {
                    //This is a new article make sure we care about checking it.
                    DateTime articleDate = DateTime.Parse(dateLine[0].GetAttribute("innerHTML"));
                    if ((DateTime.Now - articleDate).TotalDays < 365)
                    {
                        Console.WriteLine(articleDate.ToString());
                        break;
                    }
                }
                ReadOnlyCollection<IWebElement> foundLinks = ObjectRepository.Driver.FindElements(By.TagName("a"));

                Parallel.ForEach(foundLinks, (foundLink) =>
               {
                   string url = "";
                   try
                   {
                       GenericHelper.HighlightElement(foundLink);
                       url = foundLink.GetAttribute("href");
                   }
                   catch (StaleElementReferenceException)
                   {
                   }
                   if (url != "javascript:void(0);" && url != "javascript: {}" && url != "javascript:window.print();" && !pages.Contains(url) && !String.IsNullOrEmpty(url) && !url.Contains("tel:") && !url.Contains("mailto:") && !url.Contains("/_layouts/15/Authenticate.aspx"))
                   {
                       try
                       {
                           Uri urlChecked = new Uri(url);
                           HttpClient client = new HttpClient();

                           var res = client.GetAsync(urlChecked).Result;
                           HttpStatusCode statusCode = res.StatusCode;

                           if (statusCode == HttpStatusCode.Moved || statusCode == HttpStatusCode.NotFound)
                           {
                               fails.Add(url + " status: " + statusCode + " on page " + page + Environment.NewLine);
                           }
                       }
                       catch (Exception e)
                       {
                           TestContext.WriteLine(e);
                       }
                   }
                   GenericHelper.UnhighlightElement(foundLink);
               });

                Assert.IsTrue(fails.Count <= 0, String.Join("\n", fails));
            }
        }

        [Test]
        [Description(@"News archive is maintained and does not contain aged content.")]
        public void NewsArchive()
        {
            NavigationHelper.NavigateToUrl(TestContext.Parameters.Get("url") + "/calendar-and-news/news");
            GenericHelper.WaitForWebElement(By.XPath("//div[@class=\"icon-display noindex\"]/img[@name=\"siteCollectionLogo\"]"), TimeSpan.FromSeconds(60));

            TestContext.Progress.WriteLine(string.Format("Checking age of news: "));

            ReadOnlyCollection<IWebElement> pageCount =
                ObjectRepository.Driver.FindElements(By.XPath("//li[contains(@class, 'paging ng-scope')]"));

            if (pageCount.Count > 0)
            {
                //Handle Searching multiple pages of news
                foreach (IWebElement pageButton in pageCount)
                {
                    pageButton.Click();
                    GenericHelper.WaitForWebElement(By.XPath("//div[@class=\"icon-display noindex\"]/img[@name=\"siteCollectionLogo\"]"), TimeSpan.FromSeconds(60));
                    ReadOnlyCollection<IWebElement> dateParts = ObjectRepository.Driver.FindElements(By.XPath("//span[@class='time ng-binding ng-scope']"));

                    foreach (IWebElement date in dateParts)
                    {
                        DateTime articleDate = DateTime.Parse(date.GetAttribute("innerText"));
                        Assert.IsTrue((DateTime.Now - articleDate).TotalDays < 365);
                    }
                }
            }
            else
            // Only one page just search here with no navigation
            {
                ReadOnlyCollection<IWebElement> dateParts = ObjectRepository.Driver.FindElements(By.XPath("//span[@class='time ng-binding ng-scope']"));
                foreach (IWebElement date in dateParts)
                {
                    DateTime articleDate = DateTime.Parse(date.GetAttribute("innerText"));
                    Assert.IsTrue((DateTime.Now - articleDate).TotalDays < 365);
                }
            }
        }
    }
}