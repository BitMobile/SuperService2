using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using Test.Components;
using Test.Document;

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

        private VerticalLayout _lastClickedRequiredIndicatior;

        private string _newGuid;
        private string _pathToImg;

        private bool _readonly;

        // Для списка и даты
        private TextView _textView;

        private TopInfoComponent _topInfoComponent;

        private int _totalAnswered;

        private int _totalRequired;

        public override void OnLoading()
        {
            base.OnLoading();
            DConsole.WriteLine("CheckListScreen init");
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("clist"),
                LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")},
                ArrowVisible = false,
                SubHeader =
                    string.Format(Translator.Translate("mandatory_questions_0_1"), _totalAnswered, _totalRequired)
            };
            _readonly = (bool) Variables.GetValueOrDefault(Parameters.IdIsReadonly, false);
            _topInfoComponent.ActivateBackButton();
        }

        private void UpdateChecklist(string id, string result)
        {
            var checkList = (Event_CheckList) DBHelper.LoadEntity(id);
            checkList.Result = result;
            DBHelper.SaveEntity(checkList, false);
        }

        internal int IncTotalAnswered()
        {
            _totalAnswered++;
            return 0;
        }

        internal int IncTotalRequired()
        {
            _totalRequired++;
            return 0;
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
            _imgToReplace = (Image) ((VerticalLayout) sender).GetControl(0);
            _currentCheckListItemID = ((VerticalLayout) sender).Id;

            if (_imgToReplace.Source.StartsWith("~"))
            {
                Navigation.Move(nameof(PhotoScreen), new Dictionary<string, object>
                {
                    [Parameters.IdImage] = _imgToReplace.Source,
                    [nameof(CheckListScreen)] = _currentCheckListItemID
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
            DConsole.WriteLine("Changing indicator");
            ChangeRequiredIndicatorForDone(_lastClickedRequiredIndicatior);
        }

        // Список
        internal void CheckListValList_OnClick(object sender, EventArgs e)
        {
            if (_readonly) return;
            _currentCheckListItemID = ((VerticalLayout) sender).Id;
            _textView = (TextView) ((VerticalLayout) sender).GetControl(0);

            var tv = GetTextView(sender);
            var startObject = "not_choosed";
            var items = new Dictionary<object, string>
            {
                {"not_choosed", Translator.Translate("not_choosed")}
            };
            var temp = DBHelper.GetActionValuesList(_textView.Id);
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

            //TODO: КОСТЫЛЬ когда в платформе починять работу bool заменить код ниже на вызов ChangeRequiredIndicator(_lastClickedRequiredIndicatior, args.Result.Value != Translator.Translate("not_choosed"));
            if (args.Result.Value != Translator.Translate("not_choosed"))
                ChangeRequiredIndicatorForDone(_lastClickedRequiredIndicatior);
            else
                ChangeRequiredIndicatorForRequired(_lastClickedRequiredIndicatior);
        }

        // Дата
        internal void CheckListDateTime_OnClick(object sender, EventArgs e)
        {
            if (_readonly) return;
            _currentCheckListItemID = ((VerticalLayout) sender).Id;
            _textView = (TextView) ((VerticalLayout) sender).GetControl(0);
            DateTime date;
            var isDate = DateTime.TryParse(_textView.Text, out date);
            date = isDate ? date : DateTime.Now;
            Dialog.DateTime(Translator.Translate("select_date"), date, DateCallback);
        }

        internal void DateCallback(object state, ResultEventArgs<DateTime> args)
        {
            _textView.Text = args.Result.Date.ToString("dd MMMM yyyy");
            UpdateChecklist(_currentCheckListItemID, args.Result.ToString("yyyy-MM-dd HH:mm:ss"));
            //ChangeRequiredIndicator(_lastClickedRequiredIndicatior, true);
            //TODO: КОСТЫЛЬ когда в платформе починять работу bool заменить код ниже на вызов ChangeRequiredIndicator(_lastClickedRequiredIndicatior, true);
            ChangeRequiredIndicatorForDone(_lastClickedRequiredIndicatior);
        }

        // Булево
        internal void CheckListBoolean_OnClick(object sender, EventArgs e)
        {
            if (_readonly) return;
            _currentCheckListItemID = ((VerticalLayout) sender).Id;
            _textView = (TextView) ((VerticalLayout) sender).GetControl(0);

            var tv = GetTextView(sender);

            var items = new Dictionary<object, string>
            {
                {"true", Translator.Translate("yes")},
                {"false", Translator.Translate("no")},
                {"not_choosed", Translator.Translate("not_choosed")}
            };
            var startKey = _textView.Text == Translator.Translate("not_choosed")
                ? ""
                : _textView.Text == Translator.Translate("yes") ? "true" : "false";
            Dialog.Choose(tv.Text, items, startKey, BooleanCallback);
        }

        //TODO: Костыль, возможно измениться в будущем.
        //Костыльно. Получаем парента при помощи интерфейса, которого(интерфейс) у нас в SDK у класса нет.
        private TextView GetTextView(object sender)
        {
            var hl = (HorizontalLayout) ((VerticalLayout) sender).Parent;
            var vl = (VerticalLayout) hl.GetControl(hl.ControlsCount < 3 ? 0 : 1);
            var tv = (TextView) vl.GetControl(0);
            return tv;
        }

        internal void BooleanCallback(object state, ResultEventArgs<KeyValuePair<object, string>> args)
        {
            _textView.Text = args.Result.Value;
            UpdateChecklist(_currentCheckListItemID,
                (string) args.Result.Key == "not_choosed" ? "" : (string) args.Result.Key);
            Utils.TraceMessage($"Boleean choose: Key {args.Result.Key} Value: {args.Result.Value}");
            //TODO: КОСТЫЛЬ когда в платформе починять работу bool заменить код ниже на вызов ChangeRequiredIndicator(_lastClickedRequiredIndicatior, args.Result.Value != Translator.Translate("not_choosed"));
            if (args.Result.Value != Translator.Translate("not_choosed"))
                ChangeRequiredIndicatorForDone(_lastClickedRequiredIndicatior);
            else
                ChangeRequiredIndicatorForRequired(_lastClickedRequiredIndicatior);

            _textView.Refresh();
        }

        // С точкой
        internal void CheckListDecimal_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText) sender;
            _currentCheckListItemID = ((EditText) sender).Id;

            UpdateChecklist(_currentCheckListItemID, _editText.Text);

            _lastClickedRequiredIndicatior =
                (VerticalLayout) GetControl(GenerateRequiredIndicatorId(_editText.Id), true);

            //TODO: КОСТЫЛЬ когда в платформе починять работу bool заменить код ниже на вызов  ChangeRequiredIndicator(_lastClickedRequiredIndicatior, string.IsNullOrWhiteSpace(_editText.Text));
            if (!string.IsNullOrWhiteSpace(_editText.Text))
                ChangeRequiredIndicatorForDone(_lastClickedRequiredIndicatior);
            else
                ChangeRequiredIndicatorForRequired(_lastClickedRequiredIndicatior);
        }

        //Целое
        internal void CheckListInteger_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText) sender;
            _currentCheckListItemID = ((EditText) sender).Id;

            UpdateChecklist(_currentCheckListItemID, _editText.Text);

            _lastClickedRequiredIndicatior =
                (VerticalLayout) GetControl(GenerateRequiredIndicatorId(_editText.Id), true);

            //TODO: КОСТЫЛЬ когда в платформе починять работу bool заменить код ниже на вызов  ChangeRequiredIndicator(_lastClickedRequiredIndicatior, string.IsNullOrWhiteSpace(_editText.Text));
            if (!string.IsNullOrWhiteSpace(_editText.Text))
                ChangeRequiredIndicatorForDone(_lastClickedRequiredIndicatior);
            else
                ChangeRequiredIndicatorForRequired(_lastClickedRequiredIndicatior);
        }

        internal void CheckListString_OnGetFocus(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((EditText) sender).Id;
            _lastClickedRequiredIndicatior =
                (VerticalLayout) GetControl(GenerateRequiredIndicatorId(_currentCheckListItemID), true);
        }

        internal void CheckListDecimal_OnGetFocus(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((EditText) sender).Id;
            _lastClickedRequiredIndicatior =
                (VerticalLayout) GetControl(GenerateRequiredIndicatorId(_currentCheckListItemID), true);
        }

        internal void CheckListInteger_OnGetFocus(object sender, EventArgs e)
        {
            _currentCheckListItemID = ((EditText) sender).Id;
            _lastClickedRequiredIndicatior =
                (VerticalLayout) GetControl(GenerateRequiredIndicatorId(_currentCheckListItemID), true);
        }

        // Строка
        internal void CheckListString_OnLostFocus(object sender, EventArgs e)
        {
            _editText = (EditText) sender;
            _currentCheckListItemID = ((EditText) sender).Id;

            UpdateChecklist(_currentCheckListItemID, _editText.Text);

            _lastClickedRequiredIndicatior =
                (VerticalLayout) GetControl(GenerateRequiredIndicatorId(_editText.Id), true);

            //TODO: КОСТЫЛЬ когда в платформе починять работу bool заменить код ниже на вызов  ChangeRequiredIndicator(_lastClickedRequiredIndicatior, string.IsNullOrWhiteSpace(_editText.Text)););
            if (!string.IsNullOrWhiteSpace(_editText.Text))
                ChangeRequiredIndicatorForDone(_lastClickedRequiredIndicatior);
            else
                ChangeRequiredIndicatorForRequired(_lastClickedRequiredIndicatior);
        }

        internal void CheckListElementLayout_OnClick(object sender, EventArgs e)
        {
            var horizontalLayout = (HorizontalLayout) sender;
            _lastClickedRequiredIndicatior = (VerticalLayout) horizontalLayout.GetControl(0);
        }

        //TODO: КОСТЫЛЬ метод оставлен для совместимости. когда-нибудь, когда bool начнет работать как надо он опять начнет использоваться
        // ReSharper disable once UnusedMember.Local
        private void ChangeRequiredIndicator(VerticalLayout requiredIndecator, bool done)
        {
            if (requiredIndecator.CssClass == "CheckListNotRequiredVL")
                return;

            if (requiredIndecator.CssClass == "CheckListRequiredDoneVL" && !done)
                _totalAnswered--;
            if (requiredIndecator.CssClass == "CheckListRequiredVL" && done)
                _totalAnswered++;

            _topInfoComponent.SubHeader = string.Format(Translator.Translate("mandatory_questions_0_1"), _totalAnswered,
                _totalRequired);
            requiredIndecator.CssClass = done ? "CheckListRequiredDoneVL" : "CheckListRequiredVL";
            requiredIndecator.Refresh();
        }

        //TODO: КОСТЫЛЬ: поскольку bool в платформе не работает метод заглушка, если когда-нибудь bool починят - заменить все вызовы на private static void ChangeRequiredIndicator(VerticalLayout requiredIndecator, bool done)
        private void ChangeRequiredIndicatorForDone(VerticalLayout requiredIndecator)
        {
            if (requiredIndecator.CssClass == "CheckListNotRequiredVL")
                return;
            if (requiredIndecator.CssClass == "CheckListRequiredVL")
                _totalAnswered++;
            _topInfoComponent.SubHeader = string.Format(Translator.Translate("mandatory_questions_0_1"), _totalAnswered,
                _totalRequired);
            requiredIndecator.CssClass = "CheckListRequiredDoneVL";
            requiredIndecator.Refresh();
        }

        //TODO: КОСТЫЛЬ: поскольку bool в платформе не работает метод заглушка, если когда-нибудь bool починят - заменить все вызовы на private static void ChangeRequiredIndicator(VerticalLayout requiredIndecator, bool done)
        private void ChangeRequiredIndicatorForRequired(VerticalLayout requiredIndecator)
        {
            if (requiredIndecator.CssClass == "CheckListNotRequiredVL")
                return;
            if (requiredIndecator.CssClass == "CheckListRequiredDoneVL")
                _totalAnswered--;
            _topInfoComponent.SubHeader = string.Format(Translator.Translate("mandatory_questions_0_1"), _totalAnswered,
                _totalRequired);
            requiredIndecator.CssClass = "CheckListRequiredVL";
            requiredIndecator.Refresh();
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

        internal IEnumerable GetCheckList()
        {
            return DBHelper.GetCheckListByEventID((string) BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
        }

        internal void BackButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
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
            return !(bool) Variables[Parameters.IdIsReadonly];
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}