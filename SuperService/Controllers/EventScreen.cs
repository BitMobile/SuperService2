using System;
using System.Collections;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
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
        private bool _needSync;
        private bool _readonly;
        private Button _refuseButton;
        private DockLayout _rootLayout;
        private Button _startButton;

        private VerticalLayout _startFinishButton;
        private Image _statusImage;
        private bool _taskCommentTextExpanded;
        private TextView _taskCommentTextView;

        private TopInfoComponent _topInfoComponent;
        private Image _wrapUnwrapImage;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this);
            _topInfoComponent.ActivateBackButton();
            LoadControls();
            FillControls();

            IsEmptyDateTime((string) _currentEventRecordset["ActualStartDate"]);
            
        }

        private bool ReadEvent()
        {
            var currentEventId = (string) BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId];
            var @event = (Event) DBHelper.LoadEntity(currentEventId);
            if (@event.Status.Equals(StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.Agreed)))
            {
                @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.Accepted);
                var entitesList = new ArrayList();
                entitesList.Add(@event);
                entitesList.Add(DBHelper.CreateHistory(@event));
                DBHelper.SaveEntities(entitesList, false);
                GetCurrentEvent();
                return true;
            }
            return false;
        }


        private void FillControls()
        {
            _topInfoComponent.Header =
                ((string) _currentEventRecordset["clientDescription"]).CutForUIOutput(13, 2);
            _topInfoComponent.CommentLayout.AddChild(new TextView(
                ((string) _currentEventRecordset["clientAddress"]).CutForUIOutput(17, 2)));

            _topInfoComponent.LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")};
            _topInfoComponent.RightButtonControl = new Image {Source = ResourceManager.GetImage("topheading_info")};

            _taskCommentTextView = (TextView) GetControl("EventCommentTextView", true);
            _wrapUnwrapImage = (Image) GetControl("WrapUnwrapImage", true);

            var extraHorizontalLayout = new HorizontalLayout {CssClass = "ExtraHorizontalLayout"};
            var leftExtraLayout = new VerticalLayout {CssClass = "ExtraVerticalLayout"};
            var rightExtraLayout = new VerticalLayout {CssClass = "ExtraVerticalLayout"};

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
            var text = (string) _currentEventRecordset["ContactVisitingDescription"];
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
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
            Navigation.Move(nameof(ContactScreen), new Dictionary<string, object>
            {
                [Parameters.Contact] = (Contacts) DBHelper.LoadEntity(_currentEventRecordset["contactId"].ToString())
            });
        }

        private bool CheckAndGoIfNotExsist()
        {
            GetCurrentEvent();
            Utils.TraceMessage($"{_currentEventRecordset["UserId"] == null}");
            if (_currentEventRecordset["UserId"] == null || !Equals(_currentEventRecordset["UserId"], Settings.UserDetailedInfo.Id))
            {
                Toast.MakeToast(Translator.Translate("eventChangeSr"));
                Navigation.ModalMove(nameof(EventListScreen));
                return true;
            }
            return false;
        }
        public override void OnShow()
        {
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
            _needSync = ReadEvent();
            GpsTracking.Start();
            if ((string) _currentEventRecordset["statusName"] == EventStatus.Done
                || (string) _currentEventRecordset["statusName"] == EventStatus.DoneWithTrouble
                || (string) _currentEventRecordset["statusName"] == EventStatus.OnTheApprovalOf
                || (string) _currentEventRecordset["statusName"] == EventStatus.Close
                || (string) _currentEventRecordset["statusName"] == EventStatus.NotDone)
            {
                Toast.MakeToast(Translator.Translate("event_finished_ro"));
                _readonly = true;
            }
            if ((string) _currentEventRecordset["statusName"] == EventStatus.Cancel)
            {
                Toast.MakeToast(Translator.Translate("event_canceled_ro"));
                _readonly = true;
            }
        }

        private void LoadControls()
        {
            _rootLayout = (DockLayout) Variables["RootLayout"];
            _startFinishButton = (VerticalLayout) Variables.GetValueOrDefault("StartFinishButton");
            _startButton = (Button) Variables.GetValueOrDefault("StartButton");
            _refuseButton = (Button) Variables.GetValueOrDefault("RefuseButton");
            _statusImage = (Image) Variables.GetValueOrDefault("StatusImage");
        }

        internal void ClientInfoButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
            Navigation.Move("ClientScreen");
        }

        internal void RefuseButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
            Navigation.Move("CancelEventScreen");
        }

        internal string FormatEventStartDatePlanTime(string eventStartDatePlan)
        {
            return DateTime.Parse(eventStartDatePlan).ToString("HH:mm");
        }

        internal void StartButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
            Dialog.Ask(Translator.Translate("areYouSure"), (o, args) =>
            {
                if (args.Result == Dialog.Result.Yes)
                    ChangeLayoutToStartedEvent();
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
            _refuseButton.CssClass = "NoHeight";
            _refuseButton.Visible = false;
            _refuseButton.Refresh();
            _startFinishButton.CssClass = "FinishButton";
            _startFinishButton.Refresh();
            _statusImage.Source = GetStatusPicture((string) _currentEventRecordset["ImportanceName"], "InWork");
            _rootLayout.Refresh();
            Event_OnStart();
        }

        internal void StartFinishButton_OnClick(object sender, EventArgs eventArgs)
        {
            var result = DBHelper.GetTotalFinishedRequireQuestionByEventId(
                (string) BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);

            var isActiveEvent = !result.Next() || (long) result["count"] == 0;

            if (isActiveEvent)
                Dialog.Alert(Translator.Translate("closeeventquestion"), (o, args) =>
                    {
                        if (!CheckEventBeforeClosing() || args.Result != 0) return;
                       
                        Navigation.Move("CloseEventScreen");
                    }, null,
                    Translator.Translate("yes"), Translator.Translate("no"));
            else
                Dialog.Message(Translator.Translate("unfinished_business"));
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

            if (status == EventStatus.Agreed || status == EventStatus.Accepted)
                pictureTag += "border";
            else if (status == EventStatus.Cancel)
                pictureTag += "cancel";
            else if (status == EventStatus.DoneWithTrouble || status == EventStatus.Done ||
                     status == EventStatus.OnTheApprovalOf || status == EventStatus.Close ||
                     status == EventStatus.NotDone)
                pictureTag += "done";
            else if (status == EventStatus.InWork)
                pictureTag += "circle";
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

            var currentEventId = (string) BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId];
            var @event = (Event) DBHelper.LoadEntity(currentEventId);
            @event.ActualStartDate = DateTime.Now;
            @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.InWork);
            @event.LatitudeStart = Converter.ToDecimal(latitude);
            @event.LongitudeStart = Converter.ToDecimal(longitude);
            var enitylist = new ArrayList();
            enitylist.Add(@event);
            enitylist.Add(DBHelper.CreateHistory(@event));
            DBHelper.SaveEntities(enitylist);
            _needSync = false;
            var rimList = DBHelper.GetServicesAndMaterialsByEventId(currentEventId);
            var rimArrayList = new ArrayList();
            while (rimList.Next())
            {
                var rim = (Event_ServicesMaterials) ((DbRef) rimList["Id"]).GetObject();
                rim.AmountFact = rim.AmountPlan;
                rim.SumFact = rim.SumPlan;
                rimArrayList.Add(rim);
            }
            DBHelper.SaveEntities(rimArrayList, false);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_needSync)
                DBHelper.SyncAsync();
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
            BusinessProcess.GlobalVariables[Parameters.IdClientId] =
                _currentEventRecordset[Parameters.IdClientId].ToString();
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
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
                    {Parameters.IdIsReadonly, _readonly}
                };
                if (CheckAndGoIfNotExsist())
                {
                    return;
                }
                Navigation.Move("TaskListScreen", dictionary);
            }
        }

        private bool CheckBigButtonActive(object sender)
        {
            // TODO: Сделать проверку более аккуратной?
            var layout = (HorizontalLayout) sender;
            return ((TextView) layout.GetControl(2)).Text != "0";
        }

        internal void GoToCOCScreen_OnClick(object sender, EventArgs e)
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
                DConsole.WriteLine("Can't find current event ID, going to crash");
            var @event = (Event) DBHelper.LoadEntity(_currentEventRecordset["Id"].ToString());
            var status = ((StatusyEvents) @event.Status.GetObject()).Name;
            var wasStarted = status == EventStatus.InWork || status == EventStatus.Done 
                || status == EventStatus.Close || status == EventStatus.NotDone 
                || status == EventStatus.DoneWithTrouble || status == EventStatus.OnTheApprovalOf;

            var dictinory = new Dictionary<string, object>
            {
                {Parameters.IdCurrentEventId, (string) eventId},
                {Parameters.IdIsReadonly, _readonly},
                {Parameters.IdWasEventStarted, wasStarted}
            };
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
            Navigation.Move(nameof(COCScreen), dictinory);
        }

        internal void CheckListCounterLayout_OnClick(object sender, EventArgs eventArgs)
        {
            var statusName = (string) _currentEventRecordset["statusName"];
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
            if (statusName.Equals(EventStatus.Agreed, StringComparison.OrdinalIgnoreCase)
                || statusName.Equals(EventStatus.Accepted, StringComparison.OrdinalIgnoreCase))
                Dialog.Ask(Translator.Translate("start_event"), (o, args) =>
                {
                    if (args.Result != Dialog.Result.Yes) return;
                    Event_OnStart();
                    Navigation.Move("CheckListScreen", new Dictionary<string, object>
                    {
                        [Parameters.IdIsReadonly] = _readonly
                    });
                });
            else
                Navigation.Move("CheckListScreen", new Dictionary<string, object>
                {
                    [Parameters.IdIsReadonly] = _readonly
                });
        }

        internal DbRecordset GetCurrentEvent()
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
                DConsole.WriteLine("Can't find current event ID, going to crash");
            _currentEventRecordset = DBHelper.GetEventByID((string) eventId);
            return _currentEventRecordset;
        }

        internal string GetStringPartOfTotal(object part, object total)
            => !part.Equals(0) ? $"{part}/{total}" : $"{total}";

        internal string GetPrice(DbRecordset eventRecordset)
        {
            var status = (string) eventRecordset["statusName"];
            var sums = DBHelper.GetCocSumsByEventId(eventRecordset["Id"].ToString(),
                status != EventStatus.Done && status != EventStatus.InWork
                && status != EventStatus.Close && status != EventStatus.NotDone
                && status != EventStatus.DoneWithTrouble && status != EventStatus.OnTheApprovalOf);
            var total = (double) sums["Sum"];
            var services = (double) sums["SumServices"];
            var materials = (double) sums["SumMaterials"];
            if (!Settings.ShowMaterialPrice) return $"{services:N2} {Translator.Translate("currency")}";
            return Settings.ShowServicePrice
                ? $"{total:N2} {Translator.Translate("currency")}"
                : $"{materials:N2} {Translator.Translate("currency")}";
        }

        internal bool IsEmptyDateTime(string dateTime)
        {
            return dateTime == "0001-01-01 00:00:00";
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
            var clientId = (string) _currentEventRecordset[Parameters.IdClientId];
            var dictionary = new Dictionary<string, object>
            {
                {Parameters.IdScreenStateId, MapScreenStates.EventScreen},
                {Parameters.IdClientId, clientId}
            };

            BusinessProcess.GlobalVariables.Remove(Parameters.IdScreenStateId);
            BusinessProcess.GlobalVariables.Remove(Parameters.IdClientId);
            BusinessProcess.GlobalVariables[Parameters.IdScreenStateId] = MapScreenStates.EventScreen;
            BusinessProcess.GlobalVariables[Parameters.IdClientId] = clientId;
            if (CheckAndGoIfNotExsist())
            {
                return;
            }
            Navigation.Move(nameof(MapScreen), dictionary);
        }

        internal bool IsNotZero(long count)
            => Convert.ToInt64(count) != Convert.ToInt64(0L);

        internal string FormatTimer(string date)
        {
            var parsedDate = DateTime.Parse(date);
            var result = DateTime.Now - parsedDate;
            return $"{(int)result.TotalHours:00}:{result.Minutes:00}";
        }

        internal bool ShowTaskButton() => Settings.EquipmentEnabled;

        internal string SelectAndShowRightPrice(double materials, double services)
        {
            if (!Settings.ShowMaterialPrice && !Settings.ShowServicePrice) return Parameters.EmptyPriceDescription;
            decimal total = 0;
            if (Settings.ShowMaterialPrice)
                total += (decimal) materials;
            if (Settings.ShowServicePrice)
                total += (decimal) services;
            return $"{total:N2} {Translator.Translate("currency")}";
        }

        internal long GetTotalTask(object eventId, object clientId)
            => DBHelper.GetTotalTaskByEventIdOrClientId(eventId, clientId);

        internal long GetTotalAnsweredTask(object eventId, object clientId)
            => DBHelper.GetTotalTaskAnsweredByEventIdOrClientId(eventId, clientId);
    }
}