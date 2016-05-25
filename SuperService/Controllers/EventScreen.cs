﻿using System;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class EventScreen : Screen
    {
        private DbRecordset _currentEventRecordset;
        private Button _refuseButton;
        private DockLayout _rootLayout;
        private Button _startButton;

        private Button _startFinishButton;

        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this);

            LoadControls();
            FillControls();

            IsEmptyDateTime((string) _currentEventRecordset["ActualStartDate"]);
        }

        private void FillControls()
        {
            _topInfoComponent.HeadingTextView.Text = (string) _currentEventRecordset["clientDescription"];
            _topInfoComponent.CommentTextView.Text = (string) _currentEventRecordset["clientAddress"];
            _topInfoComponent.LeftButtonImage.Source = @"Image\top_back.png";
            _topInfoComponent.RightButtonImage.Source = @"Image\top_info.png";

            _topInfoComponent.LeftExtraLayout.AddChild(new Image
            {
                CssClass = "TopInfoSideImage",
                Source = @"Image\top_map.png"
            });
            _topInfoComponent.LeftExtraLayout.AddChild(new TextView
            {
                Text = Translator.Translate("onmap"),
                CssClass = "TopInfoSideText"
            });

            _topInfoComponent.RightExtraLayout.AddChild(new Image
            {
                CssClass = "TopInfoSideImage",
                Source = @"Image\top_person.png"
            });
            _topInfoComponent.RightExtraLayout.AddChild(new TextView
            {
                Text = (string) _currentEventRecordset["clientDescription"],
                CssClass = "TopInfoSideText"
            });
        }

        private void LoadControls()
        {
            _rootLayout = (DockLayout) GetControl("RootLayout");
            _startFinishButton = (Button) GetControl("StartFinishButton", true);
            _startButton = (Button) GetControl("StartButton", true);
            _refuseButton = (Button) GetControl("RefuseButton", true);
        }

        internal void ClientInfoButton_OnClick(object sender, EventArgs eventArgs)
        {
            BusinessProcess.DoAction("Client");
        }

        internal void RefuseButton_OnClick(object sender, EventArgs eventArgs)
        {
            DBHelper.UpdateCancelEventById((string) BusinessProcess.GlobalVariables["currentEventId"]);
            BusinessProcess.DoAction("EventList");
        }

        internal string FormatEventStartDatePlanTime(string eventStartDatePlanTime)
        {
            return eventStartDatePlanTime.Substring(0, 5);
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

        private void ChangeLayoutToStartedEvent()
        {
            _startButton.CssClass = "NoHeight";
            _startButton.Visible = false;
            _startButton.Refresh();
            _refuseButton.CssClass = "NoHeight";
            _refuseButton.Visible = false;
            _refuseButton.Refresh();
            _startFinishButton.CssClass = "FinishButton";
            _startFinishButton?.Refresh();
            _startFinishButton.Text = Translator.Translate("finish");
            _rootLayout.Refresh();
            Event_OnStart();
        }

        internal void StartFinishButton_OnClick(object sender, EventArgs eventArgs)
        {
            Dialog.Alert(Translator.Translate("closeeventquestion"), (o, args) =>
            {
                if (CheckEventBeforeClosing() && args.Result == 0)
                {
                    DBHelper.UpdateActualEndDateByEnetId(DateTime.Now,
                        (string) BusinessProcess.GlobalVariables["currentEventId"]);
                    BusinessProcess.DoAction("CloseEvent");
                }

            }, null,
                Translator.Translate("yes"), Translator.Translate("no"));
        }

        private bool CheckEventBeforeClosing()
        {
            // TODO: Здесь будет проверка наряда перед закрытием
            return true;
        }

        private void Event_OnStart()
        {
            DBHelper.UpdateActualStartDateByEventId(DateTime.Now,
                (string) BusinessProcess.GlobalVariables["currentEventId"]);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            BusinessProcess.DoAction("EventList");
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
            DConsole.WriteLine("Nothing to see here");
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
            _topInfoComponent.Arrow_OnClick(sender, eventArgs);
            _rootLayout.Refresh();
        }

        internal void TaskCounterLayout_OnClick(object sender, EventArgs eventArgs)
        {
            BusinessProcess.DoAction("ViewTasks");
        }

        internal DbRecordset GetCurrentEvent()
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue("currentEventId", out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }
            _currentEventRecordset = DBHelper.GetEventByID((string) eventId);
            return _currentEventRecordset;
        }

        internal string GetStringPartOfTotal(long part, long total)
        {
            if (Convert.ToInt64(part) != 0) return $"{part}/{total}";
//            DConsole.WriteLine($"{part == 0L}, {Convert.ToInt64(total) == 0L}, {part}, {total}");
            return $"{total}";
        }

        internal bool IsEmptyDateTime(string dateTime)
        {
            return dateTime == "0001-01-01 00:00:00";
        }
    }
}