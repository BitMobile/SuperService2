﻿using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using System.Collections;
using Test.Components;
using Test.Enum;

namespace Test
{
    public class CancelEventScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            base.OnLoading();
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowActive = false,
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                RightButtonControl = new TextView(Translator.Translate("send")),
                Header = Translator.Translate("cancel"),
                ArrowVisible = false
            };
            _topInfoComponent.ActivateBackButton();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs args)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs args)
        {
            var eventId = (string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId];
            var @event = (Document.Event)DbRef.FromString(eventId).GetObject();
            var commentMemoEdit = (MemoEdit)Variables["CommentaryMemoEdit"];
            @event.CommentContractor = commentMemoEdit.Text;
            @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.Cancel);
            @event.ActualEndDate = DateTime.Now;
            var entitiesList = new ArrayList();
            entitiesList.Add(@event);
            entitiesList.Add(DBHelper.CreateHistory(@event));
            DBHelper.SaveEntities(entitiesList);
            Navigation.CleanStack();
            Navigation.ModalMove("EventListScreen");
        }

        internal void TopInfo_LeftButton_OnPressDown(object sender, EventArgs e)
        {
            Image image = (Image)_topInfoComponent.LeftButtonControl;
            image.Source = ResourceManager.GetImage("topheading_back_active");
            image.Refresh();
        }

        internal void TopInfo_LeftButton_OnPressUp(object sender, EventArgs e)
        {
            Image image = (Image)_topInfoComponent.LeftButtonControl;
            image.Source = ResourceManager.GetImage("topheading_back");
            image.Refresh();
        }

        internal void TopInfo_RightButton_OnPressDown(object sender, EventArgs e)
        {
            ((VerticalLayout)((TextView)_topInfoComponent.RightButtonControl).Parent).CssClass = "TopInfoButtonRightActive";
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_RightButton_OnPressUp(object sender, EventArgs e)
        {
            ((VerticalLayout)((TextView)_topInfoComponent.RightButtonControl).Parent).CssClass = "TopInfoButtonRight";
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs args)
        {
            _topInfoComponent.Arrow_OnClick(sender, args);
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}