using System;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Application;
using BitMobile.Common.Device.Providers;
using BitMobile.Common.FiscalRegistrator;
using Test.Components;
using Test.Enum;
using Thread = System.Threading.Thread;

namespace Test
{
    public class FiscalRegistratorSettingsScreen : Screen
    {
        private TextView _connectedStatusTextView;
        private TextView _connectionButtonDescriptionTextView;
        private VerticalLayout _connectionButtonImage;

        //        ---------Controls---------

        private TextView _dontSendChecksTextView;
        private TextView _dotSendChecksDataTextView;
        private IFiscalRegistratorProvider _fptr;
        private VerticalLayout _isConnectedImage;
        private VerticalLayout _leftButtonVerticalLayout;
        private bool _readonlyForIos;
        private VerticalLayout _rightButtonVerticalLayout;
        private DockLayout _rootLayout;
        private TabBarComponent _tabBarComponent;
//        -------------------------------------------------------
        public override void OnLoading()
        {
            base.OnLoading();
            _tabBarComponent = new TabBarComponent(this);

            _leftButtonVerticalLayout =
                (VerticalLayout) GetControl("TopInfoLeftButton", true);

            _leftButtonVerticalLayout.AddChild(new Image
            {
                Source = ResourceManager.GetImage("fptr_errorlist")
            });

            _rightButtonVerticalLayout =
                (VerticalLayout) GetControl("TopInfoRightButton", true);

            _rightButtonVerticalLayout.AddChild(new Image
            {
                Source = ResourceManager.GetImage("fptr_settings")
            });

            B672E6Cf63784Ca9A44Eaa6024E0B11B();

            ChangeLayoutsAsync();
        }

        //TODO: бомбануло
        private void B672E6Cf63784Ca9A44Eaa6024E0B11B()
        {
            _dontSendChecksTextView =
                (TextView) GetControl("d93a34ef373b48939b2bf3a588b6e9b1", true);
            _dotSendChecksDataTextView =
                (TextView) GetControl("19931320adbd4fbf9f2bb2f6ae765677", true);
            _connectedStatusTextView =
                (TextView) GetControl("c3ec25a698d140e89086926246db8e2f", true);
            _isConnectedImage =
                (VerticalLayout) GetControl("ab27dfe251704c82a835a85088e98c2b", true);
            _connectionButtonImage =
                (VerticalLayout) GetControl("83841f90385143248d8e865eed46ee3a", true);
            _connectionButtonDescriptionTextView =
                (TextView) GetControl("cde826477edc449faefce13edf3da0ed", true);
            _rootLayout =
                (DockLayout) GetControl("a2dc5f6557284abe92f6d343ade27192", true);
        }

        internal int Init()
        {
            _readonlyForIos = Application.TargetPlatform == TargetPlatform.iOS
                              || Application.TargetPlatform == TargetPlatform.Other;

            if (_readonlyForIos)
                return -1;

            _fptr = FptrInstance.Instance;
            return 0;
        }

        public override void OnShow()
        {
            base.OnShow();
            GpsTracking.StartAsync();

            if (!Settings.EnableFPTR)
            {
                Toast.MakeToast(Translator.Translate("fr_disable"));
                return;
            }
            if (!DBHelper.CheckRole(nameof(WebactionsEnum.MobileFPRAccess)))
            {
                Toast.MakeToast(Translator.Translate("fr_role_disable"));
                return;
            }
            if (_readonlyForIos)
            {
                Toast.MakeToast(Translator.Translate("Функциональность не поддерживается на iOS"));
                return;
            }
        }


        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Utils.TraceMessage("Lclick");

            if (!Settings.EnableFPTR)
            {
                Toast.MakeToast(Translator.Translate("fr_disable"));
                return;
            }
            if (!DBHelper.CheckRole(nameof(WebactionsEnum.MobileFPRAccess)))
            {
                Toast.MakeToast(Translator.Translate("fr_role_disable"));
                return;
            }
            if (_readonlyForIos)
            {
                Toast.MakeToast(Translator.Translate("Функциональность не поддерживается на iOS"));
                return;
            }
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
            Utils.TraceMessage("Rclick");
            if (!Settings.EnableFPTR)
            {
                Toast.MakeToast(Translator.Translate("fr_disable"));
                return;
            }
            if (!DBHelper.CheckRole(nameof(WebactionsEnum.MobileFPRAccess)))
            {
                Toast.MakeToast(Translator.Translate("fr_role_disable"));
                return;
            }
            if (_readonlyForIos)
            {
                Toast.MakeToast(Translator.Translate("Функциональность не поддерживается на iOS"));
                return;
            }

