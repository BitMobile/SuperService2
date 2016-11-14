using ClientModel3.MD;
using System;

namespace Test
{
    internal static class PushService
    {
        public static void Init()
        {
            var userId = Settings.UserDetailedInfo.Id.Guid;
            Utils.TraceMessage($"Push Initialized: {PushNotification.IsInitialized}");
            if (PushNotification.IsInitialized) return;
            Utils.TraceMessage($"Сервер:{Settings.SolutionUrl} Юзер:{Settings.UserDetailedInfo.Id.Guid} Пароль:{Settings.Password}");
            if (!string.IsNullOrEmpty(Settings.User) && !string.IsNullOrEmpty(Settings.Password) &&
                !string.IsNullOrEmpty(Settings.SolutionUrl) && (userId != Guid.Empty))
            {
                PushNotification.InitializePushService(Settings.SolutionUrl, userId.ToString(), Settings.Password);
            }
        }

        public static void Unregister()
        {
            var userId = Settings.UserDetailedInfo.Id.Guid;
            if (!string.IsNullOrEmpty(Settings.User) && !string.IsNullOrEmpty(Settings.Password) &&
               !string.IsNullOrEmpty(Settings.SolutionUrl) && (userId != Guid.Empty))
            {
                Utils.TraceMessage($"In Unregister");
                PushNotification.Unregister(Settings.SolutionUrl, userId.ToString(), Settings.Password);
            }
        }
    }
}