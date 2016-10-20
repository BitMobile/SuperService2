using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using Test.Module;

namespace Test
{
    public class AuthScreen : Screen
    {
        private EditText _loginEditText;
        private EditText _passwordEditText;

        public override void OnLoading()
        {
            DConsole.WriteLine("AuthScreen init");

            _loginEditText = (EditText)GetControl("AuthScreenLoginET", true);
            _passwordEditText = (EditText)GetControl("AuthScreenPasswordET", true);
        }

        public override void OnShow()
        {
            PushServerServices.Init();
            _loginEditText.Text = Settings.User;
            _passwordEditText.Text = Settings.Password;
        }

        //TODO: Кнопка временно отключена, так как пока невозможно реализовать её функционал.
        internal void CantSigningButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void СonnectButton_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_loginEditText.Text)
                && string.IsNullOrEmpty(_passwordEditText.Text))
            {
                Toast.MakeToast(Translator.Translate("user_pass_empty"));
            }
            else if (string.IsNullOrEmpty(_loginEditText.Text))
            {
                Toast.MakeToast(Translator.Translate("user_empty"));
            }
            else if (string.IsNullOrEmpty(_passwordEditText.Text))
            {
                Toast.MakeToast(Translator.Translate("password_empty"));
            }
            else
            {
                Authorization.StartAuthorization(_loginEditText.Text, _passwordEditText.Text, this);
            }
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        public void ClearPassword()
        {
            _passwordEditText.Text = "";
        }
    }
}