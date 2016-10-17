using BitMobile.ClientModel3.UI;
using System;
using Test.Components;

namespace Test
{
    public class WebViewScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowActive = false,
                ArrowVisible = false,
                Header = "Обозреватель",
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") }
            };
            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
            Utils.TraceMessage($"{Variables[Parameters.WebUri]}");
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal string GetResourceImage(object tag)
            => ResourceManager.GetImage($"{tag}");

        internal string GetUrl()
            => $"{Variables[Parameters.WebUri]}";
    }
}