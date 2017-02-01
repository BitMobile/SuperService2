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
        private List<string> _fiscalList;
        private bool _readonly;
        private TopInfoComponent _topInfoComponent;
        private bool _wasStarted;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("cashbox_check"),
                LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")},
                RightButtonControl = _fiscalList.Count == 0
                    ? new Image {Source = ResourceManager.GetImage("print_icon")}
                    : new Image {Source = ResourceManager.GetImage("print_icon_disabel")},
                ArrowVisible = false
            };

            _topInfoComponent.ActivateBackButton();
            _eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);

            _readonly = (bool) Variables.GetValueOrDefault(Parameters.IdIsReadonly, false);
            _wasStarted = (bool) Variables.GetValueOrDefault(Parameters.IdWasEventStarted, true);
        }

        public override void OnShow()
        {
        }


        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
            => Navigation.Back();

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
            if (_fiscalList.Count == 0)
                Navigation.Move(nameof(PrintCheckScreen), new Dictionary<string, object>
                {
                    {Parameters.IdCurrentEventId, _eventId},
                    {Parameters.IdIsReadonly, _readonly},
                    {Parameters.IdWasEventStarted, _wasStarted}
                });
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        internal string GetResourceImage(string tag) => ResourceManager.GetImage(tag);

        internal string GetNameVAT(string vatEnum)
        {
            var strStart = Translator.Translate("VAT");
            switch (vatEnum)
            {
                case "Percent18":
                    return strStart + " 18%";
                case "Percent0":
                    return strStart + " 0%";
                case "PercentWithoOut":
                    return Translator.Translate("percent_witho_out");
                case "Percent10":
                    return strStart + " 10%";

                default:
                    return "";
            }
        }

        internal string GetSumCheck()
        {
            var totalSum =
                DBHelper.GetCheckSKUSum((string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty));
            var decimalSum = Converter.ToDecimal(totalSum);

            return $"{decimalSum:N}";
        }

        internal DbRecordset GetFiscalProp()
        {
            _fiscalList = new List<string>();
            var eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
            var fiscalRecordSet = DBHelper.GetFiscalEvent(eventId);
            while (fiscalRecordSet.Next())
            {
                _fiscalList.Add(fiscalRecordSet["CheckNumber"].ToString());
                _fiscalList.Add(fiscalRecordSet["Date"].ToString());
                _fiscalList.Add(fiscalRecordSet["NumberFtpr"].ToString());
                _fiscalList.Add(fiscalRecordSet["ShiftNumber"].ToString());
            }
            return fiscalRecordSet;
        }

        internal string GetCheckNumber() => _fiscalList[0];
        internal string GetDate() => _fiscalList[1];
        internal string GetNumberFtpr() => _fiscalList[2];
        internal string GetShiftNumber() => _fiscalList[3];

        internal bool CheckFiscalEvent() => _fiscalList.Count != 0;

        internal string ConvertToDec(object price)
        {
            return $"{Converter.ToDecimal(price):N}";
        }


        internal DbRecordset GetRIMList()
        {
            var eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
            return DBHelper.GetCheckSKU(eventId);
        }
    }
}