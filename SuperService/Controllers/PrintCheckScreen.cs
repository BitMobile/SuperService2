using System;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class PrintCheckScreen : Screen
    {
        private string _eventId;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("registration_check"),
                LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")},
                ArrowVisible = false
            };

            _topInfoComponent.ActivateBackButton();
            _eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);

            Utils.TraceMessage($"Event Id {_eventId}");
        }

        internal string GetResourceImage(string tag) => ResourceManager.GetImage(tag);

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e) => Navigation.Back();

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        internal string GetFormatTotalSum()
        {
            var totalSum = DBHelper.GetCheckSKUSum(_eventId);
            var decimalSum = Converter.ToDecimal(totalSum);
            Utils.TraceMessage($"Total SUm {totalSum}");
            Utils.TraceMessage($"{nameof(decimalSum)}: {decimalSum:N} -> {decimalSum}");

            return $"{decimalSum:N}";
        }
    }
}