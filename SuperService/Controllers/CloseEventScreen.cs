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
        private MemoEdit _commentaryMemoEdit;
        private TextView _closeResult;
        private static EventResults _eventResults;

        private static string _commentaryString;

        public override void OnLoading()
        {
            _commentaryMemoEdit = (MemoEdit)GetControl("CommentaryMemoEdit", true);
            _closeResult = (TextView)GetControl("75097b50211a453883f5e31f4e878c2c", true);

            if (_eventResults == null) _eventResults = new EventResults();

            if (_commentaryString == null)
            {
                Utils.TraceMessage($"Стока {_commentaryString} пустая");
                _commentaryString = string.Empty;
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

            _closeResult.Text = string.IsNullOrEmpty(_eventResults.Description)
                ? Translator.Translate("not_choosed")
                : _eventResults.Description;
        }

        internal void WantToBuyButton_OnClick(object sender, EventArgs eventArgs)
        {
            
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

          

            if (!string.IsNullOrEmpty(_commentaryMemoEdit.Text))
                @event.CommentContractor = _commentaryMemoEdit.Text;

            if (!_eventResults.Id.EmptyRef())
                @event.EventResult = _eventResults.Id;

            if (!string.IsNullOrEmpty(@event.CommentContractor) || !@event.EventResult.EmptyRef())
                entitiesList.Add(@event);

            DBHelper.SaveEntities(entitiesList);

            _eventResults = null;

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
        {
            _commentaryString = _commentaryMemoEdit.Text;
            Navigation.ModalMove(nameof(CompleteResultScreen));
        }

        internal void Comment_OnChange(object sender, EventArgs e)
        {
        }

        internal void WantToBuy_OnChange(object sender, EventArgs e)
        {
        }

        internal void ProblemComment_OnChange(object sender, EventArgs e)
        {
        }
    }
}