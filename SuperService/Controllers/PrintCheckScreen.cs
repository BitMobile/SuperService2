using System;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class PrintCheckScreen : Screen
    {
        private HorizontalLayout _changeHorizontalLayout;
        private int _choosedPaymentType;
        private string _eventId;
        private Dictionary<object, string> _paymentTypes;
        private TextView _paymentTypeTextView;
        private DockLayout _rootDockLayout;
        private TopInfoComponent _topInfoComponent;
        private decimal _totalSum;
        private EditText _enteredSumEditText;
        private TextView _cashNotEnoughTextView;
        private VerticalLayout _punchButtonLayout;
        private Image _printImage;
        private TextView _changeTextView;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("registration_check"),
                LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")},
                ArrowVisible = false
            };

            _topInfoComponent.ActivateBackButton();


            InitFields();
        }

        private void InitFields()
        {
            _choosedPaymentType = 0;

            _paymentTypeTextView = (TextView) GetControl("7c46a01e25b34835b7ff98f6debfeac0", true);
            _rootDockLayout = (DockLayout) GetControl("07ec0239c319491eb406a40e1183d9b5", true);
            _changeHorizontalLayout = (HorizontalLayout) GetControl("c129ed940d97427fa7cd303171370fde", true);
            _enteredSumEditText = (EditText) GetControl("778f105408c745b48d4eab7bff782e72", true);
            _cashNotEnoughTextView = (TextView) GetControl("fde6ae3fe5e946b88a13eb305372e38d", true);
            _punchButtonLayout = (VerticalLayout) GetControl("2551f8ad1b2749d3847581fd124c841b", true);
            _printImage = (Image) GetControl("ecd5c17d8f904d368bb5ef92bae35447", true);
            _changeTextView = (TextView) GetControl("fa4aad30428344f7ac60ca62f721f67a", true);
            _paymentTypes = new Dictionary<object, string>
            {
                {"0", "НАЛИЧНЫЕ"},
                {"1", "КАРТОЙ"},
                {"2", "БОНУСЫ"},
                {"3", "ТАРОЙ"}
            };

            _eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
        }

        internal string GetResourceImage(string tag) => ResourceManager.GetImage(tag);

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e) => Navigation.Back();

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        internal string GetFormatTotalSum()
        {
            _eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
            var totalSum = DBHelper.GetCheckSKUSum(_eventId);
            _totalSum = Converter.ToDecimal(totalSum);
            Utils.TraceMessage($"Total SUm {totalSum}");
            Utils.TraceMessage($"{nameof(_totalSum)}: {_totalSum:N} -> {_totalSum}");

            return $"{_totalSum:N}";
        }

        internal void ChoosePaymentType_OnClick(object sender, EventArgs e)
        {
            Dialog.Choose(Translator.Translate("payment_type"), _paymentTypes, $"{_choosedPaymentType}", (o, args) =>
            {
                _choosedPaymentType = int.Parse($"{args.Result.Key}");
                _paymentTypeTextView.Text = args.Result.Value;
                DisableButton();
                ChangeViewState(_choosedPaymentType != 1);


                _punchButtonLayout.OnClick -= Print_OnClick;
            });
        }

        private void ChangeViewState(bool isVisible)
        {
            _changeHorizontalLayout.Visible = isVisible;
            ((HorizontalLine) GetControl("b6be07680f594b6bbbc8ae137376ddce", true)).Visible = isVisible;
            ((HorizontalLine) GetControl("6f931069b5624ed19fc1e705cc0a71b9", true)).Visible = isVisible;
            ((HorizontalLayout) GetControl("0f36a14440114070a5dd337601396244", true)).Visible = isVisible;
        }

        internal void CheckEnteredSumm_OnChange(object sender, EventArgs e)
        {
            switch (_choosedPaymentType)
            {
                case 0:
                    ProcessPaymentCashType();
                    break;
                case 1:
                    ProcessCardPaymentType();
                    break;
                case 2:
                    ProcessBonusPaymentType();
                    break;
                case 3:
                    ProcessTaraPaymentType();
                    break;
            }
        }


        private void ProcessPaymentCashType()
        {
            var sum = GetEnteredSum();
            var notEnough = _totalSum - sum;
            if (notEnough > 0)
            {
                EnableButton(false);
                _cashNotEnoughTextView.Text = $"{notEnough:N}";
            }
            else
            {
                EnableButton(true);
                Utils.TraceMessage($"{-notEnough}");
                _changeTextView.Text = $"{-notEnough:N}";
            }
        }

        private void ProcessCardPaymentType()
        {
            var sum = GetEnteredSum();
            var notEnough = _totalSum - sum;
            if (notEnough == 0m)
            {
                _printImage.Source = ResourceManager.GetImage("printcheckscreen_white_printer");
                _punchButtonLayout.CssClass = "PrintCheckContainerActivate";
                ((TextView) GetControl("c5e071d3b31b4133917ecfa793ef9614", true)).CssClass = "PunchActive";
                _rootDockLayout.Refresh();
                _punchButtonLayout.OnClick += Print_OnClick;
            }
            else
            {
                _printImage.Source = ResourceManager.GetImage("printcheckscreen_white_printer_diactivated");
                _punchButtonLayout.CssClass = "PrintCheckContainerDiactivate";
                ((TextView)GetControl("c5e071d3b31b4133917ecfa793ef9614", true)).CssClass = "PunchDiactive";
                _rootDockLayout.Refresh();
                _punchButtonLayout.OnClick -= Print_OnClick;
            }

        }

        private void ProcessBonusPaymentType()
        {
        }

        private void ProcessTaraPaymentType()
        {
        }

        private decimal GetEnteredSum()
        {
            decimal result;

            return decimal.TryParse(_enteredSumEditText.Text, out result) ? result : 0m;
        }

        private void EnableButton(bool enable)
        {
            if (enable)
            {
                ((HorizontalLine)GetControl("6f931069b5624ed19fc1e705cc0a71b9", true)).Visible = false;
                ((HorizontalLayout)GetControl("0f36a14440114070a5dd337601396244", true)).Visible = false;
                _printImage.Source = ResourceManager.GetImage("printcheckscreen_white_printer");
                _punchButtonLayout.CssClass = "PrintCheckContainerActivate";
                ((TextView) GetControl("c5e071d3b31b4133917ecfa793ef9614", true)).CssClass = "PunchActive";
                _rootDockLayout.Refresh();
                _punchButtonLayout.OnClick += Print_OnClick;
            }
            else
            {
                ((HorizontalLine)GetControl("6f931069b5624ed19fc1e705cc0a71b9", true)).Visible = true;
                ((HorizontalLayout)GetControl("0f36a14440114070a5dd337601396244", true)).Visible = true;
                _printImage.Source = ResourceManager.GetImage("printcheckscreen_white_printer_diactivated");
                _punchButtonLayout.CssClass = "PrintCheckContainerDiactivate";
                ((TextView)GetControl("c5e071d3b31b4133917ecfa793ef9614", true)).CssClass = "PunchDiactive";
                _rootDockLayout.Refresh();
                _punchButtonLayout.OnClick += Print_OnClick;
            }
        }

        private void DisableButton()
        {
            _printImage.Source = ResourceManager.GetImage("printcheckscreen_white_printer_diactivated");
            _punchButtonLayout.CssClass = "PrintCheckContainerDiactivate";
            ((TextView)GetControl("c5e071d3b31b4133917ecfa793ef9614", true)).CssClass = "PunchActive";
            _rootDockLayout.Refresh();
            _enteredSumEditText.Text = string.Empty;
            _changeTextView.Text = string.Empty;
            _cashNotEnoughTextView.Text = string.Empty;
        }


        internal void Print_OnClick(object sender, EventArgs e)
        {
            Utils.TraceMessage($"PRIFKSDFJKLSDJFJDSKFJHGJKLSDGK");
        }
    }
}