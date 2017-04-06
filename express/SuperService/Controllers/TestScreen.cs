using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using Test.Components;

namespace Test
{
    public class TestScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;
        private Indicator _indicator;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                LeftButtonControl = new TextView("Отмена"),
                RightButtonControl = new TextView("Сохранить"),
                Header = "Курочка ряба",
            };

            _topInfoComponent.CommentLayout.AddChild(new TextView("Малая балканская, 17, Санкт-Петербург"));

            _topInfoComponent.ExtraLayout.AddChild(new TextView("Экстра инфо"));
            _topInfoComponent.ExtraLayout.AddChild(new TextView("Шамеймару, Марисса, Спелл Кард, Спелл Кард, Мастер Спарк, Экстра Фантазм"));

            _indicator = (Indicator)Variables["Indicator"];
            DConsole.WriteLine($"Yes, I loaded indicator ({_indicator})");
            _indicator.Start();
        }

        public override void OnShow()
        { }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            DConsole.WriteLine("I am left");
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
            DConsole.WriteLine("I am right");
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
            _topInfoComponent.Arrow_OnClick(sender, eventArgs);
        }

        internal void NiceStartButton_OnClick(object o, EventArgs eventArgs)
        {
            _indicator.Start();
        }

        internal void NiceStopButton_OnClick(object o, EventArgs eventArgs)
        {
            _indicator.Stop();
        }
    }
}