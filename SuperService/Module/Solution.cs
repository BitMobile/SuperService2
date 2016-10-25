using BitMobile.ClientModel3;
using ClientModel3.MD;
using System;

namespace Test
{
    public class Solution : Application
    {
        public override void OnCreate()
        {
            DConsole.WriteLine("DB init...");
            DBHelper.Init();
            DConsole.WriteLine("Settings init...");
            Settings.Init();
            DConsole.WriteLine("Authorization init...");
            Authorization.Init();
            if (Authorization.FastAuthorization())
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
            var result = GpsTracking.Stop();
#if DEBUG
            DConsole.WriteLine($"Свернули приложение. GpsTracking is stop: result = {result}");
#endif
        }

        public override void OnRestore()
        {
            var result = GpsTracking.Start();
#if DEBUG
            DConsole.WriteLine($"Развернули приложение.GpsTracking is start: result = {result}");
#endif
        }

        public override void OnPushMessage(string message)
        {
            if (Settings.EnablePush)
                InvokeOnMainThread(() => LocalNotification.Notify(Translator.Translate("notification"), message));

            DBHelper.SyncAsync();
        }
    }
}