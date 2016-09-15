using BitMobile.Application;
using BitMobile.ClientModel3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using XmlDocument = BitMobile.ClientModel3.XmlDocument;

namespace Test
{
    public static class Settings
    {
        private static Dictionary<string, object> _settings;
        private static bool _initialized;
        private static string _userId;

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

        public static string ImageServer { get; set; }

        public static string Host { get; set; }

        public static string UserId
        {
            get { return _userId ?? DBHelper.GetUserId(); }
            set { _userId = value; }
        }

        public static string AuthUrl { get; set; }

        public static string GPSSyncUrl { get; set; }
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
            XmlNode serverNode;
            XmlNode solutionPathNode;

            Stream stream = Stream.Null;
            try
            {
                try
                {
                    stream = Application.GetResourceStream("customSettings.xml");
                }
                catch
                {
                    stream = Application.GetResourceStream("settings.xml");
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(stream);
                serverNode = xmlDocument.SelectSingleNode("/configuration/server/host");
                DConsole.WriteLine("Настройки из XML");
                DConsole.WriteLine($"{serverNode?.Name}:{serverNode?.Attributes?["url"]?.Value}");
                solutionPathNode = xmlDocument.SelectSingleNode("/configuration/server/solutionPath");
                DConsole.WriteLine($"{solutionPathNode?.Name}:{solutionPathNode?.Attributes?["url"]?.Value}");
            }
            finally
            {
                stream?.Close();
            }

            Host = serverNode?.Attributes?["url"]?.Value ?? "https://sstest.superagent.ru";
            var server = Host + (solutionPathNode?.Attributes?["url"]?.Value ?? "/bitmobile3/superservice3demo");

            Server = server + "/device";
            ImageServer = server + "/";
            AuthUrl = Server + @"/GetUserId";
            GPSSyncUrl = server;

            DConsole.WriteLine($"Host = {Host}");
            DConsole.WriteLine($"Server = {Server}");

            _initialized = true;

            GpsTrackingInit();
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
            DConsole.WriteLine($"{nameof(UserId)}: {UserId}");
            DConsole.WriteLine($"GPSTracking.{nameof(GpsTracking.IsBestAccuracy)}: {GpsTracking.IsBestAccuracy}");
            DConsole.WriteLine($"GPSTracking.{nameof(GpsTracking.MinInterval)}: {GpsTracking.MinInterval}");
            DConsole.WriteLine($"GPSTracking.{nameof(GpsTracking.DistanceFilter)}: {GpsTracking.DistanceFilter}");
            DConsole.WriteLine($"GPSTracking.{nameof(GpsTracking.SendInterval)}: {GpsTracking.SendInterval}");
            DConsole.WriteLine($"GPSTracking.{nameof(GpsTracking.SendUrl)}: {GpsTracking.SendUrl}");
            DConsole.WriteLine($"GPSTracking.{nameof(GpsTracking.UserId)}: {GpsTracking.UserId}");
            DConsole.WriteLine($"{Parameters.Splitter}{Environment.NewLine}");
        }

#endif

        private static void GpsTrackingInit()
        {
            GpsTracking.UserId = UserId;
            GpsTracking.SendUrl = GPSSyncUrl;
            GpsTracking.IsBestAccuracy = GetLogicValue(nameof(GpsTracking.IsBestAccuracy), DefaultGpsTrackingParameters.IsBestAccuracy);
            GpsTracking.MinInterval = SetGpsTrackingParameter(nameof(GpsTracking.MinInterval),
                MinGpsTrackingParameters.MinInterval, DefaultGpsTrackingParameters.MinInterval);
            GpsTracking.MinDistance = SetGpsTrackingParameter(nameof(GpsTracking.MinDistance),
                MinGpsTrackingParameters.MinDistance, DefaultGpsTrackingParameters.MinDistance);
            GpsTracking.DistanceFilter = SetGpsTrackingParameter(nameof(GpsTracking.DistanceFilter),
                MinGpsTrackingParameters.DistanceFilter, DefaultGpsTrackingParameters.DistanceFilter);
            GpsTracking.SendInterval = SetGpsTrackingParameter(nameof(GpsTracking.SendInterval),
                MinGpsTrackingParameters.SendInterval, DefaultGpsTrackingParameters.SendInterval);
        }

        private static int SetGpsTrackingParameter(string parameterName, int minValue, int defaultValue = 0)
        {
            var value = GetNumericValue(parameterName, defaultValue);

            return value >= minValue ? value : minValue;
        }
    }

    public static class DefaultGpsTrackingParameters
    {
        public const bool IsBestAccuracy = true;
        public const int MinInterval = 5;
        public const int MinDistance = 10;
        public const int DistanceFilter = 5;
        public const int SendInterval = int.MaxValue;
    }

    public static class MinGpsTrackingParameters
    {
        public const int MinInterval = 10;
        public const int MinDistance = 8;
        public const int DistanceFilter = 4;
        public const int SendInterval = 15;
    }

    public static class TimeRangeCoordinate
    {
        public const uint DefaultTimeRange = 30U;
    }
}