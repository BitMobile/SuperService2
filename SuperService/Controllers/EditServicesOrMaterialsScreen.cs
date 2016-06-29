﻿using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Globalization;

namespace Test
{
    public class EditServicesOrMaterialsScreen : Screen
    {
        private bool _fieldsAreInitialized = false;
        private BehaviourEditServicesOrMaterialsScreen _behaviourEditServicesOrMaterialsScreen;
        private EditText _countEditText;
        private bool _editPrices;
        private string _key;
        private string _lineId;
        private int _minimum;
        private int _value;

        private decimal _price;
        private EditText _priceEditText;
        private string _rimId;

        private bool _showPrices;
        private TextView _totalPriceTextView;

        private decimal Price
        {
            get { return GetAndCheckPriceEditText(_priceEditText); }
            set
            {
                value = Math.Max(value, 0);
                _priceEditText.Text = value.ToString(CultureInfo.CurrentCulture);
            }
        }

        private int Count
        {
            get { return GetAndCheckCountEditText(_countEditText); }
            set
            {
                value = Math.Max(value, _minimum);
                _countEditText.Text = value.ToString();
                if (_totalPriceTextView != null)
                    _totalPriceTextView.Text = (Price * Count).ToString(CultureInfo.CurrentCulture);
            }
        }

        public int InitClassFields()
        {
            if (_fieldsAreInitialized)
            {
                return 0;
            }
            _behaviourEditServicesOrMaterialsScreen =
                    (BehaviourEditServicesOrMaterialsScreen)
                        Variables.GetValueOrDefault(Parameters.IdBehaviour, BehaviourEditServicesOrMaterialsScreen.None);

            _key = (string)Variables.GetValueOrDefault("returnKey", "somNewValue");
            _minimum = (int)Variables.GetValueOrDefault("minimum", 1);
            _showPrices = (bool)Variables.GetValueOrDefault("priceVisible", true);
            _editPrices = (bool)Variables.GetValueOrDefault("priceEditable", false);
            _rimId = (string)Variables.GetValueOrDefault("rimId");
            _lineId = (string)Variables.GetValueOrDefault(Parameters.IdLineId);
            _value = (int)Variables.GetValueOrDefault("value", 0);

            BusinessProcess.GlobalVariables.Remove(_key);

            _fieldsAreInitialized = true;

            return 0;
        }

        public override void OnLoading()
        {
            InitClassFields();

            _countEditText = (EditText)GetControl("CountEditText", true);
            _priceEditText = (EditText)Variables["PriceEditText"];
            _totalPriceTextView = (TextView)GetControl("TotalPriceTextView", true);
        }

        public override void OnShow()
        {
            FindTextViewAndChangeVisibility("PriceTitleTextView", _showPrices);
            FindTextViewAndChangeVisibility("TotalPriceTitleTextView", _showPrices);
            FindTextViewAndChangeVisibility("TotalPriceTextView", _showPrices);

            FindEditTextAndChangeVisibilityAndEditable("PriceEditText", _showPrices, _editPrices);
            Price = _price;

            if (_value > 0)
                _countEditText.Text = $"{_value}";
        }

        private void FindTextViewAndChangeVisibility(string id, bool visibility)
        {
            ((TextView)Variables[id]).Visible = visibility;
        }

        private void FindEditTextAndChangeVisibilityAndEditable(string id, bool visibility, bool editable)
        {
            var et = (EditText)Variables[id];
            et.Visible = visibility;
            et.Enabled = editable;
        }

        internal void BackButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back();
        }

        internal void AddServiceMaterialButton_OnClick(object sender, EventArgs eventArgs)
        {
            switch (_behaviourEditServicesOrMaterialsScreen)
            {
                case BehaviourEditServicesOrMaterialsScreen.InsertIntoDB:
                    DConsole.WriteLine("InsertIntoDB");
                    InsertIntoDb();
                    break;

                case BehaviourEditServicesOrMaterialsScreen.UpdateDB:
                    DConsole.WriteLine("UpdateDB");
                    UpdateDb();
                    break;

                case BehaviourEditServicesOrMaterialsScreen.ReturnValue:
                    DConsole.WriteLine("ReturnValue");
                    ReturnValue();
                    break;
            }
            Navigation.Back();
        }

