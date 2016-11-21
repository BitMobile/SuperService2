using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Globalization;
using Test.Components;

namespace Test
{
    public class ListScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                Header = Translator.Translate("activity"),
                ArrowVisible = false,
                ArrowActive = false
            };

            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
            => Navigation.Back();

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        internal IEnumerable GetTenderActivity()
            => DBHelper.GetActivitiByTender(Variables[Parameters.IdTenderId]);

        internal string ConcatCountUnit(Single count, string unit)
        {
            return string.Concat(count.ToString(CultureInfo.CurrentCulture), unit);
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}