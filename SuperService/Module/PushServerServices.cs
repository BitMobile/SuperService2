using System;
using ClientModel3.MD;

namespace Test.Module
{
    internal class PushServerServices
    {
        public static void Init()
        {
            var userId = Settings.UserDetailedInfo.Id.Guid;
            Utils.TraceMessage($"Push Initialized: {PushNotification.IsInitialized}");
            if (PushNotification.IsInitialized) return;
            Utils.TraceMessage($"Сервер:{Settings.PushServer} Юзер:{Settings.UserDetailedInfo.Id.Guid} Пароль:{Settings.Password}");
            if (!string.IsNullOrEmpty(Settings.User) && !string.IsNullOrEmpty(Settings.Password) &&
                !string.IsNullOrEmpty(Settings.PushServer) && (userId != Guid.Empty))
            {
                PushNotification.InitializePushService(Settings.PushServer, userId.ToString(), Settings.Password);
            }
        }
    }
}