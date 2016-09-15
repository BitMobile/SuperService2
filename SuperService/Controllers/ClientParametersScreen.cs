using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Controls;
using BitMobile.DbEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Test.Catalog;
using Test.Components;

namespace Test
{
    public class ClientParametersScreen : Screen
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

        private bool _readonly;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("client_parameters"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                ArrowVisible = false,
            };
            _topInfoComponent.ActivateBackButton();
            _readonly = (bool)Variables.GetValueOrDefault(Parameters.IdIsReadonly, false);
            _topInfoComponent.ActivateBackButton();
        }

        private static void UpdateChecklist(string id, string result)
        {
            var clientParameters = (Client_Parameters)DBHelper.LoadEntity(id);
            clientParameters.Val = result;
            DBHelper.SaveEntity(clientParameters, false);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal string GenerateRequiredIndicatorId(DbRef id)
        {
            return GenerateRequiredIndicatorId(id.ToString(CultureInfo.CurrentCulture));
        }

        internal string GenerateRequiredIndicatorId(string id)
        {
            return "RVL" + id;
        }

        // Камера
        internal void CheckListSnapshot_OnClick(object sender, EventArgs eventArgs)
        {
            if (_readonly) return;
            _imgToReplace = (Image)((VerticalLayout)sender).GetControl(0);
            _currentCheckListItemID = ((VerticalLayout)sender).Id;

            if (_imgToReplace.Source.StartsWith("~"))
            {
                Navigation.Move(nameof(PhotoScreen), new Dictionary<string, object>
                {
                    [Parameters.IdImage] = _imgToReplace.Source,
                    [nameof(ClientParametersScreen)] = _currentCheckListItemID
                });
            }
            else if (_imgToReplace.Source == ResourceManager.GetImage("checklistscreen_photo"))
            {
                _newGuid = Guid.NewGuid().ToString();
                _pathToImg = $@"\private\{_newGuid}.jpg";

                Camera.MakeSnapshot(_pathToImg, Settings.PictureSize, CameraCallback, _newGuid);
            }
        }

        private void CameraCallback(object state, ResultEventArgs<bool> args)
        {
            if (!args.Result) return;

            DConsole.WriteLine("New image");
            _imgToReplace.Source = "~" + _pathToImg;
            _imgToReplace.Refresh();
            DConsole.WriteLine("Updating");
            UpdateChecklist(_currentCheckListItemID, state.ToString());
        }

        // Список
        internal void CheckListValList_OnClick(object sender, EventArgs e)
        {
            if (_readonly) return;
            _currentCheckListItemID = ((VerticalLayout)sender).Id;
            _textView = (TextView)((VerticalLayout)sender).GetControl(0);

            var tv = GetTextView(sender);
            var startObject = "not_choosed";
            var items = new Dictionary<object, string>
            {
                {"not_choosed", Translator.Translate("not_choosed")}
            };
            var temp = DBHelper.GetClientOptionValuesList(_textView.Id);
            while (temp.Next())
            {
                items[temp["Id"].ToString()] = temp["Val"].ToString();
                if (temp["Val"].ToString() == _textView.Text)
                    startObject = temp["Id"].ToString();
            }
            Dialog.Choose(tv.Text, items, startObject, ValListCallback);
        }

        private void ValListCallback(object state, ResultEventArgs<KeyValuePair<object, string>> args)
        {
            _textView.Text = args.Result.Value;
            UpdateChecklist(_currentCheckListItemID,
                args.Result.Value == Translator.Translate("not_choosed") ? "" : _textView.Text);

            _textView.Refresh();
        }

        // Дата
        internal void CheckListDateTime_OnClick(object sender, EventArgs e)
        {
            if (_readonly) return;
            _currentCheckListItemID = ((VerticalLayout)sender).Id;
            _textView = (TextView)((VerticalLayout)sender).GetControl(0);
            DateTime date;
            var isDate = DateTime.TryParse(_textView.Text, out date);
            date = isDate ? date : DateTime.Now;
            Dialog.DateTime(Translator.Translate("select_date"), date, DateCallback);
        }

        internal void DateCallback(object state, ResultEventArgs<DateTime> args)
        {
            _textView.Text = args.Result.Date.ToString("dd MMMM yyyy");
            UpdateChecklist(_currentCheckListItemID, _textView.Text);
        }

        // Булево
        internal void CheckListBoolean_OnClick(object sender, EventArgs e)
        {
            if (_readonly) return;
            _currentCheckListItemID = ((VerticalLayout)sender).Id;
            _textView = (TextView)((VerticalLayout)sender).GetControl(0);

            var tv = GetTextView(sender);

            var items = new Dictionary<object, string>
            {
                {"true", Translator.Translate("yes")},
                {"false", Translator.Translate("no")},
            };
            var startKey = _textView.Text == Translator.Translate("no") ? "false" : "true";
            Dialog.Choose(tv.Text, items, startKey, BooleanCallback);
        }

        private ITextView3 GetTextView(object sender)
        {
            var hl = (IHorizontalLayout3)((VerticalLayout)sender).Parent;
            var vl = (IVerticalLayout3)hl.Controls[hl.Controls.Length < 3 ? 0 : 1];
            var tv = (ITextView3)vl.Controls[0];
            return tv;
        }

        internal void BooleanCallback(object state, ResultEventArgs<KeyValuePair<object, string>> args)
        {
            _textView.Text = args.Result.Value;
            UpdateChecklist(_currentCheckListItemID,
                args.Result.Value == Translator.Translate("not_choosed") ? "" : _textView.Text);
            _textView.Refresh();
        }

        // С точкой
        internal void CheckListDecimal_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText)sender;
            _currentCheckListItemID = ((EditText)sender).Id;

            UpdateChecklist(_currentCheckListItemID, _editText.Text);
        }

        //Целое
        internal void CheckListInteger_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText)sender;
            _currentCheckListItemID = ((EditText)sender).Id;

            UpdateChecklist(_currentCheckListItemID, _editText.Text);
        }

        internal void CheckListString_OnGetFocus(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((EditText)sender).Id;
        }

        internal void CheckListDecimal_OnGetFocus(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((EditText)sender).Id;
        }

        internal void CheckListInteger_OnGetFocus(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((EditText)sender).Id;
        }

        // Строка
        internal void CheckListString_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText)sender;
            _currentCheckListItemID = ((EditText)sender).Id;

            UpdateChecklist(_currentCheckListItemID, _editText.Text);
        }

        internal void CheckListElementLayout_OnClick(object sender, EventArgs e)
        {
        }

        internal string GetResultImage(string guid)
        {
            return string.IsNullOrEmpty(guid)
                ? ResourceManager.GetImage("checklistscreen_photo")
                : FileSystem.Exists($@"\private\{guid}.jpg")
                    ? $@"~\private\{guid}.jpg"
                    : FileSystem.Exists($@"\shared\{guid}.jpg")
                        ? $@"~\shared\{guid}.jpg"
                        : ResourceManager.GetImage("checklistscreen_nophoto");
        }

        internal IEnumerable GetParameters()
        {
            return DBHelper.GetClientParametersByClientId(Variables[Parameters.IdClientId].ToString());
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

        internal bool IsNotReadonly()
        {
            return true;
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}