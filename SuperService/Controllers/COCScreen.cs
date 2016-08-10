using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Controls;
using BitMobile.DbEngine;
using System;
using System.Collections.Generic;
using Test.Components;
using Test.Document;
using Test.Enum;
using DbRecordset = BitMobile.ClientModel3.DbRecordset;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace Test
{
    public class COCScreen : Screen
    {
        private DbRecordset _currentEventDbRecordset;
        private string _currentEventId;

        private bool _fieldsAreInitialized;
        private DbRecordset _sums;
        private TopInfoComponent _topInfoComponent;
        private TextView _topInfoTotalTextView;
        private TextView _totalSumForMaterials;
        private TextView _totalSumForServices;
        private bool _usedCalculateMaterials;
        private bool _usedCalculateService;

        public override void OnLoading()
        {
            InitClassFields();
            string totalSum;

            if (!_usedCalculateMaterials && !_usedCalculateService)
            {
                totalSum = Parameters.EmptyPriceDescription;
            }
            else
            {
                totalSum =
                    $"{Math.Round((_usedCalculateService ? (double)_sums["SumServices"] : 0) + (_usedCalculateMaterials ? (double)_sums["SumMaterials"] : 0), 2)}";
            }

            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("coc"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                ArrowActive = false
            };

            _topInfoComponent.CommentLayout.AddChild(new TextView($"{Translator.Translate("total")}"));
            _topInfoTotalTextView = new TextView($"{totalSum} {Translator.Translate("currency")}") { CssClass = "TotalPriceTV" };
            _topInfoComponent.CommentLayout.AddChild(_topInfoTotalTextView);
            _totalSumForServices = (TextView)GetControl("RightInfoServicesTV", true);
            _totalSumForMaterials = (TextView)GetControl("RightInfoMaterialsTV", true);

            _topInfoComponent.ActivateBackButton();
            DConsole.WriteLine($"{Variables[Parameters.IdIsReadonly]}");
        }

        public int InitClassFields()
        {
            if (_fieldsAreInitialized)
            {
                return 0;
            }

            _currentEventId = (string)Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
            _usedCalculateService = Settings.ShowServicePrice;
            _usedCalculateMaterials = Settings.ShowMaterialPrice;

            GetSums();

            _fieldsAreInitialized = true;

            _currentEventDbRecordset = DBHelper.GetEventByID(_currentEventId);

            return 0;
        }

        public override void OnShow()
        {
            GPS.StopTracking();
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal void AddService_OnClick(object sender, EventArgs e)
        {
            var eventStatus = (string)_currentEventDbRecordset["statusName"];

            if (eventStatus.Equals(EventStatus.Appointed))
            {
                Dialog.Ask(Translator.Translate("start_event"), (innerSender, args) =>
                {
                    if (args.Result != Dialog.Result.Yes) return;
                    ChangeEventStatus();

                    AddServiceArgument();
                });
            }
            else
            {
                AddServiceArgument();
            }
        }

        private void AddServiceArgument()
        {
            var dictionary = new Dictionary<string, object>
            {
                {Parameters.IdIsService, true},
                {Parameters.IdCurrentEventId, _currentEventId}
            };
            Navigation.Move("RIMListScreen", dictionary);
        }

        internal void AddMaterial_OnClick(object sender, EventArgs e)
        {
            var eventStatus = (string)_currentEventDbRecordset["statusName"];

            if (eventStatus.Equals(EventStatus.Appointed))
            {
                Dialog.Ask(Translator.Translate("start_event"), (innerSender, args) =>
                {
                    if (args.Result != Dialog.Result.Yes) return;
                    ChangeEventStatus();
                    AddMaterialArgument();
                });
            }
            else
            {
                AddMaterialArgument();
            }
        }

        private void AddMaterialArgument()
        {
            var dictionary = new Dictionary<string, object>
            {
                {Parameters.IdIsService, false},
                {Parameters.IdCurrentEventId, _currentEventId}
            };
            Navigation.Move("RIMListScreen", dictionary);
        }

        internal void EditServicesOrMaterials_OnClick(object sender, EventArgs e)
        {
            if ((bool)Variables.GetValueOrDefault(Parameters.IdIsReadonly, true)) return;
            var vl = (VerticalLayout)sender;
            var dictionary = new Dictionary<string, object>
            {
                {Parameters.IdBehaviour, BehaviourEditServicesOrMaterialsScreen.UpdateDB},
                {Parameters.IdLineId, vl.Id},
                {Parameters.IsEdit, true }
            };

            Navigation.Move("EditServicesOrMaterialsScreen", dictionary);
        }

        internal void ApplicatioMaterials_OnClick(object sender, EventArgs e)
        {
            Navigation.Move("MaterialsRequestScreen");
        }

        internal void OpenDeleteButton_OnClick(object sender, EventArgs e)
        {
            //TODO: Обходной путь получения парента. Внимание!!!!! .
            var vl = (VerticalLayout)sender;
            var hl = (IHorizontalLayout3)vl.Parent;
            var shl = (ISwipeHorizontalLayout3)hl.Parent;
            ++shl.Index;
        }

        internal void DeleteButton_OnClick(object sender, EventArgs e)
        {
            //TODO: Обходной путь получения парента. Внимание!!!!!.
            var vl = (HorizontalLayout)sender;
            DBHelper.DeleteByRef(DbRef.FromString(vl.Id));
            var shl = (ISwipeHorizontalLayout3)vl.Parent;
            var outerVl = (IVerticalLayout3)shl.Parent;
            outerVl.CssClass = "NoHeight";
            var sums = GetSums();
            _totalSumForServices.Text = GetFormatStringForServiceSums();
            _totalSumForMaterials.Text = GetFormatStringForMaterialSums();
            _topInfoTotalTextView.Text = $"{Math.Round((double)sums["Sum"], 2)} {Translator.Translate("currency")}";
            shl.Refresh();
        }

        internal string GetFormatStringForServiceSums()
        {
            var totalSum = Convert.ToDouble(_sums["SumServices"]).ToString();
            return
                $"\u2022 {(_usedCalculateService ? totalSum : Parameters.EmptyPriceDescription)} {Translator.Translate("currency")}";
        }

        internal string GetFormatStringForMaterialSums()
        {
            var totalSum = Convert.ToDouble(_sums["SumMaterials"]).ToString();
            return
                $"\u2022 {(_usedCalculateMaterials ? totalSum : Parameters.EmptyPriceDescription)} {Translator.Translate("currency")}";
        }

        internal string GetServicePriceDescription(DbRecordset service)
        {
            return _usedCalculateService ? service["Price"].ToString() : Parameters.EmptyPriceDescription;
        }

        internal string GetMaterialPriceDescription(DbRecordset material)
        {
            return _usedCalculateMaterials ? material["Price"].ToString() : Parameters.EmptyPriceDescription;
        }

        internal DbRecordset GetSums()
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }
            DConsole.WriteLine("In to: " + nameof(GetSums));
            _sums = DBHelper.GetCocSumsByEventId((string)eventId);

            return _sums;
        }

        internal DbRecordset GetServices()
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }

            return DBHelper.GetServicesByEventId((string)eventId);
        }

        internal string Concat(float amountFact, string price, string unit)
        {
            return $"{amountFact} x {price} {Translator.Translate("currency")} " +
                   (string.IsNullOrEmpty(unit) ? "" : $"/ {unit}");
        }

        internal DbRecordset GetMaterials()
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }

            return DBHelper.GetMaterialsByEventId((string)eventId);
        }

        private void ChangeEventStatus()
        {
            var @event = (Event)DBHelper.LoadEntity(_currentEventId);
            @event.ActualStartDate = DateTime.Now;
            @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.InWork);
            DBHelper.SaveEntity(@event);
            _currentEventDbRecordset = DBHelper.GetEventByID(_currentEventId);
        }

        internal bool ShowNotEnoughMaterials() => Settings.BagEnabled;
    }
}