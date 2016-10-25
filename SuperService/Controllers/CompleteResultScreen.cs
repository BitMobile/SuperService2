using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections.Generic;
using Test.Components;

namespace Test
{
    public class CompleteResultScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowVisible = false,
                ArrowActive = false,
                Header = Translator.Translate("complete_results"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") }
            };

            _topInfoComponent.ActivateBackButton();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.ModalMove(nameof(CloseEventScreen));
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal string GetResourceImage(object tag)
            => ResourceManager.GetImage($"{tag}");

        internal DbRecordset GetEventResult()
            => DBHelper.GetEventResults();

        internal void SelectedEventResult_OnClick(object sender, EventArgs e)
        => Navigation.ModalMove(nameof(CloseEventScreen),
            new Dictionary<string, object>
            {
                {
                    Parameters.IdResultEventId ,((VerticalLayout) sender).Id
                }
            });
    }
}