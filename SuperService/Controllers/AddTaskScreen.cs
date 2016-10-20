using BitMobile.BusinessProcess.ClientModel;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using System.Collections.Generic;
using Test.Catalog;
using Test.Components;
using Test.Document;
using Dialog = BitMobile.ClientModel3.Dialog;

namespace Test
{
    public class AddTaskScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;
        private Event _event;
        private object _choosedTaskType;
        private object _statusImportance;
        private Client _client;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowActive = false,
                ArrowVisible = false,
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                Header = Translator.Translate("add_task")
            };
            _event = new Event();
            _topInfoComponent.ActivateBackButton();

            if (!string.IsNullOrEmpty($"{Variables.GetValueOrDefault(Parameters.IdClientId, "")}"))
            {
                _client = (Client)DBHelper.LoadEntity($"{Variables[Parameters.IdClientId]}");
                ((Button)GetControl("9a0421b3c9644f2095ae851b6adae631", true)).Text = _client.Description;
                _event.Client = _client.Id;
            }
            else if (_client != null)
            {
                ((Button)GetControl("9a0421b3c9644f2095ae851b6adae631", true)).Text = _client.Description;
                _event.Client = _client.Id;
            }
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

        //TODO: Делать проверку на заполнение обязательных полей
        internal void CreateTask_OnClick(object sender, EventArgs e)
        {
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
            { {Parameters.IsAsTask, true}});
    }
}