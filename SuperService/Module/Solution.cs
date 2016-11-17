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
        }

        public override void OnRestore()
        {
        }

        public override void OnPushMessage(string message)
        {
            Utils.TraceMessage($"{Settings.EnablePush}");
            if (Settings.EnablePush)
                InvokeOnMainThread(() => LocalNotification.Notify(Translator.Translate("notification"), message));

            DBHelper.SyncAsync();
        }
    }
}