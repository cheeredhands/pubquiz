using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Pubquiz.Domain.Tests
{
    [TestCategory("Localization")]
    [TestClass]
    public class LocalizationTests
    {
        [TestMethod]
        public void ResultCodes_MustBeLocalized()
        {
            // Make sure all missing codes are caught first and then displayed.
            var missing = new List<string>();
            
            var values = Enum.GetNames(typeof(ResultCode));
  
            var localeFiles = GetLocales();
            foreach (var file in localeFiles)
            {
                var content = File.ReadAllText(file.FullName);
                var elements = JObject.Parse(content);

                foreach (var resultCode in values)
                {
                    if (elements.ContainsKey(resultCode))
                    {
                        continue;
                    }
                    
                    // coming here means, the resultCode was not found.
                    missing.Add($"Result code {resultCode} not found in {file.Name}.");
                }
            }

            if (missing.Any())
            {
                Assert.Fail(string.Join(Environment.NewLine, missing));
            }
        }

        private static IEnumerable<FileInfo> GetLocales()
        {
            // read the folder with the locales, find the configured locales
            // and return them to use in the test.
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine($"1: {currentFolder}");
            
            var rootFolder = new DirectoryInfo(currentFolder).Parent.Parent.Parent.Parent.Parent;
            Console.WriteLine($"2: {rootFolder}");

            // ReSharper disable once PossibleNullReferenceException : live on the edge ...
            var localesFolder = Path.Combine(rootFolder.FullName, "team-frontend", "src", "locales");
            Console.WriteLine($"3: {localesFolder}");
                        
            var locales = Directory.GetFiles(localesFolder, "*.json")
                .Select(item => new FileInfo(item))
                .ToArray();

            return locales;
        }
    }
}