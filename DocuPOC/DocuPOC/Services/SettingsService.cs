using System.Configuration;

namespace DocuPOC.Services
{
    public interface ISettingsService
    {
        public string GetSettingWithDefault(string key, string defaultValue);
    }
    public class SettingsService : ISettingsService
    {
        Configuration settingsConfiguration;

        public SettingsService()
        {
            settingsConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public string GetSettingWithDefault(string key, string defaultValue)
        {
            var result = settingsConfiguration.AppSettings.Settings[key];

            if (result != null)
            {
                return result.Value;
            }
            else
            {
                settingsConfiguration.AppSettings.Settings.Add(key, defaultValue);
                settingsConfiguration.Save();
            }

            return defaultValue;
        }

    }
}