            FptrInstance.Instance.OpenSettings();
        }

        internal void TopInfo_LeftButton_OnPressDown(object sender, EventArgs e)
        {
            ((Image)_leftButtonVerticalLayout.GetControl(0))
                .Source = ResourceManager.GetImage("fptr_errorlist_active");
            _leftButtonVerticalLayout.Refresh();
        }

        internal void TopInfo_LeftButton_OnPressUp(object sender, EventArgs e)
        {
            ((Image)_leftButtonVerticalLayout.GetControl(0))
                .Source = ResourceManager.GetImage("fptr_errorlist");
            _leftButtonVerticalLayout.Refresh();
        }

        internal void TopInfo_RightButton_OnPressDown(object sender, EventArgs e)
        {
            ((Image)_rightButtonVerticalLayout.GetControl(0))
                .Source = ResourceManager.GetImage("fptr_settings_active");
            _rightButtonVerticalLayout.Refresh();
        }

        internal void TopInfo_RightButton_OnPressUp(object sender, EventArgs e)
        {
            ((Image)_rightButtonVerticalLayout.GetControl(0))
                .Source = ResourceManager.GetImage("fptr_settings");
            _rightButtonVerticalLayout.Refresh();
        }

        private string GetFrStatusStyle(int statusCode)
        {
            if (_readonlyForIos)
                return Parameters.FiscalRegistratorScreenDisconnectStyle;

            return statusCode >= 0
                ? Parameters.FiscalRegistratorScreenConnectStyle
                : Parameters.FiscalRegistratorScreenDisconnectStyle;
        }

        private string GetFrStatusText(int statusCode)
        {
            return Translator.Translate(statusCode >= 0
                ? "fptr_connected"
                : "fptr_is_not_connected");
        }

        internal string GetConnectButtonStyle(int statusCode)
        {
            Utils.TraceMessage("GetConnectButtonStylemethod");
            if (_readonlyForIos)
                return Parameters.FiscalRegistratorScreenDisconnectedButtonStyle;

            return statusCode >= 0
                ? Parameters.FiscalRegistratorScreenConnectedButtonStyle
                : Parameters.FiscalRegistratorScreenDisconnectedButtonStyle;
        }

