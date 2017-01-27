using System;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class CheckInfoScreen : Screen
    {

        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("cashbox_check"),
                LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")},
                RightButtonControl = new Image {Source = ResourceManager.GetImage("print_icon")},
                ArrowVisible = false
            };

            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
        }


        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
            => Navigation.Back();

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        internal string GetResourceImage(string tag) => ResourceManager.GetImage(tag);
    }
}