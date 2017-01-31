using System;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class CheckInfoScreen : Screen
    {
        private string _eventId;
        private bool _readOnly;
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
            _eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
        }

        public override void OnShow()
        {
        }


        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
            => Navigation.Back();

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
            => Navigation.Move(nameof(PrintCheckScreen), new Dictionary<string, object>
            {
                {Parameters.IdCurrentEventId, _eventId}
            });

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        internal string GetResourceImage(string tag) => ResourceManager.GetImage(tag);

        internal string GetNameVAT(string vatEnum)
        {
            switch (vatEnum)
            {
                case "Percent18":
                    return "18%";
                case "Percent0":
                    return "0%";
                case "PercentWithoOut":
                    return Translator.Translate("percent_witho_out");
                case "Percent10":
                    return "10%";

                default:
                    return "";
            }
        }

        internal string GetSumCheck()
        {
            var totalSum = DBHelper.GetCheckSKUSum((string)Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty));
            var decimalSum = Converter.ToDecimal(totalSum);

            return $"{decimalSum:N}";
        }

        internal DbRecordset GetRIMList()
        {
            var eventId = (string)Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
            var res = DBHelper.GetCheckSKU(eventId);
            return res;
        }
    }
}