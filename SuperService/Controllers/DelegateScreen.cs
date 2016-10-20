using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using ClientModel3.MD;
using System;
using System.Collections.Generic;
using Test.Catalog;
using Test.Components;
using Test.Document;
using DbRecordset = BitMobile.ClientModel3.DbRecordset;

namespace Test
{
    public class DelegateScreen : Screen
    {
        private bool _isAsTask;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowVisible = false,
                ArrowActive = false,
                Header = Translator.Translate("delegate"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") }
            };

            _isAsTask = (bool)Variables.GetValueOrDefault(Parameters.IsAsTask, false);

            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_isAsTask)
                Navigation.ModalMove(nameof(AddTaskScreen));
            else
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

        internal DbRecordset GetUsers()
            => DBHelper.GetUsers();

        internal bool GetCurUserOrNO(DbRef UsId)
        {
            return Settings.UserId != $"{UsId.Guid}";
        }

        internal void SelectUser_OnClick(object sender, EventArgs e)
        {
            if (_isAsTask)
            {
                Navigation.ModalMove(nameof(AddTaskScreen),
                    new Dictionary<string, object>
                    { {Parameters.IdUserId, ((VerticalLayout) sender).Id} });
            }
            else
            {
                var eventId = (string)Variables[Parameters.IdCurrentEventId];
                try
                {
                    Utils.TraceMessage($"{eventId.GetType()}");
                    var currentEvent = (Event)DBHelper.LoadEntity(eventId.ToString());
                    var user = (User)DBHelper.LoadEntity(((VerticalLayout)sender).Id);

                    Dialog.Ask(Translator.Translate("assign_on") + " " + user.Description + "?", (o, args) =>
                    {
                        if (args.Result == Dialog.Result.No) return;

                        currentEvent.UserMA = user.Id;
                        DBHelper.SaveEntity(currentEvent);

                        try
                        {
                            PushNotification.PushMessage(Translator.Translate("assign_user"), new[] { $"{user.Id.Guid}" });
                        }
                        catch (Exception exception)
                        {
                            Utils.TraceMessage($"{exception.Message}{Environment.NewLine}" +
                                               $"{exception.StackTrace}");
                        }
                        finally
                        {
                            Navigation.CleanStack();
                            Navigation.ModalMove(nameof(EventListScreen));
                        }
                    });
                }
                catch (Exception exception)
                {
                    Utils.TraceMessage($"{exception.Message}" +
                                       $"{Environment.NewLine} {exception.StackTrace}");
                }
            }
        }
    }
}