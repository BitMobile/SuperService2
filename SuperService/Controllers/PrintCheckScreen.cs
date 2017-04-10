using System;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Device.Providers;
using BitMobile.Common.FiscalRegistrator;
using BitMobile.DbEngine;
using Test.Components;
using Test.Document;

namespace Test
{
    public class PrintCheckScreen : Screen
    {
        private TextView _cashNotEnoughTextView;
        private HorizontalLayout _changeHorizontalLayout;
        private TextView _changeTextView;
        private int _choosedPaymentType;
        private EditText _enteredSumEditText;
        private string _eventId;
        private IFiscalRegistratorProvider _fptr;
        private Dictionary<object, string> _paymentTypes;
        private TextView _paymentTypeTextView;
        private Image _printImage;
        private VerticalLayout _punchButtonLayout;
        private bool _readonly;
        private DockLayout _rootDockLayout;
        private TopInfoComponent _topInfoComponent;
        private decimal _totalSum;
        private bool _wasStarted;

        public override void OnLoading()
        {
            base.OnLoading();
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

            _fptr = FptrInstance.Instance;
            _eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);

            _readonly = (bool) Variables.GetValueOrDefault(Parameters.IdIsReadonly, false);
            _wasStarted = (bool) Variables.GetValueOrDefault(Parameters.IdWasEventStarted, true);
            _enteredSumEditText.Mask = @"^(\+|\-)?\d+([\.\,]\d{0,2})*$";
            _enteredSumEditText.Required = true;
        }

        public override void OnShow()
        {
            base.OnShow();
            try
            {
                _enteredSumEditText.Text = $"{_totalSum}";
                ProcessingPaymentType();
            }
            catch (Exception e)
            {
                Utils.TraceMessage($"{e.Message}");
            }
        }

        internal string GetResourceImage(string tag) => ResourceManager.GetImage(tag);

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
            => Navigation.ModalMove(nameof(CheckInfoScreen), new Dictionary<string, object>
            {
                {Parameters.IdCurrentEventId, _eventId},
                {Parameters.IdIsReadonly, _readonly},
                {Parameters.IdWasEventStarted, _wasStarted}
            });

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_LeftButton_OnPressDown(object sender, EventArgs e)
        {
        }

        internal void TopInfo_RightButton_OnPressDown(object sender, EventArgs e)
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

        internal void FocusEdit_OnClick(object sender, EventArgs e)
        {
            _enteredSumEditText.SetFocus();
        }

        internal void ChoosePaymentType_OnClick(object sender, EventArgs e)
        {
            Dialog.Choose(Translator.Translate("payment_type"), _paymentTypes, $"{_choosedPaymentType}", (o, args) =>
            {
                _choosedPaymentType = int.Parse($"{args.Result.Key}");
                _paymentTypeTextView.Text = args.Result.Value;
                DisableButton();
                ChangeViewState(_choosedPaymentType <= 0);
                _punchButtonLayout.OnClick -= Print_OnClick;
                switch (_choosedPaymentType)
                {
                    case 0:
                        _enteredSumEditText.Enabled = true;
                        _enteredSumEditText.Text = $"{_totalSum}";
                        break;
                    case 1:
                    case 2:
                    case 3:
                        _enteredSumEditText.Text = $"{_totalSum}";
                        _enteredSumEditText.Enabled = false;
                        break;
                }
                ProcessingPaymentType();
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
            ProcessingPaymentType();
        }

        private void ProcessingPaymentType()
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
                ShowControls(false);
                _cashNotEnoughTextView.Text = $"{notEnough:N}";
                _changeTextView.Text = Translator.Translate("change_is_not_calculated");
                _changeTextView.CssClass = "SumIsNotCalculated";
                _rootDockLayout.Refresh();
            }
            else
            {
                ShowControls(true);
                Utils.TraceMessage($"{-notEnough}");
                _changeTextView.Text = $"{-notEnough:N}";
                _changeTextView.CssClass = "SumIsCalculated";
                _rootDockLayout.Refresh();
            }
        }

        private void ProcessCardPaymentType()
        {
            var sum = GetEnteredSum();
            var notEnough = _totalSum - sum;

            if (notEnough == 0m)
                EnableButton();
            else
                DisableButton();
        }

        private void ProcessBonusPaymentType()
        {
            var sum = GetEnteredSum();
            var notEnough = _totalSum - sum;

            if (notEnough == 0m)
                EnableButton();
            else
                DisableButton();
        }

        private void ProcessTaraPaymentType()
        {
            var sum = GetEnteredSum();
            var notEnough = _totalSum - sum;

            if (notEnough == 0m)
                EnableButton();
            else
                DisableButton();
        }

        private decimal GetEnteredSum()
        {
            decimal result;

            var parcingString = _enteredSumEditText.Text.Replace(".", ",");

            if (decimal.TryParse(parcingString, out result))
                return result;

            return decimal.TryParse(_enteredSumEditText.Text, out result) ? result : 0m;
        }

        private void ShowControls(bool isVisible)
        {
            if (isVisible)
            {
                ((HorizontalLine) GetControl("6f931069b5624ed19fc1e705cc0a71b9", true)).Visible = false;
                ((HorizontalLayout) GetControl("0f36a14440114070a5dd337601396244", true)).Visible = false;
                EnableButton();
            }
            else
            {
                ((HorizontalLine) GetControl("6f931069b5624ed19fc1e705cc0a71b9", true)).Visible = true;
                ((HorizontalLayout) GetControl("0f36a14440114070a5dd337601396244", true)).Visible = true;
                DisableButton();
            }
        }

