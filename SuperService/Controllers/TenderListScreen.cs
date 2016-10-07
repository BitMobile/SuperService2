using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Components;

namespace Test
{
    public class TenderListScreen : Screen
    {
        private bool _needTodayBreaker = true;
        private bool _needTodayLayout = true;
        private TabBarComponent _tabBarComponent;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _tabBarComponent = new TabBarComponent(this);
            _topInfoComponent = new TopInfoComponent(this)
            {
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_sync") },
                RightButtonControl = new Image { Source = ResourceManager.GetImage("topheading_map") },
                Header = Translator.Translate("tenders"),
                ArrowVisible = false
            };

            var extraHorizontalLayout = new HorizontalLayout { CssClass = "ExtraHorizontalLayout" };
            var leftExtraLayout = new VerticalLayout { CssClass = "ExtraLeftLayoutCss" };
            var rightExtraLayout = new VerticalLayout { CssClass = "ExtraRightLayoutCss" };
            extraHorizontalLayout.AddChild(leftExtraLayout);
            extraHorizontalLayout.AddChild(rightExtraLayout);

            leftExtraLayout.AddChild(
                new TextView($"Новые")
                {
                    CssClass = "ExtraInfo"
                });
            leftExtraLayout.AddChild(new TextView(Translator.Translate("new").ToLower())
            {
                CssClass = "ButtonExtraInfo"
            });

            rightExtraLayout.AddChild(
                new TextView($"В работе")
                {
                    CssClass = "ExtraInfo"
                });
            rightExtraLayout.AddChild(new TextView(Translator.Translate("in_progress").ToLower())
            {
                CssClass = "ButtonExtraInfo"
            });

            _topInfoComponent.ExtraLayout.AddChild(extraHorizontalLayout);
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

        internal string GetDateNowEventList()
        {
            return DateTime.Now.ToString("dd-MM-yyyy");
        }

        internal string DateTimeToDateWithWeekCheck(string datetime)
        {
            var workDate = DateTime.Parse(datetime).Date;
            var currentDate = DateTime.Now.Date;

            DConsole.WriteLine($"week = {currentDate.GetWeekNumber()}");

            var workDateWeekNumber = workDate.GetWeekNumber();
            var currentDateWeekNumber = currentDate.GetWeekNumber();

            if (workDateWeekNumber == currentDateWeekNumber)
            {
                return DateTime.Parse(datetime).ToString("dddd, dd MMMM").ToUpper();
            }
            return DateTime.Parse(datetime).ToString("dd MMMM yyyy").ToUpper();
        }

        [Obsolete("Не используется на экране.")]
        internal string GetStartDate(string startPlan, string endPlan)
        {
            var startTime = DateTime.Parse(startPlan); //DateTime.Parse(startPlan).ToString("HH:mm:ss");
            var endTime = DateTime.Parse(endPlan); // .ToString("HH:mm");
            if (endTime - startTime > new TimeSpan(23, 59, 00))
            {
                return Translator.Translate("allday");
            }
            return startTime.ToString("HH:mm");
        }

        internal string GetTimeCounter(string actualStartDate, string statusName)
        {
            var actualTime = DateTime.Parse(actualStartDate);

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

        [Obsolete("Не используется в Биовитрум")]
        internal int SetTodayBreakerToFalse()
        {
            _needTodayBreaker = false;
            return 0;
        }

        internal bool IsDateEquals(string lastdate, string nowdate)
        {
            if (DateTime.Parse(lastdate).Date == DateTime.Parse(nowdate).Date)
            {
                return true;
            }
            return false;
        }

        internal bool IsDateEqualsOrLess(string lastdate, string nowdate)
        {
            return DateTime.Parse(lastdate).Date >= DateTime.Parse(nowdate).Date;
        }

        internal bool IsDateChanged(string lastdate, string nowdate)
        {
            if (DateTime.Parse(lastdate).Date < DateTime.Parse(nowdate).Date)
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

        internal IEnumerable GetTenders() =>
            DBHelper.GetTenderList(DBHelper.GetUserInfoByUserName(Settings.User)["Id"]);

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
        => Navigation.Move(nameof(TenderScreen),
            new Dictionary<string, object> { { Parameters.IdTenderId, ((HorizontalLayout)sender).Id } });

        // TabBar parts
        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Events_OnClick(sender, eventArgs);
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            //_tabBarComponent.TendersListScreen_OnClick(sender, eventArgs);
        }

        internal void TabBarThirdButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Clients_OnClick(sender, eventArgs);
        }

        internal void TabBarFourthButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Settings_OnClick(sender, eventArgs);
        }

        internal string ToUpper(object @string) => @string.ToString().ToUpper();

        internal string GetFormatDate(object date)
        {
            DateTime extractDate;

            if (!DateTime.TryParse(date.ToString(), out extractDate))
            {
                Utils.TraceMessage($"DateTime {date} don't parse");
            }

            return extractDate.ToString("dd.MM");
        }

        internal string ConcatCurrencyString(object activityTypeDescription, object totalSum)
            => $"{activityTypeDescription} - {totalSum:C2}";
    }
}