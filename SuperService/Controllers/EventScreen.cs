using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Catalog;
using Test.Components;
using Test.Document;
using Test.Enum;
using DbRecordset = BitMobile.ClientModel3.DbRecordset;

namespace Test
{
    public class EventScreen : Screen
    {
        private DbRecordset _currentEventRecordset;
        private bool _readonly;
        private DockLayout _rootLayout;
        private Button _startButton;
        private Button _DelegateButton;

        private VerticalLayout _startFinishButton;
        private Image _statusImage;
        private bool _taskCommentTextExpanded;
        private TextView _taskCommentTextView;
        private TextView _startDatePlanTextView;

        private TopInfoComponent _topInfoComponent;
        private Image _wrapUnwrapImage;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this);
            _topInfoComponent.ActivateBackButton();
            LoadControls();
            FillControls();

            IsEmptyDateTime((string)_currentEventRecordset["ActualStartDate"]);

            _startDatePlanTextView = (TextView)GetControl("StartTimeTextView", true);
        }

        private void FillControls()
        {
            _topInfoComponent.Header =
                ((string)_currentEventRecordset["clientDescription"]).CutForUIOutput(13, 2);
            _topInfoComponent.CommentLayout.AddChild(new TextView(
                ((string)_currentEventRecordset["clientAddress"]).CutForUIOutput(17, 2)));

            _topInfoComponent.LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") };
            _topInfoComponent.RightButtonControl = new Image { Source = ResourceManager.GetImage("topheading_info") };

            _taskCommentTextView = (TextView)GetControl("EventCommentTextView", true);
            _wrapUnwrapImage = (Image)GetControl("WrapUnwrapImage", true);

            var extraHorizontalLayout = new HorizontalLayout { CssClass = "ExtraHorizontalLayout" };
            var leftExtraLayout = new VerticalLayout { CssClass = "ExtraVerticalLayout" };
            var rightExtraLayout = new VerticalLayout { CssClass = "ExtraVerticalLayout" };

            _topInfoComponent.ExtraLayout.AddChild(extraHorizontalLayout);
            extraHorizontalLayout.AddChild(leftExtraLayout);
            extraHorizontalLayout.AddChild(rightExtraLayout);

            leftExtraLayout.AddChild(new Image
            {
                CssClass = "TopInfoSideImage",
                Source = ResourceManager.GetImage("topinfo_extra_map")
            });
            leftExtraLayout.AddChild(new TextView
            {
                Text = Translator.Translate("onmap"),
                CssClass = "TopInfoSideText"
            });
            rightExtraLayout.AddChild(new Image
            {
                CssClass = "TopInfoSideImage",
                Source = ResourceManager.GetImage("topinfo_extra_person")
            });
            var text = (string)_currentEventRecordset["ContactVisitingDescription"];
            if (string.IsNullOrEmpty(text))
                text = Translator.Translate("contact_not_present");
            else
                rightExtraLayout.OnClick += RightExtraLayoutOnOnClick;

            rightExtraLayout.AddChild(new TextView
            {
                Text = text.CutForUIOutput(12, 2),
                CssClass = "TopInfoSideText"
            });

            leftExtraLayout.OnClick += GoToMapScreen_OnClick;
        }

        private void RightExtraLayoutOnOnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Move(nameof(ContactScreen), new Dictionary<string, object>
            {
                [Parameters.Contact] = (Contacts)DBHelper.LoadEntity(_currentEventRecordset["contactId"].ToString())
            });
        }

        public override void OnShow()
        {
            if ((string)_currentEventRecordset["statusName"] == "Done")
            {
                Toast.MakeToast(Translator.Translate("task_finished_ro"));
                _readonly = true;
            }
            if ((string)_currentEventRecordset["statusName"] == "Cancel")
            {
                Toast.MakeToast(Translator.Translate("task_canceled_ro"));
                _readonly = true;
            }
        }

        private void LoadControls()
        {
            _rootLayout = (DockLayout)Variables["RootLayout"];
            _startFinishButton = (VerticalLayout)Variables.GetValueOrDefault("StartFinishButton");
            _startButton = (Button)Variables.GetValueOrDefault("StartButton");
            _statusImage = (Image)Variables.GetValueOrDefault("StatusImage");
            _DelegateButton = (Button)Variables.GetValueOrDefault("DelegateButton");
        }

        internal void ClientInfoButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Move("ClientScreen");
        }

        internal void RefuseButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Move("CancelEventScreen");
        }

        internal string FormatEventStartDatePlanTime(string eventStartDatePlan)
        {
            return DateTime.Parse(eventStartDatePlan).ToString("HH:mm");
        }

        internal void StartButton_OnClick(object sender, EventArgs eventArgs)
        {
            Dialog.Ask(Translator.Translate("areYouSure"), (o, args) =>
            {
                if (args.Result == Dialog.Result.Yes)
                {
                    ChangeLayoutToStartedEvent();
                }
            });
        }

        internal void WrapUnwrapButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_taskCommentTextExpanded)
            {
                _taskCommentTextView.CssClass = "SubComment";
                _wrapUnwrapImage.Source = ResourceManager.GetImage("longtext_expand");
                _taskCommentTextExpanded = false;
            }
            else
            {
                _taskCommentTextView.CssClass = "SubCommentExpanded";
                _wrapUnwrapImage.Source = ResourceManager.GetImage("longtext_close");
                _taskCommentTextExpanded = true;
            }
            _taskCommentTextView.Refresh();
            _rootLayout.Refresh();
        }

        private void ChangeLayoutToStartedEvent()
        {
            _startButton.CssClass = "NoHeight";
            _startButton.Visible = false;
            _startButton.Refresh();
            _DelegateButton.CssClass = "NoHeight";
            _DelegateButton.Visible = false;
            _DelegateButton.Refresh();
            _startFinishButton.CssClass = "FinishButton";
            _startFinishButton.Refresh();
            _statusImage.Source = GetStatusPicture((string)_currentEventRecordset["ImportanceName"], "InWork");
            _rootLayout.Refresh();
            Event_OnStart();
        }

        internal void StartFinishButton_OnClick(object sender, EventArgs eventArgs)
        {
            var result = DBHelper.GetTotalFinishedRequireQuestionByEventId(
                (string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);

            var isActiveEvent = !result.Next() || (long)result["count"] == 0;

            if (isActiveEvent)
            {
                Dialog.Alert(Translator.Translate("closetaskquestion"), (o, args) =>
                {
                    if (!CheckEventBeforeClosing() || args.Result != 0) return;
                    var @event =
                        (Event)
                            DBHelper.LoadEntity(
                                (string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
                    @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.Done);
                    @event.ActualEndDate = DateTime.Now;
                    DBHelper.SaveEntity(@event);
                    Navigation.Move("CloseEventScreen");
                }, null,
                    Translator.Translate("yes"), Translator.Translate("no"));
            }
            else
            {
                Dialog.Message(Translator.Translate("unfinished_business"));
            }
        }

        internal string GetStatusPicture(string importance, string status)
        {
            var pictureTag = @"eventlistscreen_";
            switch (importance)
            {
                case "Standart":
                    pictureTag += "blue";
                    break;

                case "High":
                    pictureTag += "yellow";
                    break;

                case "Critical":
                    pictureTag += "red";
                    break;
            }

            switch (status)
            {
                case "Appointed":
                    pictureTag += "border";
                    break;

                case "Cancel":
                    pictureTag += "cancel";
                    break;

                case "Done":
                    pictureTag += "done";
                    break;

                case "InWork":
                    pictureTag += "circle";
                    break;
            }
            return ResourceManager.GetImage(pictureTag);
        }

        private bool CheckEventBeforeClosing()
        {
            // TODO: Здесь будет проверка наряда перед закрытием
            return true;
        }

        private void Event_OnStart()
        {
            ChangeEventStatus();
            GetCurrentEvent();
        }

        private void ChangeEventStatus()
        {
            var result = DBHelper.GetCoordinate(TimeRangeCoordinate.DefaultTimeRange);
            var latitude = Converter.ToDouble(result["Latitude"]);
            var longitude = Converter.ToDouble(result["Longitude"]);

            var currentEventId = (string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId];
            var @event = (Event)DBHelper.LoadEntity(currentEventId);
            @event.ActualStartDate = DateTime.Now;
            @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.InWork);
            @event.Latitude = Converter.ToDecimal(latitude);
            @event.Longitude = Converter.ToDecimal(longitude);
            DBHelper.SaveEntity(@event);
            var rimList = DBHelper.GetServicesAndMaterialsByEventId(currentEventId);
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

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
            BusinessProcess.GlobalVariables[Parameters.IdClientId] =
                _currentEventRecordset[Parameters.IdClientId].ToString();
            Navigation.Move("ClientScreen");
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
            _topInfoComponent.Arrow_OnClick(sender, eventArgs);
            _rootLayout.Refresh();
        }

        internal void TaskCounterLayout_OnClick(object sender, EventArgs eventArgs)
        {
            if (CheckBigButtonActive(sender))
            {
                var dictionary = new Dictionary<string, object>
                {
                    {Parameters.IdClientId, _currentEventRecordset[Parameters.IdClientId]},
                    {Parameters.IdCurrentEventId, _currentEventRecordset["Id"]},
                    {Parameters.IdIsReadonly, _readonly }
                };
                Navigation.Move("TaskListScreen", dictionary);
            }
        }

        private bool CheckBigButtonActive(object sender)
        {
            // TODO: Сделать проверку более аккуратной?
            var layout = (HorizontalLayout)sender;
            return ((TextView)layout.Controls[2]).Text != "0";
        }

        internal void GoToCOCScreen_OnClick(object sender, EventArgs e)
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }

            var @event = (Event)DBHelper.LoadEntity(_currentEventRecordset["Id"].ToString());
            var status = ((StatusyEvents)@event.Status.GetObject()).GetEnum();
            var wasStarted = status == StatusyEventsEnum.InWork || status == StatusyEventsEnum.Done;
            var dictinory = new Dictionary<string, object>
            {
                {Parameters.IdCurrentEventId, (string) eventId},
                {Parameters.IdIsReadonly, _readonly},
                {Parameters.IdWasEventStarted, wasStarted}
            };
            Navigation.Move("COCScreen", dictinory);
        }

        internal void CheckListCounterLayout_OnClick(object sender, EventArgs eventArgs)
        {
            var statusName = (string)_currentEventRecordset["statusName"];
            if (statusName.Equals(EventStatus.Appointed))
            {
                Dialog.Ask(Translator.Translate("start_task"), (o, args) =>
                {
                    if (args.Result != Dialog.Result.Yes) return;
                    Event_OnStart();
                    Navigation.Move("CheckListScreen", new Dictionary<string, object>
                    {
                        [Parameters.IdIsReadonly] = _readonly
                    });
                });
            }
            else
            {
                Navigation.Move("CheckListScreen", new Dictionary<string, object>
                {
                    [Parameters.IdIsReadonly] = _readonly
                });
            }
        }

        internal DbRecordset GetCurrentEvent()
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }
            _currentEventRecordset = DBHelper.GetEventByID((string)eventId);
            return _currentEventRecordset;
        }

        internal string GetStringPartOfTotal(object part, object total)
         => !part.Equals(0) ? $"{part}/{total}" : $"{total}";

        internal string GetPrice(DbRecordset eventRecordset)
        {
            var status = (string)eventRecordset["statusName"];
            var sums = DBHelper.GetCocSumsByEventId(eventRecordset["Id"].ToString(),
                status != "Done" && status != "InWork");
            var total = (double)sums["Sum"];
            var services = (double)sums["SumServices"];
            var materials = (double)sums["SumMaterials"];
            if (!Settings.ShowMaterialPrice) return $"{services:N2} {Translator.Translate("currency")}";
            return Settings.ShowServicePrice
                ? $"{total:N2} {Translator.Translate("currency")}"
                : $"{materials:N2} {Translator.Translate("currency")}";
        }

        internal bool IsEmptyDateTime(string dateTime)
        {
            return dateTime == "0001-01-01 00:00:00";
        }

        internal bool IsNotCurrUser(DbRef UserIn)
        {
            if (Settings.UserDetailedInfo.Id.Guid == UserIn.Guid) return false;
            return true;
        }

        internal bool IsNotEmptyDateTime(string dateTime)
        {
            return dateTime != "0001-01-01 00:00:00";
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void GoToMapScreen_OnClick(object sender, EventArgs e)
        {
            var clientId = (string)_currentEventRecordset[Parameters.IdClientId];
            var dictionary = new Dictionary<string, object>
            {
                {Parameters.IdScreenStateId, MapScreenStates.EventScreen},
                {Parameters.IdClientId, clientId}
            };

            BusinessProcess.GlobalVariables.Remove(Parameters.IdScreenStateId);
            BusinessProcess.GlobalVariables.Remove(Parameters.IdClientId);
            BusinessProcess.GlobalVariables[Parameters.IdScreenStateId] = MapScreenStates.EventScreen;
            BusinessProcess.GlobalVariables[Parameters.IdClientId] = clientId;

            Navigation.Move("MapScreen", dictionary);
        }

        internal bool IsNotZero(long count)
            => Convert.ToInt64(count) != Convert.ToInt64(0L);

        internal string FormatTimer(string date)
        {
            var parsedDate = DateTime.Parse(date);
            var result = DateTime.Now - parsedDate;
            return $"{result.TotalHours:00}:{result.Minutes:00}";
        }

        internal bool ShowTaskButton() => Settings.EquipmentEnabled;

        internal string SelectAndShowRightPrice(double materials, double services)
        {
            if (!Settings.ShowMaterialPrice && !Settings.ShowServicePrice) return Parameters.EmptyPriceDescription;
            decimal total = 0;
            if (Settings.ShowMaterialPrice)
                total += (decimal)materials;
            if (Settings.ShowServicePrice)
                total += (decimal)services;
            return $"{total:N2} {Translator.Translate("currency")}";
        }

        internal long GetTotalTask(object eventId, object clientId)
            => DBHelper.GetTotalTaskByEventIdOrClientId(eventId, clientId);

        internal long GetTotalAnsweredTask(object eventId, object clientId)
            => DBHelper.GetTotalTaskAnsweredByEventIdOrClientId(eventId, clientId);

        internal void Delegate_OnClick(object sender, EventArgs e)
            => Navigation.Move($"{nameof(DelegateScreen)}", new Dictionary<string, object>
            { {Parameters.IdCurrentEventId, $"{_currentEventRecordset["Id"]}"} });

        internal void ChangeStartDatePlan_OnClick(object sender, EventArgs e)
        {
            if (_readonly) return;

            DateTime currentStartDate;

            if (!DateTime.TryParse($"{_currentEventRecordset["StartDatePlan"]}",
                out currentStartDate))
            {
                Utils.TraceMessage($"uncorrect format date {_currentEventRecordset["StartDatePlan"]}");
                currentStartDate = DateTime.Now;
            }

            Dialog.DateTime(Translator.Translate("select_date"), currentStartDate,
                (o, args) =>
                {
                    _startDatePlanTextView.Text = args.Result.ToString("HH:mm");
                    var @event = (Event)DBHelper.LoadEntity($"{_currentEventRecordset["Id"]}");
                    if (@event.EndDatePlan != DateTime.MinValue)
                        @event.EndDatePlan = args.Result + (@event.EndDatePlan - @event.StartDatePlan);
                    @event.StartDatePlan = args.Result;
                    DBHelper.SaveEntity(@event);
                });
        }
    }
}