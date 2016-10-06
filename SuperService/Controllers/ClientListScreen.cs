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
            GpsTracking.Start();
        }

        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Events_OnClick(sender, eventArgs);
            DConsole.WriteLine("Clients Events");
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.TendersListScreen_OnClick(sender, eventArgs);
            DConsole.WriteLine("Clients Bag");
        }

        internal void TabBarThirdButton_OnClick(object sender, EventArgs eventArgs)
        {
            //_tabBarComponent.Clients_OnClick(sender, eventArgs);
            DConsole.WriteLine("Clients Clients");
        }

        internal void TabBarFourthButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Settings_OnClick(sender, eventArgs);
            DConsole.WriteLine("Clients Settings");
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
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