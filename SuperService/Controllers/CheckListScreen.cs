﻿using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Components;

namespace Test
{
    public class CheckListScreen : Screen
    {
        // Для обновления
        private string _currentCheckListItemID;

        // Для целого и строки
        private EditText _editText;

        // Для камеры
        private Image _imgToReplace;

        private string _newGuid;
        private string _pathToImg;

        // Для списка и даты
        private TextView _textView;

        private TopInfoComponent _topInfoComponent;
        private VerticalLayout _lastClickedRequiredIndicatior;

        public override void OnLoading()
        {
            DConsole.WriteLine("CheckListScreen init");
            _topInfoComponent = new TopInfoComponent(this)
            {
                ExtraLayoutVisible = false,
                HeadingTextView = { Text = Translator.Translate("clist") },
                LeftButtonImage = { Source = ResourceManager.GetImage("topheading_back") },
                RightButtonImage = { Visible = false }
            };
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back(true);
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        // Камера
        internal void CheckListSnapshot_OnClick(object sender, EventArgs eventArgs)
        {
            _currentCheckListItemID = ((VerticalLayout)sender).Id;
            _newGuid = Guid.NewGuid().ToString();
            _pathToImg = $@"\private\{_newGuid}.jpg";

            _imgToReplace = (Image)((VerticalLayout)sender).GetControl(0);

            Camera.MakeSnapshot(_pathToImg, int.MaxValue, CameraCallback, sender);
            // TODO: Ожидать фичи получения изображения с памяти устройства
        }

        private void CameraCallback(object state, ResultEventArgs<bool> args)
        {
            //Document.Order order = (Document.Order)state;
            //order.HasPhoto = args.Result;

            _imgToReplace.Source = _pathToImg;
            //_imgToReplace.Refresh();
            ChangeRequiredIndicator(_lastClickedRequiredIndicatior, args.Result);
        }

        // Список
        internal void CheckListValList_OnClick(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((VerticalLayout)sender).Id;
            _textView = (TextView)((VerticalLayout)sender).GetControl(0);
            var items = new Dictionary<object, string>
            {
                {"", Translator.Translate("not_choosed")}
            };
            var temp = DBHelper.GetActionValuesList(_textView.Id);
            while (temp.Next())
            {
                items[temp["Id"].ToString()] = temp["Val"].ToString();
            }
            Dialog.Choose(Translator.Translate("select_answer"), items, ValListCallback);
        }

        private void ValListCallback(object state, ResultEventArgs<KeyValuePair<object, string>> args)
        {
            _textView.Text = args.Result.Value;
            DBHelper.UpdateCheckListItem(_currentCheckListItemID,
                args.Result.Value == Translator.Translate("not_choosed") ? "" : _textView.Text);
            _textView.Refresh();
            ChangeRequiredIndicator(_lastClickedRequiredIndicatior, args.Result.Value == Translator.Translate("not_choosed"));
        }

        // Дата
        internal void CheckListDateTime_OnClick(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((VerticalLayout)sender).Id;
            _textView = (TextView)((VerticalLayout)sender).GetControl(0);

            Dialog.DateTime(@"Выберите дату", DateTime.Now, DateCallback);
        }

        internal void DateCallback(object state, ResultEventArgs<DateTime> args)
        {
            _textView.Text = args.Result.Date.ToString("dd MMMM yyyy");
            DBHelper.UpdateCheckListItem(_currentCheckListItemID, _textView.Text);
            ChangeRequiredIndicator(_lastClickedRequiredIndicatior, true);
        }

        // Булево
        internal void CheckListBoolean_OnClick(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((VerticalLayout)sender).Id;
            _textView = (TextView)((VerticalLayout)sender).GetControl(0);

            var items = new Dictionary<object, string>
            {
                {"true", Translator.Translate("yes")},
                {"false", Translator.Translate("no")},
                {"", Translator.Translate("not_choosed")}
            };

            Dialog.Choose(Translator.Translate("select_answer"), items, BooleanCallback);
        }

        internal void BooleanCallback(object state, ResultEventArgs<KeyValuePair<object, string>> args)
        {
            _textView.Text = args.Result.Value;
            DBHelper.UpdateCheckListItem(_currentCheckListItemID,
                args.Result.Value == Translator.Translate("not_choosed") ? "" : _textView.Text);
            ChangeRequiredIndicator(_lastClickedRequiredIndicatior, args.Result.Value == Translator.Translate("not_choosed"));
            _textView.Refresh();
        }

        // С точкой
        internal void CheckListDecimal_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText)sender;
            _currentCheckListItemID = ((EditText)sender).Id;

