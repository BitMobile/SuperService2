using BitMobile.Application.Log;
using BitMobile.ClientModel3;
using ClientModel3.MD;
using System;

namespace Test
{
    public class Solution : Application
    {
        public override void OnCreate()
        {
            base.OnCreate();
            Logger.SolutionType = "Grotem Service 2 mobile app";
            ReadLoggerSettings();
            DConsole.WriteLine("DB init...");
            DBHelper.Init();
            DConsole.WriteLine("Settings init...");
            Settings.Init();
            DConsole.WriteLine("Authorization init...");
            DynamicScreenRefreshService.Init();
            Authorization.Init();
            if (Authorization.FastAuthorization() && Settings.UserDetailedInfo!=null)
            {
#if DEBUG
                DConsole.WriteLine($"Логин и пароль были сохранены." +
                                   $"{Environment.NewLine}" +
                                   $"Login: {Settings.User} Password: {Settings.Password}{Environment.NewLine}");
#endif
                DConsole.WriteLine("Loading first screen...");
                Navigation.Move(nameof(EventListScreen));
            }
            else
            {
#if DEBUG
                if (Settings.UserDetailedInfo == null)
                {
                    Utils.TraceMessage("Произошло падение базы... извините");
                }
                DConsole.WriteLine($"Логин и пароль НЕ были сохранены." +
                                   $"{Environment.NewLine}" +
                                   $"Login: {Settings.User} Password: {Settings.Password} {Environment.NewLine}");
#endif
                DConsole.WriteLine("Loading first screen...");
                Navigation.Move(nameof(AuthScreen));
            }
        }

        private void ReadLoggerSettings()
        {
            var settings = BitMobile.Application.ApplicationContext.Current.Settings.CustomSettings;
            if (string.IsNullOrEmpty(Settings.SolutionName))
                if (settings.ContainsKey(Parameters.UserSolutionName))
                    Logger.SolutionName = Settings.SolutionName = settings[Parameters.UserSolutionName];
        }

        public override void OnBackground()
        {
            base.OnBackground();
            var result = GpsTracking.Stop();
#if DEBUG
            DConsole.WriteLine($"Свернули приложение. GpsTracking is stop: result = {result}");
#endif
        }

        public override void OnRestore()
        {
            base.OnRestore();
            ReadLoggerSettings();
            GpsTracking.StartAsync();
        }

        public override void OnPushMessage(string message, string additionalInfo)
        {
            base.OnPushMessage(message, additionalInfo);
            LocalNotification.Notify(Translator.Translate("notification"),
                Translator.Translate(message));
            DBHelper.SyncAsync(ResultEventHandler);
        }
        private static void ResultEventHandler(object sender, ResultEventArgs<bool> resultEventArgs)
        {
            if(!resultEventArgs.Result)
                DBHelper.isPartialSyncRequired = true;
        }
    }
}