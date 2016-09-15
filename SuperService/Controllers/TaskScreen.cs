using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections.Generic;
using Test.Components;
using Test.Document;
using Test.Enum;

namespace Test
{
    public class TaskScreen : Screen
    {
        private Event_Equipments _equipments;
        private ResultEventEnum _resultEvent;

        private bool _taskCommentTextExpanded;
        private TextView _taskCommentTextView;
        private string _taskResult;
        private TopInfoComponent _topInfoComponent;
        private Image _wrapUnwrapImage;

        private HorizontalLayout _taskFinishedButton;
        private HorizontalLayout _taskRefuseButton;
        private TextView _taskFinishedButtonTextView;
        private TextView _taskRefuseButtonTextView;
        private Image _taskFinishedButtonImage;
        private Image _taskRefuseButtonImage;

        private MemoEdit _taskCommentEditText;

        private DockLayout _rootLayout;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("task"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                ArrowVisible = false
            };

            _taskCommentTextView = (TextView)GetControl("TaskCommentTextView", true);
            _wrapUnwrapImage = (Image)GetControl("WrapUnwrapImage", true);

            _taskFinishedButton = (HorizontalLayout)GetControl("TaskFinishedButton", true);
            _taskRefuseButton = (HorizontalLayout)GetControl("TaskRefuseButton", true);
            _taskFinishedButtonTextView = (TextView)GetControl("TaskFinishedButtonTextView", true);
            _taskRefuseButtonTextView = (TextView)GetControl("TaskRefuseButtonTextView", true);
            _taskFinishedButtonImage = (Image)GetControl("TaskFinishedButtonImage", true);
            _taskRefuseButtonImage = (Image)GetControl("TaskRefuseButtonImage", true);

            _taskCommentEditText = (MemoEdit)GetControl("TaskCommentEditText", true);
            _rootLayout = (DockLayout)Controls[0];
            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
        }

        internal void TaskFinishedButton_OnClick(object sender, EventArgs eventArgs)
        {
            switch (_resultEvent)
            {
                case ResultEventEnum.NotDone:
                case ResultEventEnum.New:
                    ChangeTaskToDone();
                    break;

                case ResultEventEnum.Done:
                    ChangeTaskToNew();
                    break;

                default:
                    throw new ArgumentException("Неправильный результат задания");
            }
        }

        private void ChangeTaskToNew()
        {
            _taskResult = "New";
            _resultEvent = ResultEventEnum.New;
            _taskFinishedButton.CssClass = "FinishedButtonActive";
            _taskFinishedButtonTextView.CssClass = "FinishedButtonActiveText";
            _taskFinishedButtonImage.Source = ResourceManager.GetImage("tasklist_notdone");
            _taskFinishedButton.Refresh();
            _taskRefuseButton.CssClass = "RefuseButton";
            _taskRefuseButtonTextView.CssClass = "RefuseButtonText";
            _taskRefuseButtonImage.Source = ResourceManager.GetImage("tasklist_notdone");
            _taskRefuseButton.Refresh();
        }

        private void ChangeTaskToDone()
        {
            _taskResult = "Done";
            _resultEvent = ResultEventEnum.Done;
            _taskFinishedButton.CssClass = "FinishedButtonPressed";
            _taskFinishedButtonTextView.CssClass = "FinishedButtonPressedText";
            _taskFinishedButtonImage.Source = ResourceManager.GetImage("tasklist_done");
            _taskFinishedButton.Refresh();
            _taskRefuseButton.CssClass = "RefuseButton";
            _taskRefuseButtonTextView.CssClass = "RefuseButtonText";
            _taskRefuseButtonImage.Source = ResourceManager.GetImage("tasklist_notdone");
            _taskRefuseButton.Refresh();
        }

        internal void TaskRefuseButton_OnClick(object sender, EventArgs eventArgs)
        {
            switch (_resultEvent)
            {
                case ResultEventEnum.Done:
                case ResultEventEnum.New:
                    ChangeTaskToNotDone();
                    break;

                case ResultEventEnum.NotDone:
                    ChangeTaskToNew();
                    break;

                default:
                    throw new ArgumentException("Неправильный результат задания");
            }
        }

        private void ChangeTaskToNotDone()
        {
            _taskResult = "NotDone";
            _resultEvent = ResultEventEnum.NotDone;
            _taskFinishedButton.CssClass = "FinishedButtonActive";
            _taskFinishedButtonTextView.CssClass = "FinishedButtonActiveText";
            _taskFinishedButtonImage.Source = ResourceManager.GetImage("tasklist_notdone");
            _taskFinishedButton.Refresh();
            _taskRefuseButton.CssClass = "RefuseButtonPressed";
            _taskRefuseButtonTextView.CssClass = "RefuseButtonPressedText";
            _taskRefuseButtonImage.Source = ResourceManager.GetImage("tasklist_specdone");
            _taskRefuseButton.Refresh();
        }

        internal void WrapUnwrapButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_taskCommentTextExpanded)
            {
                _taskCommentTextView.CssClass = "SubComment";
                _wrapUnwrapImage.Source = ResourceManager.GetImage("longtext_expand");
                _taskCommentTextExpanded = false;
            }
            else
            {
                _taskCommentTextView.CssClass = "SubCommentExpanded";
                _wrapUnwrapImage.Source = ResourceManager.GetImage("longtext_close");
                _taskCommentTextExpanded = true;
            }
            _taskCommentTextView.Refresh();
            _rootLayout.Refresh();
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
            _topInfoComponent.Arrow_OnClick(sender, eventArgs);
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
            DConsole.WriteLine("HOW? WHY?");
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            DConsole.WriteLine($"{_taskResult}");
            _equipments.Comment = _taskCommentEditText.Text;
            _equipments.Result = ResultEvent.GetDbRefFromEnum(_resultEvent);

            DBHelper.SaveEntity(_equipments);

            Navigation.Back();
        }

        internal void EquipmentDescriptionLayout_OnClick(object sender, EventArgs eventArgs)
        {
            // TODO(SUPS-718): Передавать информацию об оборудовании

            var v1 = (VerticalLayout)sender;

            var dictionary = new Dictionary<string, object>()
            {
                {Parameters.IdEquipmentId, v1.Id}
            };

            Navigation.Move("EquipmentScreen", dictionary);
        }

        internal void TaskCommentEditText_OnChange(object sender, EventArgs eventArgs)
        {
        }

        internal object GetTask()
        {
            string currentTaskId = (string)BusinessProcess.GlobalVariables["currentTaskId"];
            _equipments = DBHelper.GetEventEquipmentsById(currentTaskId);
            return DBHelper.GetTaskById(currentTaskId);
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal int SetResultName(string resultName)
        {
            _taskResult = resultName;
            switch (resultName)
            {
                case "New":
                    _resultEvent = ResultEventEnum.New;
                    break;

                case "NotDone":
                    _resultEvent = ResultEventEnum.NotDone;
                    break;

                case "Done":
                    _resultEvent = ResultEventEnum.Done;
                    break;

                default:
                    _resultEvent = ResultEventEnum.New;
                    break;
            }
            return 0;
        }
    }
}