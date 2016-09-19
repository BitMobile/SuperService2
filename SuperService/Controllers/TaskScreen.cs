﻿using System;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;
using Test.Document;
using Test.Enum;

namespace Test
{
    public class TaskScreen : Screen
    {
        private StatusTasksEnum _resultTaskStatus;

        private DockLayout _rootLayout;

        private MemoEdit _taskCommentEditText;

        private bool _taskCommentTextExpanded;
        private TextView _taskCommentTextView;

        private HorizontalLayout _taskFinishedButton;
        private Image _taskFinishedButtonImage;
        private TextView _taskFinishedButtonTextView;
        private HorizontalLayout _taskRefuseButton;
        private Image _taskRefuseButtonImage;
        private TextView _taskRefuseButtonTextView;
        private string _taskResult;
        private Task_Status _taskStatus;
        private TopInfoComponent _topInfoComponent;
        private Image _wrapUnwrapImage;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("task"),
                LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")},
                ArrowVisible = false
            };

            _taskCommentTextView = (TextView) GetControl("TaskCommentTextView", true);
            _wrapUnwrapImage = (Image) GetControl("WrapUnwrapImage", true);

            _taskFinishedButton = (HorizontalLayout) GetControl("TaskFinishedButton", true);
            _taskRefuseButton = (HorizontalLayout) GetControl("TaskRefuseButton", true);
            _taskFinishedButtonTextView = (TextView) GetControl("TaskFinishedButtonTextView", true);
            _taskRefuseButtonTextView = (TextView) GetControl("TaskRefuseButtonTextView", true);
            _taskFinishedButtonImage = (Image) GetControl("TaskFinishedButtonImage", true);
            _taskRefuseButtonImage = (Image) GetControl("TaskRefuseButtonImage", true);

            _taskCommentEditText = (MemoEdit) GetControl("TaskCommentEditText", true);
            _rootLayout = (DockLayout) Controls[0];
            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
            Utils.TraceMessage($"{Variables[Parameters.IdTaskId]}");
        }

        internal void TaskFinishedButton_OnClick(object sender, EventArgs eventArgs)
        {
            switch (_resultTaskStatus)
            {
                case StatusTasksEnum.Rejected:
                case StatusTasksEnum.New:
                    ChangeTaskToDone();
                    break;

                case StatusTasksEnum.Done:
                    ChangeTaskToNew();
                    break;

                default:
                    throw new ArgumentException("Неправильный результат задания");
            }
        }

        private void ChangeTaskToNew()
        {
            _taskResult = "New";
            _resultTaskStatus = StatusTasksEnum.New;
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
            _resultTaskStatus = StatusTasksEnum.Done;
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
            switch (_resultTaskStatus)
            {
                case StatusTasksEnum.Done:
                case StatusTasksEnum.New:
                    ChangeTaskToNotRejected();
                    break;

                case StatusTasksEnum.Rejected:
                    ChangeTaskToNew();
                    break;

                default:
                    throw new ArgumentException("Неправильный результат задания");
            }
        }

        private void ChangeTaskToNotRejected()
        {
            _taskResult = "Rejected";
            _resultTaskStatus = StatusTasksEnum.Rejected;
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
            _taskStatus.CommentContractor = _taskCommentEditText.Text;
            _taskStatus.Status = StatusTasks.GetDbRefFromEnum(_resultTaskStatus);

            DBHelper.SaveEntity(_taskStatus, false);

            Navigation.Back();
        }

        internal void EquipmentDescriptionLayout_OnClick(object sender, EventArgs eventArgs)
        {
            // TODO(SUPS-718): Передавать информацию об оборудовании

            var v1 = (VerticalLayout) sender;

            var dictionary = new Dictionary<string, object>
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
            string currentTaskId = $"{Variables[Parameters.IdTaskId]}";
            _taskStatus = DBHelper.GetTaskStatusByTaskId(currentTaskId);
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
                    _resultTaskStatus = StatusTasksEnum.New;
                    break;

                case "Rejected":
                    _resultTaskStatus = StatusTasksEnum.Rejected;
                    break;

                case "Done":
                    _resultTaskStatus = StatusTasksEnum.Done;
                    break;

                default:
                    _resultTaskStatus = StatusTasksEnum.New;
                    break;
            }
            return 0;
        }

        internal string UpperCaseString(object @string) => @string.ToString().ToUpper();

        internal bool IsThereAnyEquipment(object equipment) => equipment != null;

        internal void Equipment_OnClick(object sender, EventArgs e)
        {
            var equipmentId = ((VerticalLayout) sender).Id;
            Navigation.Move(nameof(EquipmentScreen),
                new Dictionary<string, object> {{Parameters.IdEquipmentId, equipmentId}});
        }

        internal DbRecordset GetTaskTargets()
            => DBHelper.GetTaskTargetsByTaskId(Variables[Parameters.IdTaskId]);

        internal void TaskTarget_OnClick(object sender, EventArgs e)
        {
            var hl = (HorizontalLayout) sender;

            var targetStatus = (Image)hl.GetControl("targetStatus", true);

            Utils.TraceMessage($"{targetStatus.Source}");

            var target = (Task_Targets)DBHelper.LoadEntity(hl.Id);

            Utils.TraceMessage($"IsDone: {target.IsDone}");

            if (target.IsDone)
                targetStatus.Source = GetResourceImage("task_target_not_done");
            else
                targetStatus.Source = GetResourceImage("task_target_done");

            target.IsDone = !target.IsDone;

            Utils.TraceMessage($"targetStatus {targetStatus.Source}{Environment.NewLine}" +
                               $"IsDone: {target.IsDone}");

            DBHelper.SaveEntity(target, false);
            targetStatus.Refresh();
        }

        internal string GetCurrentStatus(bool status)
            => status ? GetResourceImage("task_target_done") : GetResourceImage("task_target_not_done");
    }
}