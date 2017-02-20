using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Globalization;
using Test.Components;

namespace Test
{
    public class FiscalRegistratorSettingsScreen : Screen
    {
        private TabBarComponent _tabBarComponent;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("bag"),
                ArrowVisible = false
            };
            if (Settings.BagEnabled)
            {
                _topInfoComponent.LeftButtonControl = new Image
                {
                    Source = ResourceManager.GetImage("baglistscreen_busket")
                };
                _topInfoComponent.RightButtonControl = new Image
                {
                    Source = ResourceManager.GetImage("baglistscreen_plus")
                };
            }
            _tabBarComponent = new TabBarComponent(this);
        }

        public override void OnShow()
        {
            GpsTracking.Start();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            if (Settings.BagEnabled)
                Navigation.Move("RequestHistoryScreen");
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
            if (Settings.BagEnabled)
                Navigation.Move("CheckInfoScreen");
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal IEnumerable GetUserBag()
        {
            return Settings.BagEnabled ? DBHelper.GetUserBagByUserId(Settings.UserId) : DBHelper.GetAllMaterials();
        }

        internal string ConcatCountUnit(Single count, string unit)
        {
            return string.Concat(count.ToString(CultureInfo.CurrentCulture), unit);
        }

        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Events_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Events");
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs) 
            => _tabBarComponent.Clients_OnClick(sender, eventArgs);

        internal void TabBarThirdButton_OnClick(object sender, EventArgs eventArgs)
        {
            //_tabBarComponent.FrSettings_OnClick(sender, eventArgs);
        }

        internal void TabBarFourthButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Settings_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Settings");
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal bool ShowCount() => Settings.BagEnabled;
    }
}