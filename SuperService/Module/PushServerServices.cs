using ClientModel3.MD;

namespace Test.Module
{
    internal static class PushServerServices
    {
        public static void Init()
        {
            Utils.TraceMessage($"Push Initialized: {PushNotification.IsInitialized}");
            if (PushNotification.IsInitialized) return;
            if (!string.IsNullOrEmpty(Settings.User) && !string.IsNullOrEmpty(Settings.Password) &&
                !string.IsNullOrEmpty(Settings.PushServer) && !string.IsNullOrEmpty(Settings.UserId))
            {
                PushNotification.InitializePushService(Settings.PushServer, Settings.UserId, Settings.Password);
            }
        }
    }
}