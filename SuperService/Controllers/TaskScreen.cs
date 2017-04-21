using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Components;
using Test.Document;
using Test.Enum;
using DbRecordset = BitMobile.ClientModel3.DbRecordset;

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
        private bool _isReadOnly;
        private DbRecordset _currentEvent;
        private DbRef _userId;

        public override void OnLoading()
        {
            base.OnLoading();
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
            _rootLayout = (DockLayout)GetControl(0);
            _topInfoComponent.ActivateBackButton();

            _isReadOnly = (bool)Variables[Parameters.IdIsReadonly];
            _currentEvent = DBHelper.GetEventByID($"{Variables[Parameters.IdCurrentEventId]}");
            _userId = (DbRef)DBHelper.GetUserInfoByUserName(Settings.User)["Id"];
        }

        public override void OnShow()
        {
            base.OnShow();
            Utils.TraceMessage($"Task Id {Variables[Parameters.IdTaskId]}{Environment.NewLine}" +
                               $"Event Id {Variables[Parameters.IdCurrentEventId]}{Environment.NewLine}" +
                               $"Client Id {Variables[Parameters.IdClientId]}{Environment.NewLine}" +
                               $"ReadOnly {(bool)Variables[Parameters.IdIsReadonly]}");

            var eventStatus = (string)_currentEvent["statusName"];
            if (_isReadOnly)
                _taskCommentEditText.Enabled = !_isReadOnly;
            else
                _taskCommentEditText.Enabled =
                    !(eventStatus.Equals(EventStatus.Agreed, StringComparison.OrdinalIgnoreCase)
                      || eventStatus.Equals(EventStatus.Accepted, StringComparison.OrdinalIgnoreCase));
        }

        internal void TaskFinishedButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_isReadOnly) return;
            var eventStatus = (string)_currentEvent["statusName"];

            if (eventStatus.Equals(EventStatus.Agreed, StringComparison.OrdinalIgnoreCase)
                || eventStatus.Equals(EventStatus.Accepted, StringComparison.OrdinalIgnoreCase))
            {
                Dialog.Ask(Translator.Translate("start_event"), (o, args) =>
                {
                    if (args.Result != Dialog.Result.Yes) return;
                    ChangeEventStatus();

                    FinishedButtonAction();
                });
            }
            else
                FinishedButtonAction();
        }

        private void FinishedButtonAction()
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
            _taskStatus.ActualEndDate = DateTime.MinValue;
            _taskStatus.CloseEvent = DbRef.CreateInstance(_taskStatus.CloseEvent.TableName, Guid.Empty);
            _taskStatus.UserMA = DbRef.CreateInstance(_taskStatus.UserMA.TableName, Guid.Empty);
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
            _taskStatus.ActualEndDate = DateTime.Now;
            _taskStatus.CloseEvent = (DbRef)_currentEvent["Id"];
            _taskStatus.UserMA = _userId;
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
            if (_isReadOnly) return;
            var eventStatus = (string)_currentEvent["statusName"];

            if (eventStatus.Equals(EventStatus.Agreed, StringComparison.OrdinalIgnoreCase)
                || eventStatus.Equals(EventStatus.Accepted, StringComparison.OrdinalIgnoreCase))
            {
                Dialog.Ask(Translator.Translate("start_event"), (o, args) =>
                {
                    if (args.Result != Dialog.Result.Yes) return;
                    ChangeEventStatus();

                    RefuseButtonAction();
                });
            }
            else
                RefuseButtonAction();
        }

        private void RefuseButtonAction()
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
            _taskStatus.ActualEndDate = DateTime.Now;
            _taskStatus.CloseEvent = (DbRef)_currentEvent["Id"];
            _taskStatus.UserMA = _userId;
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
        }

        internal void TopInfo_RightButton_OnPressUp(object sender, EventArgs e)
        {
        }

        internal void EquipmentDescriptionLayout_OnClick(object sender, EventArgs eventArgs)
        {
            // TODO(SUPS-718): Передавать информацию об оборудовании

            var v1 = (VerticalLayout)sender;

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

        internal string UpperCaseString(object @string) => @string?.ToString().ToUpper();

        internal bool IsThereAnyEquipment(object equipment) => equipment != null;

        internal void Equipment_OnClick(object sender, EventArgs e)
        {
            var equipmentId = ((VerticalLayout)sender).Id;
            Navigation.Move(nameof(EquipmentScreen),
                new Dictionary<string, object> { { Parameters.IdEquipmentId, equipmentId } });
        }

        internal DbRecordset GetTaskTargets()
            => DBHelper.GetTaskTargetsByTaskId(Variables[Parameters.IdTaskId]);

        internal void ChangeTaskTargetStatus_OnClick(object sender, EventArgs e)
        {
            if (_isReadOnly) return;
            var eventStatus = (string)_currentEvent["statusName"];

            if (eventStatus.Equals(EventStatus.Agreed, StringComparison.OrdinalIgnoreCase)
                || eventStatus.Equals(EventStatus.Accepted, StringComparison.OrdinalIgnoreCase))
            {
                Dialog.Ask(Translator.Translate("start_event"), (o, args) =>
                {
                    if (args.Result != Dialog.Result.Yes) return;
                    ChangeEventStatus();

                    TaskTargetStatusAction(sender);
                });
            }
            else
                TaskTargetStatusAction(sender);
        }

        private void TaskTargetStatusAction(object sender)
        {
            var hl = (HorizontalLayout)sender;

            var targetStatus = (Image)hl.GetControl("targetStatus", true);

            Utils.TraceMessage($"Time: {DateTime.Now.ToString("HH:mm:ss:ffff")}" +
                               $"{Environment.NewLine}" +
                               $"targetStatus.Source = {targetStatus.Source}");

            var target = (Task_Targets)DBHelper.LoadEntity(hl.Id);

            Utils.TraceMessage($"Time: {DateTime.Now.ToString("HH:mm:ss:ffff")}" +
                               $"{Environment.NewLine}IsDone: {target.IsDone}");

            targetStatus.Source = GetResourceImage(target.IsDone ? "task_target_not_done" : "task_target_done");

            target.IsDone = !target.IsDone;

            Utils.TraceMessage($"Time: {DateTime.Now.ToString("HH:mm:ss:ffff")}" +
                               $"{Environment.NewLine}targetStatus.Source = {targetStatus.Source}" +
                               $"{Environment.NewLine}IsDone: {target.IsDone}");

            DBHelper.SaveEntity(target, false);
            targetStatus.Refresh();
        }

        internal string GetCurrentStatus(bool status)
        {
            var result = status ? GetResourceImage("task_target_done")
                 : GetResourceImage("task_target_not_done");
            Utils.TraceMessage($"Time: {DateTime.Now.ToString("HH:mm:ss:ffff")}" +
                               $"{Environment.NewLine}In XML Target Status = {result}");
            return result;
        }

        private void ChangeEventStatus()
        {
            var result = DBHelper.GetCoordinate(TimeRangeCoordinate.DefaultTimeRange);
            var latitude = Converter.ToDouble(result["Latitude"]);
            var longitude = Converter.ToDouble(result["Longitude"]);
            var @event = (Event)DBHelper.LoadEntity($"{Variables[Parameters.IdCurrentEventId]}");
            @event.ActualStartDate = DateTime.Now;
            @event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.InWork);
            @event.LatitudeStart = Converter.ToDecimal(latitude);
            @event.LongitudeStart = Converter.ToDecimal(longitude);
            var enitylist = new ArrayList();
            enitylist.Add(@event);
            enitylist.Add(DBHelper.CreateHistory(@event));
            DBHelper.SaveEntities(enitylist);
            _currentEvent = DBHelper.GetEventByID($"{Variables[Parameters.IdCurrentEventId]}");
            _taskCommentEditText.Enabled = true;

            var rimList = DBHelper.GetServicesAndMaterialsByEventId($"{Variables[Parameters.IdCurrentEventId]}");
            var rimArrayList = new ArrayList();
            while (rimList.Next())
            {
                var rim = (Event_ServicesMaterials)((DbRef)rimList["Id"]).GetObject();
                rim.AmountFact = rim.AmountPlan;
                rim.SumFact = rim.SumPlan;
                rimArrayList.Add(rim);
            }
            DBHelper.SaveEntities(rimArrayList, false);
        }

        internal void CheckStartEvent_OnGetFocus(object sender, EventArgs e)
        {
        }
    }
}