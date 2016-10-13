using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using ClientModel3.MD;
using System;
using Test.Catalog;
using Test.Components;
using Test.Document;

namespace Test
{
    public class DelegateScreen : Screen
    {
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

        internal void SelectUser_OnClick(object sender, EventArgs e)
        {
            var eventId = (string)Variables[Parameters.IdCurrentEventId];
            try
            {
                Utils.TraceMessage($"{eventId.GetType()}");
                var currentEvent = (Event)DBHelper.LoadEntity(eventId.ToString());
                var user = (User)DBHelper.LoadEntity(((VerticalLayout)sender).Id);

                Dialog.Ask(Translator.Translate("assign_user"), (o, args) =>
                {
                    if (args.Result == Dialog.Result.No) return;

                    currentEvent.UserMA = user.Id;
                    DBHelper.SaveEntity(currentEvent);
                    Utils.TraceMessage($"GUID: {user.UserID}");

                    try
                    {
                        PushNotification.PushMessage(Translator.Translate("assign_user"), new[] { $"{user.UserID}" });
                    }
                    catch (Exception exception)
                    {
                        Utils.TraceMessage($"{exception.Message}{Environment.NewLine}" +
                                           $"{exception.StackTrace}");
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