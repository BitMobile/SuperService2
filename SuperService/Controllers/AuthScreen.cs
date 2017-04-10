using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using ClientModel3.MD;
using System;
using BitMobile.Common.Application;

namespace Test
{
    public class AuthScreen : Screen
    {
        private static EditText _loginEditText;
        private static EditText _passwordEditText;
        private static Button _enterButton;
        private static Indicator _indicator;
        private static bool _isEnable;

        public override void OnLoading()
        {
            base.OnLoading();
            _isEnable = true;
            DConsole.WriteLine("AuthScreen init");

            _loginEditText = (EditText)GetControl("AuthScreenLoginET", true);
            _passwordEditText = (EditText)GetControl("AuthScreenPasswordET", true);
            _enterButton = (Button)GetControl("ba603e1782d543f696944a603d7f05f2", true);
            _indicator = (Indicator)GetControl("971cb7290f7943ea9bf500e63097b04d", true);
        }

        public override void OnShow()
        {
            base.OnShow();
            _loginEditText.Text =  Settings.User;
            _passwordEditText.Text =  Settings.Password;
        }

        public override void OnDraw()
        {
            base.OnDraw();
            Dialog.HideProgressDialog();
        }

        //TODO: Кнопка временно отключена, так как пока невозможно реализовать её функционал.
        internal void CantSigningButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void СonnectButton_OnClick(object sender, EventArgs e)
        {
            if (!_isEnable) return;

            Utils.TraceMessage(
                $"{nameof(PushNotification)}.{nameof(PushNotification.IsInitialized)} -> {PushNotification.IsInitialized}");
            if (string.IsNullOrEmpty(_loginEditText.Text)
                && string.IsNullOrEmpty(_passwordEditText.Text))
                Toast.MakeToast(Translator.Translate("user_pass_empty"));
            else if (string.IsNullOrEmpty(_loginEditText.Text))
                Toast.MakeToast(Translator.Translate("user_empty"));
            else if (string.IsNullOrEmpty(_passwordEditText.Text))
                Toast.MakeToast(Translator.Translate("password_empty"));
            else
                Authorization.StartAuthorization(_loginEditText.Text, _passwordEditText.Text);
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        public static void ClearPassword()
        {
            if (_passwordEditText != null)
                _passwordEditText.Text = "";
        }

        public static void EditableVisualElements(bool edit)
        {
            _isEnable = edit;

            if (_loginEditText != null)
                _loginEditText.Enabled = edit;

            if (_passwordEditText != null)
                _passwordEditText.Enabled = edit;

            if (_indicator != null)
            {
                _indicator.Visible = !edit;

                if (!edit)
                    _indicator.Start();
                else
                    _indicator.Stop();
            }

            if (_enterButton != null)
                _enterButton.Enabled = edit;
        }

        public string GetPlatformRoundButtonStyle()
        {
            switch (Application.TargetPlatform)
            {
                case TargetPlatform.Android:
                    return "AuthScreenConnectBTNAndroid";
                case TargetPlatform.iOS:
                    return "AuthScreenConnectBTNiOS";
                default:
                    return "Button";
            }
        }
    }
}