        private void EnableButton()
        {
            _printImage.Source = ResourceManager.GetImage("printcheckscreen_white_printer");
            _punchButtonLayout.CssClass = "PrintCheckContainerActivate";
            ((TextView) GetControl("c5e071d3b31b4133917ecfa793ef9614", true)).CssClass = "PunchActive";
            _rootDockLayout.Refresh();
            _punchButtonLayout.OnClick += Print_OnClick;
        }

        private void DisableButton()
        {
            _printImage.Source = ResourceManager.GetImage("printcheckscreen_white_printer_diactivated");
            _punchButtonLayout.CssClass = "PrintCheckContainerDiactivate";
            ((TextView) GetControl("c5e071d3b31b4133917ecfa793ef9614", true)).CssClass = "PunchDiactive";
            _rootDockLayout.Refresh();
            _punchButtonLayout.OnClick -= Print_OnClick;
        }

        internal void Print_OnClick(object sender, EventArgs e)
        {
            _enteredSumEditText.Enabled = false;

            var checkParameters = new Event_EventFiskalProperties
            {
                Id = DbRef.CreateInstance($"Document_{nameof(Event_EventFiskalProperties)}"
                    , Guid.NewGuid()),
                Ref = DbRef.FromString(_eventId),
                User = Settings.UserDetailedInfo.Id
            };
            Dialog.ShowProgressDialog(Translator.Translate("please_wait"), true);

            TaskFactory.RunTaskWithTimeout(() =>
            {
                var checkError = false;

                try
                {
                    PrintCheck();

                    if (_fptr.CloseCheck() < 0)
                        _fptr.CheckError();

                    checkParameters.Date = DateTime.Now;

                    DBHelper.SaveEntity(checkParameters, false);
                }
                catch (FPTRException exception)
                {
                    Utils.TraceMessage($"Error code {exception.Result} {exception.Message}");
                    checkError = true;
                    Toast.MakeToast(exception.Message);
                }
                catch (Exception exception)
                {
                    Utils.TraceMessage($"{exception.Message}{Environment.NewLine}" +
                                       $"Type {exception.GetType()}");
                }

                Utils.TraceMessage($"Check Error: {checkError}");

                if (!checkError)
                {
                    SaveFptrParameters(checkParameters);
                    BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId] = _eventId;

                    Utils.TraceMessage($"GoTo {nameof(COCScreen)}");
                    Application.InvokeOnMainThread(() =>
                    {
                        Navigation.ModalMove(nameof(COCScreen), new Dictionary<string, object>
                        {
                            {Parameters.IdCurrentEventId, _eventId},
                            {Parameters.IdIsReadonly, _readonly},
                            {Parameters.IdWasEventStarted, _wasStarted}
                        });
                    });
                }
                else
                {
                    try
                    {
                        DBHelper.DeleteByRef(checkParameters.Id, false);
                        _fptr.CancelCheck();
                    }
                    catch (FPTRException exception)
                    {
                        Toast.MakeToast(exception.Message);
                    }
                    finally
                    {
                        if (_choosedPaymentType == 0)
                            Application.InvokeOnMainThread(()
                                => _enteredSumEditText.Enabled = true);
                    }
                }
            }, FptrAction.PrintingTimeOut, result =>
            {
                if (result.Finished)
                {
                    Dialog.HideProgressDialog();
                    return;
                }

                Dialog.HideProgressDialog();
                Toast.MakeToast(Translator.Translate("сonnection_error"));
            });
        }

        private void PrintCheck()
        {
            var query = DBHelper.GetCheckSKU(_eventId);

            var enteredSum = GetEnteredSum();

            _fptr.OpenCheck(FiscalRegistratorConsts.ChequeTypeSell);

            while (query.Next())
            {
                var name = $"{query["Description"]}";
                var price = decimal.ToDouble((decimal) query["Price"]);
                var quantity = decimal.ToDouble((decimal) query["AmountFact"]);
                var vat = int.Parse($"{query["VAT_Number"]}");


                if (price * quantity >0)
                    _fptr.RegistrationFz54(name, price, quantity, price * quantity, vat);
            }

            _fptr.Payment(decimal.ToDouble(enteredSum), _choosedPaymentType);
        }

        private void SaveFptrParameters(Event_EventFiskalProperties checkParameters)
        {
            checkParameters.NumberFtpr = _fptr.GetSerialNumber();
            checkParameters.ShiftNumber = _fptr.GetSession() + 1;
            checkParameters.CheckNumber = _fptr.GetCheckNumber();
            checkParameters.PaymentType = _choosedPaymentType;
            checkParameters.PaymentAmount = GetEnteredSum();
            checkParameters.Date = DateTime.Now;

            Utils.TraceMessage($"{Parameters.Splitter}");

            //            var resultDateTime = _fptr.GetDate();
            //            var time = _fptr.GetTime();
            //            Utils.TraceMessage($"GetDate: {_fptr.GetDate()} GetTime: {_fptr.GetTime()}");
            //            resultDateTime = resultDateTime.AddHours(time.Hour);
            //            resultDateTime = resultDateTime.AddMinutes(time.Minute);
            //            resultDateTime = resultDateTime.AddSeconds(time.Second);

            //            checkParameters.Date = resultDateTime;

            DBHelper.SaveEntity(checkParameters);
        }
    }
}