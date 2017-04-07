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