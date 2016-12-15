using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using ClientModel3.MD;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Components;

namespace Test
{
    public class EventListScreen : Screen
    {
        private bool _needTodayBreaker = true;
        private bool _needTodayLayout = true;
        private TabBarComponent _tabBarComponent;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            DConsole.WriteLine("OnLoading EventList");

            _tabBarComponent = new TabBarComponent(this);
            _topInfoComponent = new TopInfoComponent(this)
            {
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_sync") },
                RightButtonControl = new Image { Source = ResourceManager.GetImage("topheading_map") },
                Header = Translator.Translate("orders")
            };

            var statistic = DBHelper.GetEventsStatistic();

            var extraHorizontalLayout = new HorizontalLayout { CssClass = "ExtraHorizontalLayout" };
            var leftExtraLayout = new VerticalLayout { CssClass = "ExtraLeftLayoutCss" };
            var rightExtraLayout = new VerticalLayout { CssClass = "ExtraRightLayoutCss" };
            extraHorizontalLayout.AddChild(leftExtraLayout);
            extraHorizontalLayout.AddChild(rightExtraLayout);

            leftExtraLayout.AddChild(
                new TextView($"{statistic.DayCompleteAmout}/{statistic.DayTotalAmount}")
                {
                    CssClass = "ExtraInfo"
                });
            leftExtraLayout.AddChild(new TextView(Translator.Translate("today"))
            {
                CssClass = "ButtonExtraInfo"
            });

            rightExtraLayout.AddChild(
                new TextView($"{statistic.MonthCompleteAmout}/{statistic.MonthTotalAmount}")
                {
                    CssClass = "ExtraInfo"
                });
            rightExtraLayout.AddChild(new TextView(Translator.Translate("per_month"))
            {
                CssClass = "ButtonExtraInfo"
            });

            _topInfoComponent.ExtraLayout.AddChild(extraHorizontalLayout);
        }

        public override void OnShow()
        {
            GpsTracking.Start();
            PushService.Init();
            DynamicScreenRefreshService.Init();
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
            {
                pictureTag += "border";
            }
            else if (status == EventStatus.Cancel || status == EventStatus.NotDone)
            {
                pictureTag += "cancel";
            }
            else if (status == EventStatus.DoneWithTrouble || status == EventStatus.Done ||
                     status == EventStatus.OnTheApprovalOf || status == EventStatus.Close)
            {
                pictureTag += "done";
            }
            else if (status == EventStatus.InWork)
            {
                pictureTag += "circle";
            }
            return ResourceManager.GetImage(pictureTag);
        }

        internal string GetDateNowEventList()
        {
            return DateTime.Now.ToString("dd-MM-yyyy");
        }

        internal string DateTimeToDateWithWeekCheck(string datetime)
        {
            DateTime workDate;
            var Wecan = DateTime.TryParse(datetime,out workDate);
            if (Wecan) {
                workDate = workDate.Date;
            }
            else {
                workDate = DateTime.MinValue.Date;
                return datetime;
            }
            var currentDate = DateTime.Now.Date;

            DConsole.WriteLine($"week = {currentDate.GetWeekNumber()}");

            var workDateWeekNumber = workDate.GetWeekNumber();
            var currentDateWeekNumber = currentDate.GetWeekNumber();

            if (workDate.Equals(currentDate.AddDays(1)))
            {
                return Translator.Translate("tomorrowToUpper");
            }

            if (workDate.Equals(currentDate.AddDays(-1)))
            {
                return Translator.Translate("yesterdayToUpper");
            }

            if (workDate.Equals(currentDate))
            {
                return Translator.Translate("todayUpper");
            }

            if (workDateWeekNumber == currentDateWeekNumber)
            {
                return workDate.ToString("dddd, dd MMMM").ToUpper();
            }
            return workDate.ToString("dd MMMM yyyy").ToUpper();
        }

        internal string GetStartDate(string startPlan, string endPlan)
        {
            DateTime startTime;
            var WeCanStart = DateTime.TryParse(startPlan,out startTime); //DateTime.Parse(startPlan).ToString("HH:mm:ss");
            DateTime endTime;
            var WeCanEnd = DateTime.TryParse(endPlan,out endTime); // .ToString("HH:mm");
            if(!WeCanEnd || !WeCanStart)
            {
                return startPlan;
            }
            if (endTime - startTime > new TimeSpan(23, 59, 00))
            {
                return Translator.Translate("allday");
            }
            return startTime.ToString("HH:mm");
        }

