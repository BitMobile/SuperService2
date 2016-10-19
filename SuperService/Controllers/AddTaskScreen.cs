using BitMobile.BusinessProcess.ClientModel;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using Test.Components;
using Dialog = BitMobile.ClientModel3.Dialog;

namespace Test
{
    public class AddTaskScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowActive = false,
                ArrowVisible = false,
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                Header = Translator.Translate("add_task")
            };

            _topInfoComponent.ActivateBackButton();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal string GetResourceImage(object tag)
            => ResourceManager.GetImage($"{tag}");

        internal void CreateTask_OnClick(object sender, EventArgs e)
        {
        }

        internal void SelectTaskType_OnClick(object sender, EventArgs e)
        {
        }

        internal void StartDatePlan_OnClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            Dialog.DateTime("Выберите дату", DateTime.Now, (o, args) =>
            {
                btn.Text = $"{args.Result:g}";
            });
        }

        internal void EndDatePlan_OnClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            Dialog.DateTime("Выберите дату", DateTime.Now, (o, args) =>
            {
                btn.Text = $"{args.Result:g}";
            });
        }
    }
}