using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using ClientModel3.MD;
using System;
using System.Collections.Generic;
using BitMobile.Common.Controls;
using Test.Catalog;
using Test.Components;
using Test.Document;
using DbRecordset = BitMobile.ClientModel3.DbRecordset;

namespace Test
{
    public class DelegateScreen : Screen
    {
        private bool _init = default(bool);
        private bool _isAsTask;
        private TopInfoComponent _topInfoComponent;
        private static String findText;
        private Guid _userId;
        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowVisible = false,
                ArrowActive = false,
                Header = Translator.Translate("userPick"),
                LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")}
            };

           _isAsTask = (bool) Variables.GetValueOrDefault(Parameters.IsAsTask, false);
  
            Utils.TraceMessage($"{_isAsTask}");
           _topInfoComponent.ActivateBackButton();
        }


        private void LoadControls()
        {
        }


        public override void OnShow()
        {
            Utils.TraceMessage($"{nameof(_isAsTask)}: {_isAsTask}");
            LoadControls();
        }

        internal String GetFindText()
        {
            return findText;
        }

        internal void BtnSearch_Click(object sender, EventArgs eventArgs)
        {

            findText = ((EditText) GetControl("position", true)).Text;
            Utils.TraceMessage($"{_isAsTask}");
            if (_isAsTask)
            {
                Navigation.ModalMove(nameof(DelegateScreen), new Dictionary<string, object>
                { {Parameters.IsAsTask,_isAsTask} }, null, ShowAnimationType.Refresh);
            }
            else
            {
                var eventId = (string)Variables[Parameters.IdCurrentEventId];
                Navigation.ModalMove(nameof(DelegateScreen), new Dictionary<string, object>
            { {Parameters.IdCurrentEventId, eventId},{Parameters.IsAsTask,_isAsTask} }, null, ShowAnimationType.Refresh);

            }

        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            findText = null;
            if (_isAsTask)
            {
                BusinessProcess.GlobalVariables.Remove(Parameters.IsAsTask);            
                Navigation.ModalMove(nameof(AddTaskScreen));
        }
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
            => DBHelper.GetUsers(findText);

        internal bool GetCurUserOrNO(DbRef UsId)
        {
            if (!_init)
            {
                _isAsTask = (bool)BusinessProcess.GlobalVariables.GetValueOrDefault(Parameters.IsAsTask, false);
                //BusinessProcess.GlobalVariables.Remove(Parameters.IsAsTask);
                _init = true;
            }

            if (_isAsTask) return true;

            if (_userId == Guid.Empty)
                _userId = Settings.UserDetailedInfo.Id.Guid;

            return _userId != UsId.Guid;
        }

        internal void SelectUser_OnClick(object sender, EventArgs e)
        {
            if (_isAsTask)
            {
                findText = null;
                BusinessProcess.GlobalVariables.Remove(Parameters.IsAsTask);
                Navigation.ModalMove(nameof(AddTaskScreen),
                    new Dictionary<string, object>
                    {{Parameters.IdUserId, ((VerticalLayout) sender).Id}});
            }
            else
            {
                
                try
                {
                    var eventId = (string)Variables[Parameters.IdCurrentEventId];
                    Utils.TraceMessage($"{eventId.GetType()}");
                    var currentEvent = (Event)DBHelper.LoadEntity(eventId);
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
                            findText = null;
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