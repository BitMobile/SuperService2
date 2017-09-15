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
        private static EditText _serverEditText;
        private static VerticalLayout _loginBreaker;
        private static VerticalLayout _passwordBreaker;
        private static HorizontalLayout _serverLayout;
        private static VerticalLayout _serverBreakerLayout;
        private static Button _enterButton;
        private static ProgressBar _progressBar;
        private static TextView _progressBarText;
        private static bool _isEnable;

        private static Button _demoEntranceButton;
        private static bool _isStanAlone;

        public override void OnLoading()
        {
            base.OnLoading();
            _isEnable = true;
            DConsole.WriteLine("AuthScreen init");

            _loginEditText = (EditText)GetControl("AuthScreenLoginET", true);
            _passwordEditText = (EditText)GetControl("AuthScreenPasswordET", true);
            _serverEditText = (EditText)GetControl("AuthScreenServerET", true);
            _serverLayout = (HorizontalLayout)GetControl("ServerLayout", true);
            _serverBreakerLayout = (VerticalLayout)GetControl("ServerBreakerLayout", true);
            _enterButton = (Button)GetControl("ba603e1782d543f696944a603d7f05f2", true);
            _loginBreaker = (VerticalLayout)GetControl("LoginBreaker", true);
            _passwordBreaker = (VerticalLayout)GetControl("PasswordBreaker", true);
            _progressBar = (ProgressBar)GetControl("SyncProgress", true);
            _progressBarText = (TextView)GetControl("SyncProgressBarText", true);
            _demoEntranceButton = (Button)GetControl("DemoEntranceButton", true);

            _isStanAlone = Settings.ClientEnviromentType.ToLower() == Parameters.ClientEnviromentStandAlone.ToLower();
            _demoEntranceButton.Visible = !_isStanAlone;
        }

        public override void OnShow()
        {
            base.OnShow();
            _loginEditText.Text = Settings.User;
            _passwordEditText.Text = Settings.Password;

            _serverLayout.Visible = _serverBreakerLayout.Visible = !_isStanAlone;

            var settings = BitMobile.Application.ApplicationContext.Current.Settings.CustomSettings;

            if (settings.ContainsKey(Parameters.UserSolutionName))
                _serverEditText.Text = settings[Parameters.UserSolutionName];
        }

        public override void OnDraw()
        {
            base.OnDraw();
            Dialog.HideProgressDialog();
        }

        //TODO: Кнопка временно отключена, так как пока невозможно реализовать её функционал.
        internal void TryDemoButton_OnClick(object sender, EventArgs e)
        {
            string user = "demo";
            string password = "Demo1";
            string solutionName = "demo";

            SetSolutionSettings(solutionName);

            TryToLogin(user, password);
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
            else if (string.IsNullOrEmpty(_serverEditText.Text) && !_isStanAlone)
                Toast.MakeToast(Translator.Translate("server_empty"));
            else
            {
                SetSolutionSettings(_serverEditText.Text);
                TryToLogin(_loginEditText.Text, _passwordEditText.Text);
            }
        }

        private void SetSolutionSettings(string serverName)
        {
            Settings.SolutionName = serverName;

            var settings = BitMobile.Application.ApplicationContext.Current.Settings.CustomSettings;
            if (settings.ContainsKey(Parameters.UserSolutionName))
            {
                if (settings[Parameters.UserSolutionName] != Settings.SolutionName)
                {
                    settings[Parameters.UserSolutionName] = Settings.SolutionName;
                    Settings.HasSolutionNameChanged = true;
                }
            }
            else
            {
                BitMobile.Application.ApplicationContext.Current.Settings.CustomSettings.Add(Parameters.UserSolutionName, Settings.SolutionName);
                Settings.HasSolutionNameChanged = true;
            }

            Settings.SetUrlSettings(Settings.SolutionName);
        }

        private void TryToLogin(string userName, string password)
        {
            Authorization.StartAuthorization(userName, password);
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

            if (_serverLayout != null && !_isStanAlone)
                _serverLayout.Visible = edit;

            if (_serverBreakerLayout != null && !_isStanAlone)
                _serverBreakerLayout.Visible = edit;

            if (_loginBreaker != null)
                _loginBreaker.Visible = edit;

            if (_passwordBreaker != null)
                _passwordBreaker.Visible = edit;

            if (_enterButton != null)
                _enterButton.Visible = edit;

            if (_progressBar != null)
            {
                _progressBar.Visible = !edit;
                _progressBar.CssClass = edit ? "NoHeight" : "SyncProgressBar";
                _progressBar.Refresh();
            }

            if (_progressBarText != null)
            {
                _progressBarText.Visible = !edit;
                _progressBarText.CssClass = edit ? "NoHeight" : "SyncProgressBarText";
                _progressBarText.Refresh();
            }

            if (_enterButton != null)
                _enterButton.Enabled = edit;

            if (_demoEntranceButton != null && !_isStanAlone)
                _demoEntranceButton.Visible = edit;
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
