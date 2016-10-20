using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMobile.ClientModel3;
using ClientModel3.MD;

namespace Test.Module
{
    static class PushServerServices
    {
        public static void Init()
        {
            if (PushNotification.IsInitialized) return;
            if (!string.IsNullOrEmpty(Settings.User) && !string.IsNullOrEmpty(Settings.Password) && !string.IsNullOrEmpty(Settings.PushServer) && !string.IsNullOrEmpty(Settings.UserId))
                {
                    PushNotification.InitializePushService(Settings.PushServer, Settings.UserId, Settings.Password);
                }
        }
    }
}
