using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using System.Collections;
using Test.Catalog;
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
        private TextView _closeResult;
        private static EventResults _eventResults;

        private static string _commentaryString;
        private static string _wantToBuyString;
        private static string _problemCommentString;

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
            _closeResult = (TextView)GetControl("75097b50211a453883f5e31f4e878c2c", true);

            if (_eventResults == null) _eventResults = new EventResults();

            if (_commentaryString == null)
            {
                Utils.TraceMessage($"Стока {_commentaryString} пустая");
                _commentaryString = string.Empty;
            }
            if (_wantToBuyString == null)
            {
                Utils.TraceMessage($"Стока {_wantToBuyString} пустая");
                _wantToBuyString = string.Empty;
            }
            if (_problemCommentString == null)
            {
                Utils.TraceMessage($"Стока {_problemCommentString} пустая");
                _problemCommentString = string.Empty;
            }
        }

        public override void OnShow()
        {
            var result = Variables.GetValueOrDefault(Parameters.IdResultEventId);
            if (result != null)
            {
                var closeEventResult = (EventResults)DbRef.FromString($"{result}").GetObject();
                _closeResult.Text = closeEventResult.Description;
                _eventResults = closeEventResult;
            }

            InitComponents();
        }

        private void InitComponents()
        {
            _commentaryMemoEdit.Text = _commentaryString;
            _wantToBuyCommentMemoEdit.Text = _wantToBuyString;
            _problemCommentMemoEdit.Text = _problemCommentString;

            _closeResult.Text = string.IsNullOrEmpty(_eventResults.Description)
                ? Translator.Translate("not_choosed")
                : _eventResults.Description;

            Utils.TraceMessage($"_commentMemoEdit.Text {_commentaryMemoEdit.Text}{Environment.NewLine}" +
                               $"{nameof(_commentaryString)} {_commentaryString}{Environment.NewLine}" +
                               $"{nameof(_wantToBuyCommentMemoEdit)}.Text {_wantToBuyCommentMemoEdit.Text}{Environment.NewLine}" +
                               $"{nameof(_wantToBuyString)} {_wantToBuyString}{Environment.NewLine}" +
                               $"{nameof(_problemCommentMemoEdit)}.Text {_problemCommentMemoEdit.Text}{Environment.NewLine}" +
                               $"{nameof(_problemCommentString)}");
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
            if (_eventResults.Id == null)
            {
                Toast.MakeToast("Результат завершения не может быть пустым");
                return;
            }

            Utils.TraceMessage($"_eventResult.Id.Empty: {_eventResults.Id.EmptyRef()} not {!_eventResults.Id.EmptyRef()}{Environment.NewLine}" +
                               $"_evenResult.Negative {_eventResults.Negative} {Environment.NewLine}" +
                               $"string.IsNullOrEmpty(_commentaryMemoEdit.Text) {string.IsNullOrEmpty(_commentaryMemoEdit.Text)}{Environment.NewLine}" +
                               $"Total Result: {!_eventResults.Id.EmptyRef() && _eventResults.Negative && string.IsNullOrEmpty(_commentaryMemoEdit.Text)}");

            if (!_eventResults.Id.EmptyRef() && _eventResults.Negative && string.IsNullOrEmpty(_commentaryMemoEdit.Text))
            {
                Toast.MakeToast("Комментарий не может быть пустым");
                return;
            }
            var eventRef = DbRef.FromString((string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
            var entitiesList = new ArrayList();
            var @event = (Event)eventRef.GetObject();

            if (_wantToBuy)
            {
                var reminder = CreateReminder(eventRef, _wantToBuyCommentMemoEdit.Text);
                reminder.ViewReminder = FoReminders.GetDbRefFromEnum(FoRemindersEnum.Sale);
                entitiesList.Add(reminder);
            }
            if (_problem)
            {
                var reminder = CreateReminder(eventRef, _problemCommentMemoEdit.Text);
                reminder.ViewReminder = FoReminders.GetDbRefFromEnum(FoRemindersEnum.Problem);
                entitiesList.Add(reminder);
            }

            if (!string.IsNullOrEmpty(_commentaryMemoEdit.Text))
                @event.CommentContractor = _commentaryMemoEdit.Text;

            if (!_eventResults.Id.EmptyRef())
                @event.EventResult = _eventResults.Id;

            if (!string.IsNullOrEmpty(@event.CommentContractor) || !@event.EventResult.EmptyRef())
                entitiesList.Add(@event);

            DBHelper.SaveEntities(entitiesList);

            _eventResults = null;
            _commentaryString = _wantToBuyString = _problemCommentString = null;

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

        internal void CloseEvent_OnClick(object sender, EventArgs e)
          => Navigation.ModalMove(nameof(CompleteResultScreen));

        internal void Comment_OnChange(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(((MemoEdit)sender).Text))
            {
                _commentaryString = ((MemoEdit)sender).Text;
            }

            Utils.TraceMessage($"{_commentaryString}");
        }

        internal void WantToBuy_OnChange(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(((MemoEdit)sender).Text))
            {
                _wantToBuyString = ((MemoEdit)sender).Text;
            }
            Utils.TraceMessage($"{_wantToBuyString}");
        }

        internal void ProblemComment_OnChange(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(((MemoEdit)sender).Text))
            {
                _problemCommentString = ((MemoEdit)sender).Text;
            }
            Utils.TraceMessage($"{_problemCommentString}");
        }
    }
}