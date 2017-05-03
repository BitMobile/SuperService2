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
        private static VerticalLayout _loginBreaker;
        private static VerticalLayout _passwordBreaker;
        private static Button _enterButton;
        private static ProgressBar _progressBar;
        private static TextView _progressBarText;
        private static bool _isEnable;

        public override void OnLoading()
        {
            base.OnLoading();
            _isEnable = true;
            DConsole.WriteLine("AuthScreen init");

            _loginEditText = (EditText)GetControl("AuthScreenLoginET", true);
            _passwordEditText = (EditText)GetControl("AuthScreenPasswordET", true);
            _enterButton = (Button)GetControl("ba603e1782d543f696944a603d7f05f2", true);
            _loginBreaker = (VerticalLayout)GetControl("LoginBreaker", true);
            _passwordBreaker = (VerticalLayout)GetControl("PasswordBreaker", true);
            _progressBar = (ProgressBar)GetControl("SyncProgress", true);
            _progressBarText = (TextView)GetControl("SyncProgressBarText", true);
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

            HorizontalLayout loginHL = (HorizontalLayout)_loginEditText.Parent;
            HorizontalLayout passwordHL = (HorizontalLayout)_passwordEditText.Parent;

            if (loginHL != null)
                loginHL.Visible = edit;

            if (passwordHL != null)
                passwordHL.Visible = edit;

            if (_loginBreaker != null)
                _loginBreaker.Visible = edit;

            if (_passwordBreaker != null)
                _passwordBreaker.Visible = edit;

            if (_enterButton != null)
                _enterButton.Visible = edit;

            if (_progressBar != null)
                _progressBar.Visible = !edit;

            if (_progressBarText != null)
                _progressBarText.Visible = !edit;

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

        internal static void ProgressChangedCallback(object sender, ResultEventArgs<Database.ProgressArgs> e)
        {
            Application.InvokeOnMainThread(() =>
            {
                float startProgress = 0.0f;
                float endProgress = 100.0f;

                switch (e.Result.ProgressType)
                {
                    case Database.ProgressType.Download:
                        startProgress = 0.0f;
                        endProgress = 20.0f;
                        break;
                    case Database.ProgressType.Unpack:
                        startProgress = 20.0f;
                        endProgress = 25.0f;
                        break;
                    case Database.ProgressType.Save:
                        startProgress = 25.0f;
                        endProgress = 100.0f;
                        break;
                }

                _progressBar.Percent = startProgress + e.Result.ProgressPercentage
                                    * (endProgress - startProgress) * 0.01f;
            });
        }
    }
}