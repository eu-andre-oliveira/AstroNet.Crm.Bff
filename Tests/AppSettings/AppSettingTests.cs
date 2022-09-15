using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Tests.AppSettings
{
    public class AppSettingTests
    {
        public AppSettingTests()
        {
        }

        [Theory]
        [Trait("Category", "AppSettings")]
        [InlineData("appsettings.Development")]
        [InlineData("appsettings.Staging")]
        [InlineData("appsettings.PreProd")]
        [InlineData("appsettings.Production")]
        public void VerifyAppSettingsContent(string appSettingsEnvironmentName)
        {
            string appSettingsDevName = "appsettings.Dev";

            IConfigurationRoot devConfigurations = GetAppSettingConfigurations(appSettingsDevName);
            IConfigurationRoot environmentConfigurations = GetAppSettingConfigurations(appSettingsEnvironmentName);

            IList<IDictionary<string, string>> devEntryList = GetConfigurationsEntryList(devConfigurations.Providers);
            IList<IDictionary<string, string>> environmentEntryList = GetConfigurationsEntryList(environmentConfigurations.Providers);

            CheckAppSettingsKeyExistence(devEntryList, environmentEntryList, appSettingsDevName, appSettingsEnvironmentName);
            CheckAppSettingsKeyExistence(environmentEntryList, devEntryList, appSettingsEnvironmentName, appSettingsDevName);

            CheckAppSettingsKeyNames(environmentEntryList, appSettingsEnvironmentName);
        }

        private IConfigurationRoot GetAppSettingConfigurations(string appSettingsName)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"{appSettingsName}.json")
                .Build();
            return config;
        }

        private IList<IDictionary<string, string>> GetConfigurationsEntryList(IEnumerable<IConfigurationProvider> configurationProviders)
        {
            IList<IDictionary<string, string>> entryList = new List<IDictionary<string, string>>();
            IDictionary<string, string> entries = new Dictionary<string, string>();
            
            foreach (JsonConfigurationProvider appSettingsProvider in configurationProviders)
            {
                PropertyInfo dataProperty = appSettingsProvider.GetType().GetProperty("Data", BindingFlags.Instance | BindingFlags.NonPublic);
                var dataValue = (IDictionary<string, string>)dataProperty.GetValue(appSettingsProvider);
                foreach (var item in dataValue)
                {
                    entries.Add(item.Key, item.Value);
                }

                entryList.Add(entries);
            }

            return entryList;
        }

        private void CheckAppSettingsKeyExistence(IList<IDictionary<string, string>> sourceEntryList, IList<IDictionary<string, string>> destinationEntryList,
            string sourceAppSettingsName, string destinationAppSettingsName)
        {
            for (int i = 0; i < sourceEntryList.Count; i++)
            {
                foreach (KeyValuePair<string, string> entry in sourceEntryList[i])
                {
                    Assert.True(destinationEntryList[i].Any(x => x.Key == entry.Key), 
                        $"Key {entry.Key} exists on {sourceAppSettingsName} but not in {destinationAppSettingsName}");
                }
            }
        }

        private void CheckAppSettingsKeyNames(IList<IDictionary<string, string>> environmentEntryList, string appSettingsEnvironmentName)
        {
            IDictionary<string, string> customKeys = GetCustomKeys();
            foreach (IDictionary<string, string> entryDictionary in environmentEntryList)
            {
                foreach (KeyValuePair<string, string> entry in entryDictionary)
                {
                    Assert.True(entry.Value.StartsWith("__") && entry.Value.EndsWith("__"),
                        $"Value {entry.Value} doesn't contains __ on {appSettingsEnvironmentName}");

                    var normalizedKey = entry.Key.Replace(":", string.Empty);
                    var normalizedValue = entry.Value.Replace("__", string.Empty);

                    KeyValuePair<string, string> customKey = customKeys.FirstOrDefault(x => x.Key == normalizedKey);
                    if (customKey.Key != null)
                    {
                        normalizedKey = customKey.Value;
                    }

                    Assert.True(normalizedKey == normalizedValue,
                        $"Value {normalizedValue} doesn't match with key {normalizedKey} on {appSettingsEnvironmentName}");
                }
            }
        }

        private IDictionary<string, string> GetCustomKeys()
        {
            IDictionary<string, string> customKeys = new Dictionary<string, string>
            {
                { "SerilogWriteTo0ArgsdatabaseUrl", "SerilogWriteToArgsdatabaseUrl" }
            };

            return customKeys;
        }
    }
}