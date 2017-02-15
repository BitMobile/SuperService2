using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using System.Collections;
using Test.Document;
using Test.Enum;

namespace Test
{
    public class CloseEventScreen : Screen
    {
        private bool _problem;
        private HorizontalLayout _problemButton;
        private VerticalLayout _problemCommentLayout;
        private Image _problemImage;
        private bool _wantToBuy;
        private HorizontalLayout _wantToBuyButton;
        private VerticalLayout _wantToBuyCommentLayout;
        private Image _wantToBuyImage;

        private MemoEdit _wantToBuyCommentMemoEdit;
        private MemoEdit _problemCommentMemoEdit;
        private MemoEdit _commentaryMemoEdit;

        public override void OnLoading()
        {
            _wantToBuyButton = (HorizontalLayout)GetControl("WantToBuyButton", true);
            _wantToBuyCommentLayout = (VerticalLayout)GetControl("WantToBuyCommentLayout", true);
            _wantToBuyImage = (Image)GetControl("WantToBuyImage", true);

            _problemButton = (HorizontalLayout)GetControl("ProblemButton", true);
            _problemCommentLayout = (VerticalLayout)GetControl("ProblemCommentLayout", true);
            _problemImage = (Image)GetControl("ProblemImage", true);

            _wantToBuyCommentMemoEdit = (MemoEdit)GetControl("WantToBuyCommentMemoEdit", true);
            _problemCommentMemoEdit = (MemoEdit)GetControl("ProblemCommentMemoEdit", true);
            _commentaryMemoEdit = (MemoEdit)GetControl("CommentaryMemoEdit", true);
        }

        internal void WantToBuyButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (!_wantToBuy)
            {
                UpdateButtonCSS(_wantToBuyButton, _wantToBuyCommentLayout, _wantToBuyImage, "BigButtonPressed",
                    "CommentLayout", "closeevent_wtb_selected");
                _wantToBuy = true;
            }
            else
            {
                UpdateButtonCSS(_wantToBuyButton, _wantToBuyCommentLayout, _wantToBuyImage, "BigButton", "NoHeight",
                    "closeevent_wtb");
                _wantToBuy = false;
            }
        }

        private void UpdateButtonCSS(HorizontalLayout buttonLayout, VerticalLayout commentLayout, Image image,
            string buttonCSS,
            string commentCSS, string name)
        {
            buttonLayout.CssClass = buttonCSS;
            buttonLayout.Refresh();
            commentLayout.CssClass = commentCSS;
            commentLayout.Refresh();
            image.Source = ResourceManager.GetImage(name);
            image.Refresh();
        }

        internal void ProblemButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (!_problem)
            {
                UpdateButtonCSS(_problemButton, _problemCommentLayout, _problemImage, "BigButtonPressed",
                    "CommentLayout", "closeevent_problem_selected");
                _problem = true;
            }
            else
            {
                UpdateButtonCSS(_problemButton, _problemCommentLayout, _problemImage, "BigButton", "NoHeight",
                    "closeevent_problem");
                _problem = false;
            }
        }

        internal void FinishButton_OnClick(object sender, EventArgs eventArgs)
        {
            var eventRef = DbRef.FromString((string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
            var entitiesList = new ArrayList();
            if (_wantToBuy)
            {
                var reminder = CreateReminder(eventRef, _wantToBuyCommentMemoEdit.Text);
                reminder.ViewReminder = FoReminders.GetDbRefFromEnum(FoRemindersEnum.Sale);
                entitiesList.Add(reminder);
            }
            var @event = (Event)eventRef.GetObject();
            @event.ActualEndDate = (DateTime)BusinessProcess.GlobalVariables[Parameters.DateEnd];
            if (_problem)
            {
                @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.DoneWithTrouble);
                var reminder = CreateReminder(eventRef, _problemCommentMemoEdit.Text);
                reminder.ViewReminder = FoReminders.GetDbRefFromEnum(FoRemindersEnum.Problem);
                entitiesList.Add(reminder);
            }
            else
            {
                @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.Done);
            }
            if (!string.IsNullOrEmpty(_commentaryMemoEdit.Text))
            {
                @event.CommentContractor = _commentaryMemoEdit.Text;
            }
            entitiesList.Add(@event);
            entitiesList.Add(DBHelper.CreateHistory(@event));
            DBHelper.SaveEntities(entitiesList);
            Navigation.CleanStack();
            Navigation.ModalMove("EventListScreen");
        }

        private Reminder CreateReminder(DbRef eventRef, string text)
        {
            return new Reminder
            {
                Id = DbRef.CreateInstance("Document_Reminder", Guid.NewGuid()),
                Comment = text,
                Date = DateTime.Now,
                Reminders = eventRef,
            };
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}