        private void ReturnValue()
        {
            var value = new EditServiceOrMaterialsScreenResult(Count, Price, Count * Price, _rimId);

            if (BusinessProcess.GlobalVariables.ContainsKey(_key))
                BusinessProcess.GlobalVariables.Remove(_key);
            BusinessProcess.GlobalVariables.Add(_key, value);
        }

        private void UpdateDb()
        {
            //TODO: Переделать на объектную модель когда она будет починена (начнет работать метод GetObject())

            DBHelper.UpdateServiceMaterialAmount(_lineId, Price, Count, Price * Count);
        }

        private void InsertIntoDb()
        {
            //TODO: Переделать на объектную модель когда она будет починена (начнет работать метод GetObject())

            DBHelper.InsertServiceMatherial((string)BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId], _rimId, Price,
                Count, Price * Count);
        }

        internal void RemoveButton_OnClick(object sender, EventArgs eventArgs)
        {
            Count--;
        }

        internal void AddButton_OnClick(object sender, EventArgs eventArgs)
        {
            Count++;
        }

        internal void CountEditText_OnLostFocus(object sender, EventArgs eventArgs)
        {
            GetAndCheckCountEditText((EditText)sender);
        }

        internal int SetPrice(decimal price)
        {
            _price = Convert.ToDecimal(price);
            return 0;
        }

        private int GetAndCheckCountEditText(EditText countEditText)
        {
            int res;
            if (int.TryParse(countEditText.Text, out res))
            {
                res = Convert.ToInt32(res);
                if (res > _minimum) return res;
                countEditText.Text = _minimum.ToString();
                return _minimum;
            }
            DConsole.WriteLine($"Unparsed text = {countEditText.Text}");
            countEditText.Text = _minimum.ToString();
            return _minimum;
        }

        private decimal GetAndCheckPriceEditText(EditText priceEditText)
        {
            // TODO: Разделитель целой и дробной части
            decimal res;
            if (decimal.TryParse(priceEditText.Text, out res))
            {
                res = Convert.ToDecimal(res);
                if (res > _minimum) return res;
                priceEditText.Text = _minimum.ToString();
                return _minimum;
            }
            DConsole.WriteLine($"Unparsed text = {priceEditText.Text}");
            res = new decimal(0, 0, 0, false, 0);
            priceEditText.Text = res.ToString(CultureInfo.CurrentCulture);
            return res;
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal IEnumerable GetServiceMaterialInfo()
        {
            InitClassFields();

            DConsole.WriteLine("rim_id =" + _rimId);

            var res = DBHelper.GetServiceMaterialPriceByRIMID(_rimId);
            res.Next();

            //DConsole.WriteLine("rim_id =" + _rimId)

            return _lineId != null
                ? DBHelper.GetServiceMaterialPriceByLineID(_lineId)
                : DBHelper.GetServiceMaterialPriceByRIMID(_rimId);
        }
    }

    /// <summary>
    ///     Прокидывать через DoAction в экран EditServicesOrMaterialsScreen. Если указан ReturnValue, прокинуть строку с
    ///     ключем returnKey, в глобальные переменные под ключом, равным этой строке запишется результат типа
    ///     EditServiceOrMaterialsScreenResult. Ключ по-умолчанию somNewValue.
    /// </summary>
    public enum BehaviourEditServicesOrMaterialsScreen
    {
        None,
        UpdateDB,
        InsertIntoDB,
        ReturnValue
    }

    public class EditServiceOrMaterialsScreenResult
    {
        public EditServiceOrMaterialsScreenResult(int count, decimal price, decimal fullPrice, string rimId)
        {
            Count = count;
            Price = price;
            FullPrice = fullPrice;
            RimId = rimId;
        }

        public int Count { get; }
        public decimal Price { get; }
        public decimal FullPrice { get; }
        public string RimId { get; }
    }
}