        internal string GetTimeCounter(string actualStartDate, string statusName)
        {
            DateTime actualTime; 
            var WeCan = DateTime.TryParse(actualStartDate,out actualTime);

            if (!WeCan)
            {
                return "";
            }
            if ((actualTime == default(DateTime)) || statusName != "InWork")
                return "";

            var ans = DateTime.Now - actualTime;
            var hours = (int)ans.TotalHours;
            if (ans < TimeSpan.FromHours(1))
                return $"{ans.Minutes} {Translator.Translate("min.")}";
            if (ans < TimeSpan.FromHours(24))
                return $"{hours} {Translator.Translate("h.")} {ans.Minutes} {Translator.Translate("m.")}";
            return $"{hours} {Translator.Translate("h.")}";
        }

        internal int SetTodayLayoutToFalse()
        {
            _needTodayLayout = false;
            return 0;
        }

        internal int SetTodayBreakerToFalse()
        {
            _needTodayBreaker = false;
            return 0;
        }

        internal bool IsDateEquals(string lastdate, string nowdate)
        {
            DateTime lastdateDate;
            var WeCanlastDate = DateTime.TryParse(lastdate,out lastdateDate);
            DateTime nowdateDate;
            var WeCabNowDate = DateTime.TryParse(nowdate,out nowdateDate);
            if (!WeCanlastDate || !WeCabNowDate) {
                return lastdate.Equals(nowdate);
            }
            if (lastdateDate == nowdateDate)
            {
                return true;
            }
            return false;
        }

        internal bool IsDateEqualsOrLess(string lastdate, string nowdate)
        {
            DateTime date1;
            DateTime date2;
            if (DateTime.TryParse(nowdate, out date2) && DateTime.TryParse(lastdate, out date1))
            {
                return date1.Date >= date2.Date;
            }
            else {
                return lastdate.Equals(nowdate);
            };
            //return DateTime.Parse(lastdate).Date >= DateTime.Parse(nowdate).Date;
        }

        internal bool IsDateChanged(string lastdate, string nowdate)
        {
            DateTime lastdateDate;
            var WeCanLastDate = DateTime.TryParse(lastdate,out lastdateDate);
            DateTime nowdateDate;
            var WeCanNow = DateTime.TryParse(nowdate, out nowdateDate);
            if (!WeCanLastDate || !WeCanNow) return lastdate.Equals(nowdate);
            if (lastdateDate < nowdateDate)
            {
                return true;
            }
            return false;
        }

        internal bool IsTodayLayoutNeed()
        {
            return _needTodayLayout;
        }

        internal bool IsTodayBreakerNeed()
        {
            return _needTodayBreaker;
        }

        internal string DateTimeToDate(string datetime)
        {
            return DateTime.Parse(datetime).ToString("dddd dd MMMM");
        }

        internal IEnumerable GetEvents()
        {
            return DBHelper.GetEvents(DateTime.Now.Date.AddDays(-31),DateTime.Now.Date.AddDays(31));
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        // TopInfo parts
        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Toast.MakeToast(Translator.Translate("start_sync"));
            DBHelper.SyncAsync();
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
            DConsole.WriteLine("GO to map");
            DConsole.WriteLine("Before dictionary");
            var dictionary = new Dictionary<string, object>
            {
                {Parameters.IdScreenStateId, MapScreenStates.EventListScreen}
            };
            DConsole.WriteLine("After");
            BusinessProcess.GlobalVariables[Parameters.IdScreenStateId] = MapScreenStates.EventListScreen;
            Navigation.Move("MapScreen", dictionary);
        }

        internal void EventListItemHL_OnClick(object sender, EventArgs e)
        {
            DConsole.WriteLine("Go To View Event");
            var currentEvent = (HorizontalLayout)sender;
            BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId] = currentEvent.Id;
            Navigation.Move("EventScreen");
        }

        // TabBar parts
        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            //_tabBarComponent.Events_OnClick(sender, eventArgs);
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Bag_OnClick(sender, eventArgs);
        }

        internal void TabBarThirdButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Clients_OnClick(sender, eventArgs);
        }

        internal void TabBarFourthButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Settings_OnClick(sender, eventArgs);
        }
    }

    public enum MapMarkerColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Orange
    }
}