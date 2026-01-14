using ECommerceApp.Models;
using System.Collections.Generic;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Interface for store settings operations
    /// </summary>
    public interface IStoreSettingsService
    {
        /// <summary>
        /// Gets all store settings
        /// </summary>
        List<StoreSetting> GetAllSettings();

        /// <summary>
        /// Gets a setting by key
        /// </summary>
        StoreSetting GetSetting(string key);

        /// <summary>
        /// Gets a setting value by key
        /// </summary>
        string GetSettingValue(string key, string defaultValue = null);

        /// <summary>
        /// Gets a setting as boolean
        /// </summary>
        bool GetBooleanSetting(string key, bool defaultValue = false);

        /// <summary>
        /// Gets a setting as integer
        /// </summary>
        int GetIntegerSetting(string key, int defaultValue = 0);

        /// <summary>
        /// Gets a setting as decimal
        /// </summary>
        decimal GetDecimalSetting(string key, decimal defaultValue = 0);

        /// <summary>
        /// Updates a setting value
        /// </summary>
        bool UpdateSetting(string key, string value);

        /// <summary>
        /// Creates a new setting
        /// </summary>
        bool CreateSetting(StoreSetting setting);

        /// <summary>
        /// Deletes a setting
        /// </summary>
        bool DeleteSetting(string key);

        /// <summary>
        /// Gets settings by type
        /// </summary>
        List<StoreSetting> GetSettingsByType(string settingType);
    }
}
