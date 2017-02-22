using System;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Application;
using BitMobile.Common.Device.Providers;
using BitMobile.Common.FiscalRegistrator;
using Test.Components;

namespace Test
{
    public class FiscalRegistratorSettingsScreen : Screen
    {
        private TextView _connectedStatusTextView;
        private TextView _connectionButtonDescriptionTextView;
        private Image _connectionButtonImage;

        //        ---------Controls---------

        private TextView _dontSendChecksTextView;
        private TextView _dotSendChecksDataTextView;
        private IFiscalRegistratorProvider _fptr;
        private Image _isConnectedImage;
        private VerticalLayout _leftButtonVerticalLayout;
        private bool _readonlyForIos;
        private VerticalLayout _rightButtonVerticalLayout;
        private TabBarComponent _tabBarComponent;
//        -------------------------------------------------------
        public override void OnLoading()
        {
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

            InitControls();
        }

        private void InitControls()
        {
            _dontSendChecksTextView =
                (TextView) GetControl("d93a34ef373b48939b2bf3a588b6e9b1", true);
            _dotSendChecksDataTextView =
                (TextView) GetControl("19931320adbd4fbf9f2bb2f6ae765677", true);
            _connectedStatusTextView =
                (TextView) GetControl("c3ec25a698d140e89086926246db8e2f", true);
            _isConnectedImage =
                (Image) GetControl("ab27dfe251704c82a835a85088e98c2b", true);
            _connectionButtonImage =
                (Image) GetControl("f0f0255fe94b454982a5a79b51d16ecb", true);
            _connectionButtonDescriptionTextView =
                (TextView) GetControl("cde826477edc449faefce13edf3da0ed", true);
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
            GpsTracking.Start();
        }


        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            if (_readonlyForIos)
                return;
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
            if (_readonlyForIos)
                return;

            FptrInstance.Instance.OpenSettings();
        }

        internal string GetFrStatusIcon()
        {
            if (_readonlyForIos)
                return ResourceManager.GetImage("fptr_disconnected");

            return _fptr.CurrentStatus >= 0
                ? ResourceManager.GetImage("fptr_connected")
                : ResourceManager.GetImage("fptr_disconnected");
        }

        internal string GetFrStatusText()
        {
            if (_readonlyForIos)
                return Translator.Translate("fptr_is_not_connected");

            return Translator.Translate(_fptr.CurrentStatus >= 0
                ? "fptr_connected"
                : "fptr_is_not_connected");
        }

        internal string GetStatusIconForConnectedButton()
        {
            if (_readonlyForIos)
                return ResourceManager.GetImage("fptr_pair");

            return ResourceManager.GetImage(_fptr.CurrentStatus >= 0
                ? "fptr_ping_ok"
                : "fptr_pair");
        }

        internal string GetStatusDescriptionForConnectButton()
        {
            if (_readonlyForIos)
                return Translator.Translate("connect");

            return Translator.Translate(_fptr.CurrentStatus >= 0
                ? "ping"
                : "connect");
        }

        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Events_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Events");
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs)
            => _tabBarComponent.Clients_OnClick(sender, eventArgs);

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
            if (_readonlyForIos)
                return;

            Utils.TraceMessage($"{_fptr.CurrentStatus}: {_fptr.CurrentStatus}");

            if (_fptr.CurrentStatus >= 0)
            {
                _fptr.Beep();
                ChangeLayoutStatus();
            }
            else
            {
                _fptr.PutDeviceSettings(_fptr.Settings);
                _fptr.PutDeviceEnabled(true);
                ChangeLayoutStatus();
                Utils.TraceMessage($"{nameof(_fptr.CurrentStatus)}: {_fptr.CurrentStatus}");
            }
        }


        internal void PrintX_OnClick(object sender, EventArgs e)
        {
            if (_readonlyForIos)
                return;

            ChangeLayoutStatus();

            try
            {
                _fptr.PrintX();
            }
            catch (FPTRException exception)
            {
                Toast.MakeToast(exception.Message);
            }
        }

        internal void PrintZ_OnClick(object sender, EventArgs e)
        {
            if (_readonlyForIos)
                return;
            ChangeLayoutStatus();
            
            try
            {
                _fptr.PrintZ();
            }
            catch (FPTRException exception)
            {
                Toast.MakeToast(exception.Message);
            }
        }

        private void ChangeLayoutStatus()
        {
            _connectedStatusTextView.Text =
                GetFrStatusText();

            _isConnectedImage.Source =
                GetFrStatusIcon();

            _connectionButtonImage.Source
                = GetStatusIconForConnectedButton();

            _connectionButtonDescriptionTextView.Text =
                GetStatusDescriptionForConnectButton();
        }
    }
}