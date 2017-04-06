using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using Test.Components;

namespace Test
{
    public class RequestHistoryScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        private DateTime _previousDate = new DateTime(1,1,1);

        public override void OnLoading()
        {
            DConsole.WriteLine("RequestHistoryScreen init");

            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("requests"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                ArrowVisible = false
            };
        }

        internal string GetDateNowRequestHistory()
        {
            return DateTime.Now.ToString("dddd dd MMMM");
        }

        internal string DateTimeToDate(string datetime)
        {
            return DateTime.Parse(datetime).ToString("dddd dd MMMM");
        }

        internal string ToHoursMinutes(string datetime)
        {
            return TimeSpan.Parse(datetime).ToString(@"hh\:mm");
        }


        internal bool DateIsToday(string requestDate)
        {
            //DConsole.WriteLine();
            return (DateTime.Parse(requestDate).Date == DateTime.Now.Date);
        }

        internal string GetDateHeaderDescription(string requestDate)
        {
            var mDate = DateTime.Parse(requestDate).Date;
            var daysDelta = DateTime.Now.DayOfWeek==DayOfWeek.Sunday?6:(int)DateTime.Now.DayOfWeek - 1;

            if (DateIsToday(requestDate))
            {
                return Translator.Translate("todayUpper");
            }

            if (mDate < DateTime.Now.Date && mDate >= DateTime.Now.Date.AddDays(-1 * daysDelta))
            {
                return mDate.ToString("dddd, dd MMMM").ToUpper();
            }
            return mDate.ToString("dd MMMM yyyy").ToUpper();
        }


        internal bool NeedDateLayout(string requestDate)
        {
            var result = false;
            if(_previousDate != DateTime.Parse(requestDate).Date)
            {
                _previousDate = DateTime.Parse(requestDate).Date;
                result = true;
            }
            return result;
        }

        internal IEnumerable GetNeedMats()
        {
            return DBHelper.GetNeedMats();
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        // TopInfo parts
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
    }
}