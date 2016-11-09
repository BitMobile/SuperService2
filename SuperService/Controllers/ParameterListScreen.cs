using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using Test.Components;

namespace Test
{
    public class ParameterListScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowActive = false,
                ArrowVisible = false,
                Header = "Профили",
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") }
            };
            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
            => Navigation.Back();

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void ParametersButton_OnClick(object sender, EventArgs e)
        {
            BusinessProcess.GlobalVariables[Parameters.IdProfileId] = ((Button)sender).Id;
            Navigation.Move(nameof(ClientParametersScreen));
        }

        internal string GetResourceImage(object tag)
            => ResourceManager.GetImage($"{tag}");

        public DbRecordset GetGroupParameters()
            => DBHelper.GetClientProfile();

        public static string GetNullReference()
            => "@ref[Catalog_Profile]:00000000-0000-0000-0000-000000000000";
    }
}