            DBHelper.UpdateCheckListItem(_currentCheckListItemID, _editText.Text);
            ChangeRequiredIndicator(_lastClickedRequiredIndicatior, string.IsNullOrWhiteSpace(_editText.Text));
        }

        //Целое
        internal void CheckListInteger_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText)sender;
            _currentCheckListItemID = ((EditText)sender).Id;

            //var vl1 = (IHorizontalLayout3)_editText.Parent;
            //var hl = (IVerticalLayout3)vl1.Parent;
            //var vl2 = (IHorizontalLayout3)hl.Parent;
            //var vltarget = (IVerticalLayout3)vl2.Controls[0];

            //if (_editText.Text.Length > 0)
            //{
            //    vltarget.CssClass = "VLRequiredDone";
            //    vltarget.Refresh();
            //}
            //else
            //{
            //    vltarget.CssClass = "VLRequired";
            //    vltarget.Refresh();
            //}

            DBHelper.UpdateCheckListItem(_currentCheckListItemID, _editText.Text);
            ChangeRequiredIndicator(_lastClickedRequiredIndicatior, string.IsNullOrWhiteSpace(_editText.Text));
        }

        // Строка
        internal void CheckListString_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText)sender;
            _currentCheckListItemID = ((EditText)sender).Id;

            //var vl = (IVerticalLayout3)_editText.Parent;
            //var hl = (IHorizontalLayout3)vl.Parent;
            //var vltarget = (IVerticalLayout3) hl.Controls[0];

            //DConsole.WriteLine("CSS " + vltarget.CssClass.ToString());

            //vltarget.CssClass = "VLRequiredDone";
            //DConsole.WriteLine("1");
            //vltarget.Refresh();

            //DConsole.WriteLine("CSS " + vltarget.CssClass.ToString());

            //if (vltarget.CssClass == "VLRequiredDone" || vltarget.CssClass == "VLRequired")
            //{
            //    DConsole.WriteLine("1");
            //    if (_editText.Text.Length > 0)
            //    {
            //        DConsole.WriteLine("2");
            //        vltarget.CssClass = "VLRequiredDone";
            //        vltarget.Refresh();
            //    }
            //    else
            //    {
            //        DConsole.WriteLine("3");
            //        vltarget.CssClass = "VLRequired";
            //        vltarget.Refresh();
            //    }
            //}
            // TODO: Непонятное поведение Refresh(), из-за чего не можем оперативно сменить индикатор важности. Работает на android 4, не работает на android 6
            DBHelper.UpdateCheckListItem(_currentCheckListItemID, _editText.Text);
            ChangeRequiredIndicator(_lastClickedRequiredIndicatior, string.IsNullOrWhiteSpace(_editText.Text));
        }

        internal void CheckListElementLayout_OnClick(object sender, EventArgs e)
        {
            var horizontalLayout = (HorizontalLayout)sender;
            _lastClickedRequiredIndicatior = (VerticalLayout)horizontalLayout.Controls[0];
        }

        private static void ChangeRequiredIndicator(VerticalLayout requiredIndecator, bool done)
        {
            if (requiredIndecator.CssClass == "CheckListNotRequiredVL")
                return;
            requiredIndecator.CssClass = done ? "CheckListRequiredDoneVL" : "CheckListRequiredVL";
        }

        internal IEnumerable GetCheckList()
        {
            return DBHelper.GetCheckListByEventID((string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
        }

        internal void BackButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back(true);
        }

        internal bool IsNotEmptyString(string item)
        {
            DConsole.WriteLine(item);
            return !(string.IsNullOrEmpty(item) && string.IsNullOrWhiteSpace(item));
        }

        internal bool IsEmptyString(string item)
        {
            DConsole.WriteLine("empty: " + item);
            return string.IsNullOrEmpty(item) && string.IsNullOrWhiteSpace(item);
        }

        internal string ToDate(string datetime)
        {
            DateTime temp;
            return DateTime.TryParse(datetime, out temp)
                ? temp.ToString("dd MMMM yyyy")
                : Translator.Translate("not_specified");
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}