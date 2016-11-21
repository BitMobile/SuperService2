using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using Test.Components;

namespace Test
{
    public class DeliveryDateDescriptionScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;
        private TextView _textView;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowActive = false,
                ArrowVisible = false,
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                Header = Translator.Translate("delivery_date")
            };

            _topInfoComponent.ActivateBackButton();
            _textView = (TextView)GetControl("50a4f5b5286d4f50890bf61ea4292c16", true);
            _textView.Text = $"{Variables[Parameters.DeliveryDateDescription]}";
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
    }
}