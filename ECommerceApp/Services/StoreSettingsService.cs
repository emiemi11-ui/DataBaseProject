using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for store settings operations
    /// Uses ECommerceEntities (DB First - EDMX generated context)
    /// </summary>
    public class StoreSettingsService : IStoreSettingsService, IDisposable
    {
        private readonly ECommerceEntities _context;
        private bool _disposed;

        // Common setting keys as constants
        public static class SettingKeys
        {
            public const string StoreName = "StoreName";
            public const string StoreEmail = "StoreEmail";
            public const string StorePhone = "StorePhone";
            public const string PrimaryColor = "PrimaryColor";
            public const string AccentColor = "AccentColor";
            public const string CurrencySymbol = "CurrencySymbol";
            public const string TaxRate = "TaxRate";
            public const string MinimumOrderAmount = "MinimumOrderAmount";
            public const string FreeShippingThreshold = "FreeShippingThreshold";
            public const string AllowGuestCheckout = "AllowGuestCheckout";
        }

        public StoreSettingsService()
        {
            _context = new ECommerceEntities();
        }

        public StoreSettingsService(ECommerceEntities context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all store settings
        /// </summary>
        public List<StoreSetting> GetAllSettings()
        {
            return _context.StoreSettings
                .OrderBy(s => s.SettingKey)
                .ToList();
        }

        /// <summary>
        /// Gets a setting by key
        /// </summary>
        public StoreSetting GetSetting(string key)
        {
            return _context.StoreSettings
                .FirstOrDefault(s => s.SettingKey == key);
        }

        /// <summary>
        /// Gets a setting value by key
        /// </summary>
        public string GetSettingValue(string key, string defaultValue = null)
        {
            var setting = GetSetting(key);
            return setting?.SettingValue ?? defaultValue;
        }

        /// <summary>
        /// Gets a setting as boolean
        /// </summary>
        public bool GetBooleanSetting(string key, bool defaultValue = false)
        {
            var value = GetSettingValue(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return value.ToLower() == "true" || value == "1";
        }

        /// <summary>
        /// Gets a setting as integer
        /// </summary>
        public int GetIntegerSetting(string key, int defaultValue = 0)
        {
            var value = GetSettingValue(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        /// <summary>
        /// Gets a setting as decimal
        /// </summary>
        public decimal GetDecimalSetting(string key, decimal defaultValue = 0)
        {
            var value = GetSettingValue(key);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return decimal.TryParse(value, out decimal result) ? result : defaultValue;
        }

        /// <summary>
        /// Updates a setting value
        /// </summary>
        public bool UpdateSetting(string key, string value)
        {
            try
            {
                var setting = GetSetting(key);
                if (setting == null)
                {
                    return false;
                }

                setting.SettingValue = value;
                setting.LastUpdated = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new setting
        /// </summary>
        public bool CreateSetting(StoreSetting setting)
        {
            try
            {
                // Check if setting already exists
                if (GetSetting(setting.SettingKey) != null)
                {
                    return false;
                }

                setting.LastUpdated = DateTime.Now;
                _context.StoreSettings.Add(setting);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        public bool DeleteSetting(string key)
        {
            try
            {
                var setting = GetSetting(key);
                if (setting == null)
                {
                    return false;
                }

                _context.StoreSettings.Remove(setting);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets settings by type
        /// </summary>
        public List<StoreSetting> GetSettingsByType(string settingType)
        {
            return _context.StoreSettings
                .Where(s => s.SettingType == settingType)
                .OrderBy(s => s.SettingKey)
                .ToList();
        }

        // =====================================================
        // Convenience Methods for Common Settings
        // =====================================================

        public string GetStoreName() => GetSettingValue(SettingKeys.StoreName, "E-Commerce Store");
        public string GetStoreEmail() => GetSettingValue(SettingKeys.StoreEmail, "");
        public string GetStorePhone() => GetSettingValue(SettingKeys.StorePhone, "");
        public string GetPrimaryColor() => GetSettingValue(SettingKeys.PrimaryColor, "#2196F3");
        public string GetAccentColor() => GetSettingValue(SettingKeys.AccentColor, "#FF5722");
        public string GetCurrencySymbol() => GetSettingValue(SettingKeys.CurrencySymbol, "RON");
        public decimal GetTaxRate() => GetDecimalSetting(SettingKeys.TaxRate, 19);
        public decimal GetMinimumOrderAmount() => GetDecimalSetting(SettingKeys.MinimumOrderAmount, 0);
        public decimal GetFreeShippingThreshold() => GetDecimalSetting(SettingKeys.FreeShippingThreshold, 200);
        public bool IsGuestCheckoutAllowed() => GetBooleanSetting(SettingKeys.AllowGuestCheckout, true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