        internal string GetStatusDescriptionForConnectButton(int statusCode)
        {
            if (_readonlyForIos)
                return Translator.Translate("connect");

            return Translator.Translate(statusCode >= 0
                ? "ping"
                : "connect");
        }


        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            Dialog.ShowProgressDialog(Translator.Translate("loading_message"), true);
            _tabBarComponent.Events_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Events");
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            Dialog.ShowProgressDialog(Translator.Translate("loading_message"), true);
            _tabBarComponent.Clients_OnClick(sender, eventArgs);
        }

        internal void TabBarThirdButton_OnClick(object sender, EventArgs eventArgs)
        {
            //_tabBarComponent.FrSettings_OnClick(sender, eventArgs);
        }

        internal void TabBarFourthButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Settings_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Settings");
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void ConnectToFptr_OnClick(object sender, EventArgs e)
        {
            if (!Settings.EnableFPTR)
            {
                Toast.MakeToast(Translator.Translate("fr_disable"));
                return;
            }
            if (!DBHelper.CheckRole(nameof(WebactionsEnum.MobileFPRAccess)))
            {
                Toast.MakeToast(Translator.Translate("fr_role_disable"));
                return;
            }
            if (_readonlyForIos)
            {
                Toast.MakeToast(Translator.Translate("Функциональность не поддерживается на iOS"));
                return;
            }

            Dialog.ShowProgressDialog(Translator.Translate("please_wait"), true);
            TaskFactory.RunTaskWithTimeout(() =>
            {
                try
                {
                    if (_fptr.CurrentStatus >= 0)
                    {
                        _fptr.Beep();
                        ChangeLayoutsAsync();
                    }
                    else
                    {
                        _fptr.PutDeviceSettings(_fptr.Settings);
                        _fptr.PutDeviceEnabled(true);
                        ChangeLayoutsAsync();
                        Utils.TraceMessage($"{nameof(_fptr.CurrentStatus)}: {_fptr.CurrentStatus}");
                    }
                }
                catch (FPTRException fptrException)
                {
                    Utils.TraceMessage($"ExceptionMessage: {fptrException.Message}" +
                                       $" ResultCode: {fptrException.Result}");

                    Application.InvokeOnMainThread(() => Toast.MakeToast(fptrException.Message));
                }
            }, FptrAction.DefaultTimeOut, result =>
            {
                
                if (result.Finished)
                {
                    Dialog.HideProgressDialog();
                    return;
                }
                Utils.TraceMessage($"Change status to offline");
                Application.InvokeOnMainThread(() => ChangeLayoutStatus(-1));

                Dialog.HideProgressDialog();

                Application.InvokeOnMainThread(()
                    => Toast.MakeToast(Translator.Translate("сonnection_error")));
            });
        }

        internal void ConnectToFptr_OnPressDown(object sender, EventArgs e)
        {
            _connectionButtonImage.CssClass = AddPressedStyle(_connectionButtonImage.CssClass);
            _connectionButtonImage.Refresh();
        }

        internal void ConnectToFptr_OnPressUp(object sender, EventArgs e)
        {
            _connectionButtonImage.CssClass = RemovePressedStyle(_connectionButtonImage.CssClass);
            _connectionButtonImage.Refresh();
        }

        internal void PrintX_OnClick(object sender, EventArgs e)
        {
            if (!Settings.EnableFPTR)
            {
                Toast.MakeToast(Translator.Translate("fr_disable"));
                return;
            }
            if (!DBHelper.CheckRole(nameof(WebactionsEnum.MobileFPRAccess)))
            {
                Toast.MakeToast(Translator.Translate("fr_role_disable"));
                return;
            }
            if (_readonlyForIos)
            {
                Toast.MakeToast(Translator.Translate("Функциональность не поддерживается на iOS"));
                return;
            }

            Dialog.ShowProgressDialog(Translator.Translate("please_wait"), true);

            TaskFactory.RunTaskWithTimeout(() =>
            {
                try
                {
                    _fptr.PrintX();
                }
                catch (FPTRException exception)
                {
                    Toast.MakeToast(exception.Message);
                }
            }, FptrAction.PrintingTimeOut, result =>
            {
                if (result.Finished)
                {
                    Application.InvokeOnMainThread(() => ChangeLayoutsAsync());
                    Dialog.HideProgressDialog();
                    return;
                }

                Application.InvokeOnMainThread(() => ChangeLayoutStatus(-1));
                Dialog.HideProgressDialog();

                Application.InvokeOnMainThread(()
                    => Toast.MakeToast(Translator.Translate("сonnection_error")));
            });
        }

        internal void PrintX_OnPressDown(object sender, EventArgs e)
        {
            VerticalLayout VL = (VerticalLayout)((HorizontalLayout)sender).GetControl(0);
            VL.CssClass = AddPressedStyle(VL.CssClass);
            VL.Refresh();
        }

        internal void PrintX_OnPressUp(object sender, EventArgs e)
        {
            VerticalLayout VL = (VerticalLayout)((HorizontalLayout)sender).GetControl(0);
            VL.CssClass = RemovePressedStyle(VL.CssClass);
            VL.Refresh();
        }

        internal void PrintZ_OnClick(object sender, EventArgs e)
        {
            if (!Settings.EnableFPTR)
            {
                Toast.MakeToast(Translator.Translate("fr_disable"));
                return;
            }
            if (!DBHelper.CheckRole(nameof(WebactionsEnum.MobileFPRAccess)))
            {
                Toast.MakeToast(Translator.Translate("fr_role_disable"));
                return;
            }
            if (_readonlyForIos)
            {
                Toast.MakeToast(Translator.Translate("Функциональность не поддерживается на iOS"));
                return;
            }


            Dialog.Ask(Translator.Translate("printZ_caption_ask"), (o, args) =>
            {
                if (args.Result == Dialog.Result.No)
                    return;

                Dialog.ShowProgressDialog(Translator.Translate("please_wait"), true);

                TaskFactory.RunTaskWithTimeout(() =>
                {
                    try
                    {
                        _fptr.PrintZ();
                    }
                    catch (FPTRException exception)
                    {
                        Toast.MakeToast(exception.Message);
                    }
                }, FptrAction.PrintingTimeOut, result =>
                {
                    if (result.Finished)
                    {
                        Dialog.HideProgressDialog();
                        return;
                    }

                    Application.InvokeOnMainThread(() => ChangeLayoutStatus(-1));

                    Dialog.HideProgressDialog();

                    Application.InvokeOnMainThread(()
                        => Toast.MakeToast(Translator.Translate("сonnection_error")));
                });
            });
        }

        internal void PrintZ_OnPressDown(object sender, EventArgs e)
        {
            VerticalLayout VL = (VerticalLayout)((HorizontalLayout)sender).GetControl(0);
            VL.CssClass = AddPressedStyle(VL.CssClass);
            VL.Refresh();
        }

        internal void PrintZ_OnPressUp(object sender, EventArgs e)
        {
            VerticalLayout VL = (VerticalLayout)((HorizontalLayout)sender).GetControl(0);
            VL.CssClass = RemovePressedStyle(VL.CssClass);
            VL.Refresh();
        }

        private void ChangeLayoutStatus(int statusCode)
        {
            _connectedStatusTextView.Text =
                GetFrStatusText(statusCode);

            _isConnectedImage.CssClass =
                GetFrStatusStyle(statusCode);

            _connectionButtonImage.CssClass
                = GetConnectButtonStyle(statusCode);

            _connectionButtonDescriptionTextView.Text =
                GetStatusDescriptionForConnectButton(statusCode);

            ChangeTopInfoTextViews(statusCode);

            _rootLayout.Refresh();
        }


        private int GetFptrIsNotSentChecks()
        {
            _fptr.PutRegisterNumber(44);
            var result = _fptr.Register;

            Utils.TraceMessage($"Dont Send {result}");

            return result;
        }

        private string GetDontSentChecksFormat(int statusCode)
        {
            if (_readonlyForIos) return string.Empty;
            if (_fptr.CurrentStatus >= 0)
            {
                var result = GetFptrIsNotSentChecks();
                if (result > 0)
                    return Translator.Translate("dont_sent_checks")
                           + $" {result}";
                return string.Empty;
            }
            return string.Empty;
        }

        private string FormatDate(int statusCode)
        {
            if (_readonlyForIos) return string.Empty;
            if (_fptr.CurrentStatus >= 0
                && GetFptrIsNotSentChecks() > 0)
                return "с " + DateTime.Now.ToString("HH:mm dd MMMM");
            return string.Empty;
        }

        private void ChangeTopInfoTextViews(int statusCode)
        {
            _dontSendChecksTextView.Text =
                GetDontSentChecksFormat(statusCode);
            _dotSendChecksDataTextView.Text =
                FormatDate(statusCode);
        }

        private void ChangeLayoutsAsync()
        {
            if (!Settings.EnableFPTR)
            {
                Toast.MakeToast(Translator.Translate("fr_disable"));
                Application.InvokeOnMainThread(() => ChangeLayoutStatus(-1));
                return;
            }
            if (!DBHelper.CheckRole(nameof(WebactionsEnum.MobileFPRAccess)))
            {
                Toast.MakeToast(Translator.Translate("fr_role_disable"));
                Application.InvokeOnMainThread(() => ChangeLayoutStatus(-1));
                return;
            }
            if (_readonlyForIos)
            {
                Toast.MakeToast(Translator.Translate("Функциональность не поддерживается на iOS"));
                Application.InvokeOnMainThread(() => ChangeLayoutStatus(-1));
                return;
            }

            TaskFactory.RunTaskWithTimeout(() => new TaskCompletionResult(FptrInstance.Instance.CurrentStatus)
                , FptrAction.DefaultTimeOut, result =>
                {
                    if (!result.Finished)
                    {
                        Application.InvokeOnMainThread(() => ChangeLayoutStatus(-1));
                        return;
                    }

                    Application.InvokeOnMainThread(() => ChangeLayoutStatus((int) result.Result));
                });
        }

        private string AddPressedStyle(string cssClass)
        {
            string result = cssClass;
            if (!cssClass.EndsWith("_Pressed"))
            {
                result += "_Pressed";
            }
            return result;
        }

        private string RemovePressedStyle(string cssClass)
        {
            string result = cssClass;
            if (cssClass.EndsWith("_Pressed"))
            {
                int index = cssClass.LastIndexOf("_Pressed");
                result = cssClass.Substring(0, index);
            }
            return result;
        }
    }
}