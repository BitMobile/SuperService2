using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using Test.Document;
using DbRecordset = BitMobile.ClientModel3.DbRecordset;

namespace Test
{
    public class EditServicesOrMaterialsScreen : Screen
    {
        private int _amountFact;
        private EditText _amountFactEditText;
        private BehaviourEditServicesOrMaterialsScreen _behaviourEditServicesOrMaterialsScreen;
        private bool _isMaterialRequest; //признак того, что запрос пришел из рюкзака монтажника
        private bool _isService; //отображать услуги в противном случае материалы
        private string _key;
        private string _lineId;
        private int _minimum;

        private decimal _price;

        private EditText _priceEditText;
        private string _rimId;

        private TextView _totalPriceTextView;
        private bool _usedCalculateMaterials;
        private bool _usedCalculateService;
        private int _value;
        private Image _minusImage;
        private bool _isEdit;

        private decimal Price
        {
            get { return _price; }
            set
            {
                value = Math.Max(value, 0);
                _price = value;
                if (_priceEditText != null) _priceEditText.Text = GetPriceDescription();
                if (_totalPriceTextView != null)
                    _totalPriceTextView.Text = GetTotalPriceDescription();
            }
        }

        private int AmountFact
        {
            get { return _amountFact; }
            set
            {
                value = Math.Max(value, _minimum);
                if (_minusImage != null)
                {
                    _minusImage.Source =
                        ResourceManager.GetImage(value == _minimum
                            ? "editservicesormaterialsscreen_minusdisabled"
                            : "editservicesormaterialsscreen_minus");
                }
                _amountFact = value;
                if (_amountFactEditText != null) _amountFactEditText.Text = value.ToString();
                if (_totalPriceTextView != null)
                    _totalPriceTextView.Text = GetTotalPriceDescription();
            }
        }

        internal string GetPriceDescription()
        {
            if (_isMaterialRequest || (_isService && !_usedCalculateService) ||
                (!_isService && !_usedCalculateMaterials))
            {
                return Parameters.EmptyPriceDescription;
            }
            return Price.ToString(CultureInfo.CurrentCulture);
        }

        internal string GetTotalPriceDescription()
        {
            if (_isMaterialRequest || (_isService && !_usedCalculateService) ||
                (!_isService && !_usedCalculateMaterials))
            {
                return Parameters.EmptyPriceDescription;
            }
            return (Price * AmountFact).ToString(CultureInfo.CurrentCulture);
        }

        public override void OnLoading()
        {
            _behaviourEditServicesOrMaterialsScreen =
                (BehaviourEditServicesOrMaterialsScreen)
                    Variables.GetValueOrDefault(Parameters.IdBehaviour, BehaviourEditServicesOrMaterialsScreen.None);
            _isMaterialRequest = (bool)Variables.GetValueOrDefault(Parameters.IdIsMaterialsRequest, false);
            _isService = (bool)Variables.GetValueOrDefault(Parameters.IdIsService, false);

            _usedCalculateService = Settings.ShowServicePrice;
            _usedCalculateMaterials = Settings.ShowMaterialPrice;

            _key = (string)Variables.GetValueOrDefault("returnKey", "somNewValue");
            _minimum = (int)Variables.GetValueOrDefault("minimum", 1);
            _rimId = (string)Variables.GetValueOrDefault("rimId");
            _lineId = (string)Variables.GetValueOrDefault(Parameters.IdLineId);
            _value = (int)Variables.GetValueOrDefault("value", 0);

            _amountFactEditText = (EditText)Variables["AmountFactEditText"];
            _priceEditText = (EditText)Variables["PriceEditText"];
            _totalPriceTextView = (TextView)Variables["TotalPriceTextView"];
            _minusImage = (Image)Variables["MinusImage"];
            _isEdit = (bool)Variables.GetValueOrDefault(Parameters.IsEdit, false);

            BusinessProcess.GlobalVariables.Remove(_key);

            var rimDescription = (DbRecordset)Variables["rimDescription"];
            AmountFact = (int)(decimal)rimDescription["AmountFact"];
            Price = (decimal)(float)rimDescription["Price"];
        }

        public override void OnShow()
        {
            Price = Converter.ToDecimal(((DbRecordset)Variables["rimDescription"])["Price"]);

            if (_value > 0)
                _amountFactEditText.Text = $"{_value}";
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
            var value = new EditServiceOrMaterialsScreenResult(AmountFact, Price, AmountFact * Price, _rimId);

            if (BusinessProcess.GlobalVariables.ContainsKey(_key))
                BusinessProcess.GlobalVariables.Remove(_key);
            BusinessProcess.GlobalVariables.Add(_key, value);
        }

        private void UpdateDb()
        {
            var eventServicesMaterials =
                (Event_ServicesMaterials)DbRef.FromString(_lineId).GetObject();
            eventServicesMaterials.Price = Price;
            eventServicesMaterials.AmountFact = AmountFact;
            eventServicesMaterials.SumFact = Price * AmountFact;
            DBHelper.SaveEntity(eventServicesMaterials, false);
        }

        private void InsertIntoDb()
        {
            var @ref = (string)BusinessProcess.GlobalVariables.GetValueOrDefault(Parameters.IdCurrentEventId,
                DbRef.CreateInstance("Document_Event", Guid.Empty).GetString());
            var line = new Event_ServicesMaterials
            {
                Id = DbRef.CreateInstance("Document_Event_ServicesMaterials", Guid.NewGuid()),
                Ref = DbRef.FromString(@ref),
                Price = Price,
                AmountFact = AmountFact,
                SumFact = Price * AmountFact,
                SKU = DbRef.FromString(_rimId),
                LineNumber =
                    DBHelper.GetMaxNumberFromTableInColumn("Document_Event_ServicesMaterials", "LineNumber", "Ref",
                        @ref) + 1
            };

            DBHelper.SaveEntity(line, false);
        }

        internal void RemoveButton_OnClick(object sender, EventArgs eventArgs)
        {
            AmountFact--;
            _minusImage.Refresh();
        }

        internal void AddButton_OnClick(object sender, EventArgs eventArgs)
        {
            AmountFact++;
            _minusImage.Refresh();
        }

        internal void AmountFactEditText_OnLostFocus(object sender, EventArgs eventArgs)
        {
            GetAndCheckCountEditText((EditText)sender);
        }

        internal DbRecordset GetDescriptionByLineId(string lineId)
        {
            return DBHelper.GetServiceMaterialPriceByLineID(lineId);
        }

        internal DbRecordset GetDescriptionByRIMID(string rimId)
        {
            return DBHelper.GetServiceMaterialPriceByRIMID(rimId, (int)Variables.GetValueOrDefault("minimum", 1));
        }

        private void GetAndCheckCountEditText(EditText countEditText)
        {
            int res;
            if (!int.TryParse(countEditText.Text, out res))
            {
                res = AmountFact;
                DConsole.WriteLine($"Unparsed text = {countEditText.Text}");
            }
            if (res < _minimum)
                res = _minimum;

            AmountFact = res;
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void ReturnToThePreviousScreen_OnClick(object sender, EventArgs e)
        {
            if (_isEdit)
                Navigation.Back();
            else
                Navigation.ModalMove(nameof(RIMListScreen),
                    (IDictionary<string, object>)Variables.GetValueOrDefault(Parameters.PreviousScreen));
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