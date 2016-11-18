using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using ClientModel3.MD;
using System;
using System.Collections.Generic;
using Test.Catalog;
using Test.Components;
using Test.Document;
using Test.Enum;

namespace Test
{
    public class AddTaskScreen : Screen
    {
        private static Event _event;
        private object _choosedTaskType;
        private object _statusImportance;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowActive = false,
                ArrowVisible = false,
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                Header = Translator.Translate("add_task")
            };

            _event = _event ?? new Event();

            if (_event.Tender == null && Variables.GetValueOrDefault(Parameters.IdTenderId) != null)
                _event.Tender = DbRef.FromString($"{Variables[Parameters.IdTenderId]}");

            _topInfoComponent.ActivateBackButton();

            InitControls();
        }

        public override void OnShow()
        {
            Utils.TraceMessage($"Event: {_event.Id}{Environment.NewLine}" +
                               $"{_event.UserMA}{Environment.NewLine}" +
                               $"{_event.Client}{Environment.NewLine}" +
                               $"Description: {_event.Comment}{Environment.NewLine}" +
                               $"{_event.StartDatePlan}{Environment.NewLine}" +
                               $"{_event.EndDatePlan}{Environment.NewLine}" +
                               $"{_event.KindEvent}{Environment.NewLine}" +
                               $"{_event.Importance}{Environment.NewLine}" +
                               $"{_event.Status}");
        }

        private void InitControls()
        {
            if (!string.IsNullOrEmpty($"{Variables.GetValueOrDefault(Parameters.IdClientId, "")}"))
            {
                var client = (Client)DBHelper.LoadEntity($"{Variables[Parameters.IdClientId]}");
                ((Button)GetControl("9a0421b3c9644f2095ae851b6adae631", true)).Text = client.Description;
                _event.Client = client.Id;
            }
            else if (_event.Client != null)
                ((Button)GetControl("9a0421b3c9644f2095ae851b6adae631", true)).Text =
                    ((Client)_event.Client?.LoadObject()).Description;

            if (!string.IsNullOrEmpty($"{Variables.GetValueOrDefault(Parameters.IdUserId, "")}"))
            {
                var user = (User)DBHelper.LoadEntity($"{Variables[Parameters.IdUserId]}");
                _event.UserMA = user.Id;
                ((Button)GetControl("3d8605b488da4d3db9469ca0a3c890ca", true)).Text = user.Description;
            }
            else
            {
                var user = ((User)_event.UserMA?.LoadObject())?.Description;
                ((Button)GetControl("3d8605b488da4d3db9469ca0a3c890ca", true)).Text =
                    string.IsNullOrEmpty(user) ? Translator.Translate("not_choosed") : user;
            }

            var typeStatus = ((TypesEvents)_event.KindEvent?.LoadObject())?.Description;
            ((Button)GetControl("0e8bcc83cac645debd0df7a2bc7ae59a", true)).Text =
                string.IsNullOrEmpty(typeStatus) ? Translator.Translate("not_choosed") : typeStatus;

            var importance = ((StatusImportance)_event.Importance?.LoadObject())?.Description;
            ((Button)GetControl("f8b670c4dffc477da344e73eba160cfe", true)).Text =
                string.IsNullOrEmpty(importance) ? Translator.Translate("not_choosed") : importance;

            ((Button)GetControl("a61ad431c4a94635a3c8625e5491f380", true)).Text =
                _event.StartDatePlan == DateTime.MinValue
                    ? Translator.Translate("not_choosed")
                    : $"{_event.StartDatePlan:g}";

            ((Button)GetControl("52541d2e3483440f9403a92776f297f7", true)).Text =
                _event.EndDatePlan == DateTime.MinValue
                    ? Translator.Translate("not_choosed")
                    : $"{_event.EndDatePlan:g}";

            ((MemoEdit)GetControl("2776dd7e8c604323a293635d9a0e6c09", true)).Text = _event.DetailedDescription;
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            _event = null;
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

        internal void CreateTask_OnClick(object sender, EventArgs e)
        {
            if (!CheckAllEventData()) return;

            _event.Id = DbRef.CreateInstance("Document_Event", Guid.NewGuid());
            _event.Status = StatusyEvents.GetDbRefFromEnum(StatusyEventsEnum.Appointed);
            _event.Date = DateTime.Now;
            _event.Author = Settings.UserDetailedInfo.Id;

            DBHelper.SaveEntity(_event);
            PushNotification.PushMessage(Translator.Translate("new_task"),
                new[] { $"{_event.UserMA.Guid}" });
            _event = null;

            Navigation.Back();
        }

        internal void SelectTaskType_OnClick(object sender, EventArgs e)
        {
            Dialog.Choose(Translator.Translate("task_type"), DBHelper.GetTaskTypes(),
                _choosedTaskType,
                (sendr, args) =>
                {
                    ((Button)sender).Text = args.Result.Value;
                    _choosedTaskType = args.Result.Key;
                    _event.KindEvent = (DbRef)args.Result.Key;

                    Utils.TraceMessage($"_event.KindEvent-> {_event.KindEvent}");
                });
        }

        //TODO: Ввести проверку на то что EndDatePlan > StartDatePlan
        internal void StartDatePlan_OnClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            Dialog.DateTime("Выберите дату", DateTime.Now, (o, args) =>
            {
                btn.Text = $"{args.Result:g}";
                _event.StartDatePlan = args.Result;

                Utils.TraceMessage($"{nameof(_event)}.{nameof(_event.StartDatePlan)}: " +
                                   $"{_event.StartDatePlan}");
            });
        }

        //TODO: Ввести проверку на то что EndDatePlan > StartDatePlan
        internal void EndDatePlan_OnClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            Dialog.DateTime("Выберите дату", DateTime.Now, (o, args) =>
            {
                btn.Text = $"{args.Result:g}";
                _event.EndDatePlan = args.Result;

                Utils.TraceMessage($"{nameof(_event)}.{nameof(_event.EndDatePlan)}: " +
                                   $"{_event.EndDatePlan}");
            });
        }

        internal void StatusImportance_OnClick(object sender, EventArgs e)
        {
            Dialog.Choose(Translator.Translate("select_importance"), DBHelper.GetStatusImportance(),
                _statusImportance, (o, args) =>
                {
                    ((Button)sender).Text = args.Result.Value;
                    _event.Importance = (DbRef)args.Result.Key;
                    _statusImportance = args.Result.Key;

                    Utils.TraceMessage($"{nameof(_event)}.{nameof(_event.Importance)}-> " +
                                       $"{_event.Importance}");
                });
        }

        internal void AddClient_OnClick(object sender, EventArgs e)
            => Navigation.ModalMove(nameof(ClientListScreen),
                new Dictionary<string, object>
                {{Parameters.IsAsTask, true}});

        internal void AddUser_OnClick(object sender, EventArgs e)
        {
            BusinessProcess.GlobalVariables[Parameters.IsAsTask] = true;
            Navigation.ModalMove(nameof(DelegateScreen),
                new Dictionary<string, object>
                {{Parameters.IsAsTask, true}});
        }

        internal void SaveDescription_OnChange(object sender, EventArgs e)
            => _event.DetailedDescription = ((MemoEdit)sender).Text;

        private static bool CheckAllEventData()
        {
            if (_event.UserMA == null)
            {
                Toast.MakeToast(@"Поле 'Ответсвенный' не может быть пуcтым");
                return false;
            }

            if (_event.Client == null)
            {
                Toast.MakeToast("Поле 'Клиент' не может быть пуcтым");
                return false;
            }

            if (_event.Importance == null)
            {
                Toast.MakeToast("Поле 'Важность' не может быть пуcтым");
                return false;
            }

            if (_event.KindEvent == null)
            {
                Toast.MakeToast("Поле 'Тип задачи' не может быть пуcтым");
                return false;
            }

            return CheckDate() && CheckDescription();
        }

        private static bool CheckDescription()
        {
            if (_event.DetailedDescription?.Length > 1000)
            {
                Toast.MakeToast("Превышен размер описания");
                return false;
            }

            if (!string.IsNullOrEmpty(_event.DetailedDescription)) return true;

            Toast.MakeToast("Поле описание не может быть пустым");
            return false;
        }

        private static bool CheckDate()
        {
            if (_event.StartDatePlan == DateTime.MinValue)
            {
                Toast.MakeToast("Дата начала задачи не может быть пустой");
                return false;
            }

            if (_event.EndDatePlan == DateTime.MinValue)
            {
                Toast.MakeToast("Дата окончания задачи не может быть пустой");
                return false;
            }

            if (_event.StartDatePlan > _event.EndDatePlan && (_event.EndDatePlan != DateTime.MinValue))
            {
                Toast.MakeToast("Дата начала не может быть больше даты завершeния");
                return false;
            }
            return true;
        }
    }
}