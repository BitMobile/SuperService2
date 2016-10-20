using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using ClientModel3.MD;
using System;
using BitMobile.DbEngine;
using Test.Catalog;
using Test.Components;
using Test.Document;
using DbRecordset = BitMobile.ClientModel3.DbRecordset;

namespace Test
{
    public class DelegateScreen : Screen
    {
        private bool IsAsTask;
        private TopInfoComponent _topInfoComponent;

        internal bool AddTask()
            => IsAsTask = (bool)Variables.GetValueOrDefault(Parameters.IsAsTask, false);
        
            public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowVisible = false,
                ArrowActive = false,
                Header = Translator.Translate("delegate"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") }
            };
            
            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
            Utils.TraceMessage($"Type: {Variables[Parameters.IdCurrentEventId].GetType().FullName}" +
                               $"{Environment.NewLine} Id: {Variables[Parameters.IdCurrentEventId]}");
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

        internal DbRecordset GetUsers()
            => DBHelper.GetUsers();

        internal bool GetCurUserOrNO(DbRef UsId)
        {
            return Settings.UserId != $"{UsId.Guid}";
        }
        internal void SelectUser_OnClick(object sender, EventArgs e)
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
                        PushNotification.PushMessage(Translator.Translate("assign_user"), new[] {$"{user.Id.Guid}"});
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