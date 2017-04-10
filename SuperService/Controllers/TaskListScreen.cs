using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Components;

namespace Test
{
    public class TaskListScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            base.OnLoading();
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("tasks"),
                LeftButtonControl = new Image() { Source = ResourceManager.GetImage("topheading_back") },
                ArrowVisible = false
            };
            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
            base.OnShow();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_LeftButton_OnPressDown(object sender, EventArgs e)
        {
            ((Image)_topInfoComponent.LeftButtonControl).Source = ResourceManager.GetImage("topheading_back_active");
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_LeftButton_OnPressUp(object sender, EventArgs e)
        {
            ((Image)_topInfoComponent.LeftButtonControl).Source = ResourceManager.GetImage("topheading_back");
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_RightButton_OnPressDown(object sender, EventArgs e)
        {
        }

        internal void TopInfo_RightButton_OnPressUp(object sender, EventArgs e)
        {
        }

        internal void TaskLayout_OnClick(object sender, EventArgs eventArgs)
        {
            var dictionary = new Dictionary<string, object>()
            {
                {Parameters.IdTaskId, ((HorizontalLayout)sender).Id},
                {Parameters.IdCurrentEventId, $"{Variables[Parameters.IdCurrentEventId]}" },
                {Parameters.IdClientId, $"{Variables[Parameters.IdClientId]}" },
                {Parameters.IdIsReadonly, Variables[Parameters.IdIsReadonly] }
            };
            Navigation.Move("TaskScreen", dictionary);
        }

        internal void TopInfo_Arrow_OnClick(object sernder, EventArgs eventArgs)
        {
            _topInfoComponent.Arrow_OnClick(sernder, eventArgs);
        }

        internal object GetEvent()
        {
            return DBHelper.GetEventByID((string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
        }

        internal IEnumerable GetTasks()
           => DBHelper.GetTaskList($"{Variables[Parameters.IdCurrentEventId]}",
                $"{Variables[Parameters.IdClientId]}");

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}