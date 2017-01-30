using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Controls;
using BitMobile.DbEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Catalog;
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
        private bool _isReadOnly;

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
                var total = (_usedCalculateService ? (double)_sums["SumServices"] : 0D) +
                            (_usedCalculateMaterials ? (double)_sums["SumMaterials"] : 0D);

                totalSum = $"{total:N2}";
            }

            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("coc"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                ArrowActive = false
            };

            _topInfoComponent.CommentLayout.AddChild(new TextView($"{Translator.Translate("total")}"));
            _topInfoTotalTextView = new TextView($"{totalSum} {Translator.Translate("currency")}")
            {
                CssClass = "TotalPriceTV"
            };
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
            _isReadOnly = (bool)Variables[Parameters.IdIsReadonly];
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
            if (_isReadOnly) return;

            var eventStatus = (string)_currentEventDbRecordset["statusName"];
            if (eventStatus.Equals(EventStatus.Agreed, StringComparison.OrdinalIgnoreCase) 
                || eventStatus.Equals(EventStatus.Accepted, StringComparison.OrdinalIgnoreCase))
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
            if (_isReadOnly) return;

            var eventStatus = (string)_currentEventDbRecordset["statusName"];
            if (eventStatus.Equals(EventStatus.Agreed, StringComparison.OrdinalIgnoreCase)
                || eventStatus.Equals(EventStatus.Accepted, StringComparison.OrdinalIgnoreCase))
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
            if (_isReadOnly) return;

            var eventStatus = (string)_currentEventDbRecordset["statusName"];
            var vl = (VerticalLayout)sender;

            if (eventStatus.Equals(EventStatus.Agreed, StringComparison.OrdinalIgnoreCase)
                || eventStatus.Equals(EventStatus.Accepted, StringComparison.OrdinalIgnoreCase))
            {
                Dialog.Ask(Translator.Translate("start_event"), (innerSender, args) =>
                {
                    if (args.Result != Dialog.Result.Yes) return;
                    ChangeEventStatus();

                    var dictionary = new Dictionary<string, object>
                    {
                        {Parameters.IdBehaviour, BehaviourEditServicesOrMaterialsScreen.UpdateDB},
                        {Parameters.IdLineId, vl.Id},
                        {Parameters.IsEdit, true}
                    };

                    Navigation.Move("EditServicesOrMaterialsScreen", dictionary);
                });
            }
            else
            {
                var dictionary = new Dictionary<string, object>
                {
                    {Parameters.IdBehaviour, BehaviourEditServicesOrMaterialsScreen.UpdateDB},
                    {Parameters.IdLineId, vl.Id},
                    {Parameters.IsEdit, true}
                };

                Navigation.Move("EditServicesOrMaterialsScreen", dictionary);
            }
        }

        internal void ApplicatioMaterials_OnClick(object sender, EventArgs e)
        {
            var dict = new Dictionary<string,object>
            {
                {Parameters.IdCurrentEventId, _currentEventId}
            };
            Navigation.Move(nameof(CheckInfoScreen), dict);
        }

        internal void OpenDeleteButton_OnClick(object sender, EventArgs e)
        {
            var vl = (VerticalLayout)sender;
            var hl = (HorizontalLayout)vl.Parent;
            var shl = (SwipeHorizontalLayout)hl.Parent;
            ++shl.Index;
        }

        internal void DeleteButton_OnClick(object sender, EventArgs e)
        {
            if (_isReadOnly) return;

            var vl = (HorizontalLayout)sender;
            var deleted = CheckAndMaybeDelete(vl.Id);
            if (deleted)
            {
                var shl = (SwipeHorizontalLayout)vl.Parent;
                var outerVl = (VerticalLayout)shl.Parent;
                outerVl.CssClass = "NoHeight";
                outerVl.Refresh();
            }
            else
            {
                var shl = (SwipeHorizontalLayout)vl.Parent;
                var hl = (HorizontalLayout)shl.Controls[0];
                var priceContainer = (VerticalLayout)hl.Controls[1];
                var priceTv = (TextView)priceContainer.Controls[1];
                var sm = (Event_ServicesMaterials)DBHelper.LoadEntity(vl.Id);
                var sku = (RIM)sm.SKU.GetObject();
                priceTv.Text =
                    $"{sm.AmountFact} x {sm.Price} {Translator.Translate("currency")} {(string.IsNullOrEmpty(sku.Unit) ? "" : $"/ {sku.Unit}")}";
                shl.Index = 0;
            }

            var sums = GetSums();
            _totalSumForServices.Text = GetFormatStringForServiceSums();
            _totalSumForMaterials.Text = GetFormatStringForMaterialSums();
            _topInfoTotalTextView.Text = $"{sums["Sum"]:N2} {Translator.Translate("currency")}";
        }

        private bool CheckAndMaybeDelete(string id)
        {
            var sm = (Event_ServicesMaterials)DBHelper.LoadEntity(id);
            if (sm.AmountPlan == 0)
            {
                DBHelper.DeleteByRef(sm.Id, false);
                return true;
            }
            sm.AmountFact = 0;
            sm.SumFact = 0;
            sm.Save(false);
            return false;
        }

        internal string GetFormatStringForServiceSums()
        {
            var totalSum = $"{Convert.ToDouble(_sums["SumServices"]):N2}";
            return
                $"\u2022 {(_usedCalculateService ? totalSum : Parameters.EmptyPriceDescription)} {Translator.Translate("currency")}";
        }

        internal string GetFormatStringForMaterialSums()
        {
            var totalSum = $"{Convert.ToDouble(_sums["SumMaterials"]):N2}";
            return
                $"\u2022 {(_usedCalculateMaterials ? totalSum : Parameters.EmptyPriceDescription)} {Translator.Translate("currency")}";
        }

        internal DbRecordset GetSums()
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }
            var wasStarted = (bool)Variables[Parameters.IdWasEventStarted];
            _sums = DBHelper.GetCocSumsByEventId((string)eventId, !wasStarted);

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

        internal string CreatePriceString(DbRecordset priceRecordset, string serviceString)
        {
            var wasEventStarted = (bool)Variables[Parameters.IdWasEventStarted];
            var isService = serviceString == "service";
            var showPrice = isService ? Settings.ShowServicePrice : Settings.ShowMaterialPrice;
            var amount = (decimal)priceRecordset[wasEventStarted ? "AmountFact" : "AmountPlan"];
            var price = showPrice ? $"{priceRecordset["Price"]:N2}" : Parameters.EmptyPriceDescription;
            var unit = (string)priceRecordset["Unit"];
            return
                $"{amount} x {price} {Translator.Translate("currency")} {(string.IsNullOrEmpty(unit) ? "" : $"/ {unit}")}";
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
            var result = DBHelper.GetCoordinate(TimeRangeCoordinate.DefaultTimeRange);
            var latitude = Converter.ToDouble(result["Latitude"]);
            var longitude = Converter.ToDouble(result["Longitude"]);
            var @event = (Event)DBHelper.LoadEntity(_currentEventId);
            @event.ActualStartDate = DateTime.Now;
            @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.InWork);
            @event.Latitude = Converter.ToDecimal(latitude);
            @event.Longitude = Converter.ToDecimal(longitude);
            DBHelper.SaveEntity(@event);
            Variables[Parameters.IdWasEventStarted] = true;
            _currentEventDbRecordset = DBHelper.GetEventByID(_currentEventId);
            var rimList = DBHelper.GetServicesAndMaterialsByEventId(_currentEventId);
            var rimArrayList = new ArrayList();
            while (rimList.Next())
            {
                var rim = (Event_ServicesMaterials)((DbRef)rimList["Id"]).GetObject();
                rim.AmountFact = rim.AmountPlan;
                rim.SumFact = rim.SumPlan;
                rimArrayList.Add(rim);
            }
            DBHelper.SaveEntities(rimArrayList, false);
        }

        internal bool ShowCheckInfoScreen() => true;
    }
}