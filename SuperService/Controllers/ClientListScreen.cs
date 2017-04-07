using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using Test.Components;

namespace Test
{
    public class ClientListScreen : Screen
    {
        private TabBarComponent _tabBarComponent;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            base.OnLoading();
            DConsole.WriteLine("ClientListScreen init");

            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("clients"),
                ArrowVisible = false
            };
            _tabBarComponent = new TabBarComponent(this);
        }

        public override void OnShow()
        {
            base.OnShow();
            GpsTracking.StartAsync();
            Dialog.HideProgressDialog();
        }

        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            Dialog.ShowProgressDialog(Translator.Translate("loading_message"), true);
            _tabBarComponent.Events_OnClick(sender, eventArgs);
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs) 
        {
            //_tabBarComponent.Clients_OnClick(sender, eventArgs);
        }

        internal void TabBarThirdButton_OnClick(object sender, EventArgs eventArgs)
        => _tabBarComponent.FrSettings_OnClick(sender, eventArgs);

        internal void TabBarFourthButton_OnClick(object sender, EventArgs eventArgs) 
            => _tabBarComponent.Settings_OnClick(sender, eventArgs);

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_LeftButton_OnPressDown(object sender, EventArgs e)
        {
        }

        internal void TopInfo_RightButton_OnPressDown(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
            _topInfoComponent.Arrow_OnClick(sender, eventArgs);
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void ClientLayout_OnClick(object sender, EventArgs eventArgs)
        {
            DConsole.WriteLine("ClientLayout_OnClick " + ((VerticalLayout)sender).Id);
            // TODO: Передача Id конкретной таски
            BusinessProcess.GlobalVariables[Parameters.IdClientId] = ((VerticalLayout)sender).Id;
            Navigation.Move("ClientScreen");
        }

        internal IEnumerable GetClients()
        {
            DConsole.WriteLine("получение клиентов");
            var result = DBHelper.GetClients();
            DConsole.WriteLine("Получили клиентов");

            //var result2 = DBHelper.GetClients();
            // var dbEx = result2.Unload();
            //DConsole.WriteLine("in result " + dbEx.Count());

            return result;
        }
    }
}