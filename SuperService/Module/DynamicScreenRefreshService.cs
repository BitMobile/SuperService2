using BitMobile.ClientModel3;
using BitMobile.Common.Controls;
using System;
using System.Linq;

namespace Test
{
    public static class DynamicScreenRefreshService
    {
        private static string[] _refreshingScreen;

        public static void Init()
        {
            _refreshingScreen = new[]
            {
                nameof(EventListScreen),
                nameof(BagListScreen),
                nameof(ClientListScreen),
                //nameof(COCScreen),
                //nameof(RIMListScreen),
                //nameof(TaskListScreen)
            };
        }

        public static void RefreshScreen()
        {
            foreach (var s in _refreshingScreen)
            {
                if (string.Compare(s, Navigation.CurrentScreenInfo.Name, StringComparison.OrdinalIgnoreCase) != 0)
                    continue;
                Application.InvokeOnMainThread(() => Navigation.ModalMove(s, animation: ShowAnimationType.Refresh));
                break; ;
            }
        }
    }
}