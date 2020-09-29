using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using WFSTestFramework.Base;
using WFSTestFramework.ComponentHelper;
using WFSTestFramework.Settings;

namespace WFSTestFramework.TestScripts
{
    [TestFixture]
    internal class International : BaseClass
    {
        [Test]
        [Description(@"Level 2 international page exists and the header disclaimer is accurate.")]
        public void InternationalDisclaimer()
        {
            string filePath = @"C:\Users\mbear0\Desktop\sites.csv";
            List<string> data = new List<string>();
            data = loadCsvFile(filePath);
            List<string> fails = new List<string> { };

            for (int i = 0; i < data.Count; i++)
            {
                var values = data[i].Split(';');

                NavigationHelper.NavigateToUrl(values[0]);
                try
                {
                    ObjectRepository.Driver.FindElement(By.XPath("//ul/li/a/span/span[text()=\"International\"]"));
                    Console.WriteLine(string.Format("Checking International Disclaimer at {0}", values[0]));

                    string disclaimer = ObjectRepository.Driver
                        .FindElement(By.XPath(
                            "//div[@class=\"dynamic-motto-title noindex\"]/div[@class=\"dynamic-motto noindex\"]"))
                        .GetAttribute("innerText");
                    if (!disclaimer.Contains("Department of Education trading as Education Queensland International (EQI)") || !disclaimer.Contains("CRICOS Provider Code: 00608A"))
                    {
                        fails.Add(string.Format("{0} issue with disclaimer found", values[0]));
                    }
                }
                catch (NoSuchElementException)
                {
                    TestContext.Progress.WriteLine("No International Section found. Do not need to check for disclaimer.");
                    continue;
                }
            }
            Assert.IsTrue(fails.Count <= 0, String.Join("\n", fails));
        }

        public List<string> loadCsvFile(string filePath)
        {
            var reader = new StreamReader(File.OpenRead(filePath));
            List<string> searchList = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                searchList.Add(line);
            }
            return searchList;
        }
    }
}