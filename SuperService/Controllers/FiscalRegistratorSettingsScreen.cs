using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Globalization;
using BitMobile.Common.Application;
using Test.Components;

namespace Test
{
    public class FiscalRegistratorSettingsScreen : Screen
    {
        private TabBarComponent _tabBarComponent;
        private bool _readonlyForIos;
        public override void OnLoading()
        {
            _readonlyForIos = Application.TargetPlatform == TargetPlatform.iOS 
                || Application.TargetPlatform == TargetPlatform.Other;

            _tabBarComponent = new TabBarComponent(this);
        }

        public override void OnShow()
        {
            GpsTracking.Start();
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

    }
}