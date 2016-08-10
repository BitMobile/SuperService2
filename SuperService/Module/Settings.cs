using BitMobile.Application;
using BitMobile.ClientModel3;
using System;
using System.Collections.Generic;

namespace Test
{
    public static class Settings
    {
        private static Dictionary<string, object> _settings;
        private static bool _initialized;

        //TODO: неочень хорошо так хранить пользователя и пароль.
        public static string User
        {
            get { return ApplicationContext.Current.Settings.UserName; }
            set
            {
                ApplicationContext.Current.Settings.UserName = value;
                ApplicationContext.Current.Settings.WriteSettings();
            }
        }

        public static string Password
        {
            get { return ApplicationContext.Current.Settings.Password; }
            set
            {
                ApplicationContext.Current.Settings.Password = value;
                ApplicationContext.Current.Settings.WriteSettings();
            }
        }

        // TODO: Хранить сервер
        public static string Server { get; set; }

        public static string Host { get; set; }

        public static string UserId { get; set; }

        public static string AuthUrl { get; set; }

        public static bool AllowGallery => GetLogicValue(Parameters.AllowGallery);
        public static int PictureSize => GetNumericValue(Parameters.PictureSize, 2500);
        public static bool EquipmentEnabled => GetLogicValue(Parameters.EquipmentEnabled);
        public static bool CheckListEnabled => GetLogicValue(Parameters.CheckListEnabled);
        public static bool COCEnabled => GetLogicValue(Parameters.COCEnabled);
        public static bool BagEnabled => GetLogicValue(Parameters.BagEnabled);
        public static bool ShowServicePrice => GetLogicValue(Parameters.ShowServicePrice);
        public static bool ShowMaterialPrice => GetLogicValue(Parameters.ShowMaterialPrice);

        public static void Init()
        {
            if (!_initialized)
                ApplicationContext.Current.Settings.ReadSettings();
            var settings = DBHelper.GetSettings();

            if (_settings == null)
            {
                _settings = new Dictionary<string, object>();
            }

            while (settings.Next())
            {
                var dictionary = new Dictionary<string, object>(2)
                {
                    {Parameters.LogicValue, settings[Parameters.LogicValue]},
                    {Parameters.NumericValue, settings[Parameters.NumericValue]}
                };

                _settings[(string)settings["Description"]] = dictionary;
            }

#if DEBUG
            DConsole.WriteLine(Parameters.Splitter);
            DConsole.WriteLine($"Настройки в БД.{Environment.NewLine}");
            foreach (var item in _settings)
            {
                var element = (Dictionary<string, object>)item.Value;
                DConsole.WriteLine($"Description: {item.Key} LogicValue: {(bool)element[Parameters.LogicValue]}" +
                                   $" NumericValue: {(int)element[Parameters.NumericValue]}");
            }
            DConsole.WriteLine($"{Parameters.Splitter}{Environment.NewLine}");
#endif

            Host = @"https://sstest.superagent.ru";
            Server = Host + @"/bitmobile3/superservice3test/device";
            AuthUrl = Server + @"/GetUserId";
            _initialized = true;

            //TODO: В релизе удалить. Это отлаточный вызов метода.
#if DEBUG
            CheckAllProperty();
#endif
        }

        private static bool GetLogicValue(string setupName, bool @default = false)
        {
            if (!_initialized)
                Init();

            object value;

            if (!_settings.TryGetValue(setupName, out value))
            {
#if DEBUG
                DConsole.WriteLine(Parameters.Splitter);
                DConsole.WriteLine($"Настрока с именем {setupName} НЕ найдена.");
                DConsole.WriteLine(Parameters.Splitter);
#endif
                return @default;
            }

            var dictionary = (Dictionary<string, object>)value;

            return (bool)dictionary.GetValueOrDefault(Parameters.LogicValue, @default);
        }

        private static int GetNumericValue(string setupName, int @default = 0)
        {
            if (!_initialized)
                Init();

            object value;

            if (!_settings.TryGetValue(setupName, out value))
            {
#if DEBUG
                DConsole.WriteLine(Parameters.Splitter);
                DConsole.WriteLine($"Настрока с именем {setupName} НЕ найдена.");
                DConsole.WriteLine(Parameters.Splitter);
#endif
                return @default;
            }

            var dictionary = (Dictionary<string, object>)value;

            return (int)dictionary.GetValueOrDefault(Parameters.NumericValue, @default);
        }

        //TODO: В релизе удалить. Это отладочный метод.
#if DEBUG

        private static void CheckAllProperty()
        {
            DConsole.WriteLine(Parameters.Splitter);
            DConsole.WriteLine($"Проверка параметров настройки на их получение из БД.{Environment.NewLine}");
            DConsole.WriteLine($"{nameof(AllowGallery)}: {AllowGallery}");
            DConsole.WriteLine($"{nameof(PictureSize)}: {PictureSize}");
            DConsole.WriteLine($"{nameof(EquipmentEnabled)}: {EquipmentEnabled}");
            DConsole.WriteLine($"{nameof(CheckListEnabled)}: {CheckListEnabled}");
            DConsole.WriteLine($"{nameof(COCEnabled)}: {COCEnabled}");
            DConsole.WriteLine($"{nameof(BagEnabled)}: {BagEnabled}");
            DConsole.WriteLine($"{nameof(ShowServicePrice)}: {ShowServicePrice}");
            DConsole.WriteLine($"{nameof(ShowMaterialPrice)}: {ShowMaterialPrice}");
            DConsole.WriteLine($"{Parameters.Splitter}{Environment.NewLine}");
        }

#endif
    }
}