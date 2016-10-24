using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using BitMobile.Common.Controls;
using Test.Components;

namespace Test
{
    public class ClientListScreen : Screen
    {
        private TabBarComponent _tabBarComponent;
        private TopInfoComponent _topInfoComponent;
        private bool _isAddTask;
        private static string findText;

        public override void OnLoading()
        {
            DConsole.WriteLine("ClientListScreen init");

            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("clients"),
                ArrowVisible = false
            };

            if (_isAddTask)
            {
                _topInfoComponent.LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") };
                _topInfoComponent.ActivateBackButton();
            }
            else
                _tabBarComponent = new TabBarComponent(this);
        }

        public override void OnShow()
        {
            GpsTracking.Start();
        }

        internal String GetFindText()
        {
            return findText;
        }

        internal void BtnSearch_Click(object sender, EventArgs eventArgs)
        {
            findText = ((EditText)GetControl("position", true)).Text;
            if (_isAddTask)
            {

                Navigation.ModalMove(nameof(ClientListScreen), new Dictionary<string, object>
                    { {Parameters.IsAsTask, _isAddTask} }, null, ShowAnimationType.Refresh);
            }
            else
            {
                Navigation.ModalMove(nameof(ClientListScreen), null, null, ShowAnimationType.Refresh);
            }
        }
        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            findText = null;
            _tabBarComponent?.Events_OnClick(sender, eventArgs);
            DConsole.WriteLine("Clients Events");
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            findText = null;
            _tabBarComponent?.TendersListScreen_OnClick(sender, eventArgs);
            DConsole.WriteLine("Clients Bag");
        }

        internal void TabBarThirdButton_OnClick(object sender, EventArgs eventArgs)
        {
            //_tabBarComponent.Clients_OnClick(sender, eventArgs);
            DConsole.WriteLine("Clients Clients");
        }

        internal void TabBarFourthButton_OnClick(object sender, EventArgs eventArgs)
        {
            findText = null;
            _tabBarComponent?.Settings_OnClick(sender, eventArgs);
            DConsole.WriteLine("Clients Settings");
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            findText = null;
            if (_isAddTask)
                Navigation.ModalMove(nameof(AddTaskScreen));
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
            findText = null;
            if (!_isAddTask)
            {
                DConsole.WriteLine("ClientLayout_OnClick " + ((VerticalLayout)sender).Id);
                BusinessProcess.GlobalVariables[Parameters.IdClientId] = ((VerticalLayout)sender).Id;
                Navigation.Move("ClientScreen");
            }
            else
                Navigation.ModalMove(nameof(AddTaskScreen),
                    new Dictionary<string, object>
                    { {Parameters.IdClientId, ((VerticalLayout) sender).Id} });
        }

        internal IEnumerable GetClients()
        {
            DConsole.WriteLine("получение клиентов");
            var result = DBHelper.GetClients(findText);
            DConsole.WriteLine("Получили клиентов");

            //var result2 = DBHelper.GetClients();
            // var dbEx = result2.Unload();
            //DConsole.WriteLine("in result " + dbEx.Count());

            return result;
        }

        internal bool IsNeedTabBar()
            => !(_isAddTask = (bool)Variables.GetValueOrDefault(Parameters.IsAsTask, false));
    }
}