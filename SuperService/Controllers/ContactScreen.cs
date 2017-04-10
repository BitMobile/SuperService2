using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections.Generic;
using Test.Catalog;
using Test.Components;

namespace Test
{
    public class ContactScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;
        private Contacts _contact;
        private bool _fieldsAreInitialized;

        public override void OnLoading()
        {
            base.OnLoading();
            InitClassFields();
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("contact"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                RightButtonControl = new TextView(Translator.Translate("edit")),
                ArrowVisible = false
            };
            _topInfoComponent.ActivateBackButton();
        }

        public int InitClassFields()
        {
            if (_fieldsAreInitialized)
            {
                return 0;
            }

            _contact = (Contacts)Variables.GetValueOrDefault(Parameters.Contact);
            _fieldsAreInitialized = true;
            return 0;
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        /// <summary>
        ///     Проверяет строку на то, что она null, пустая
        ///     или представляет пробельный символ
        /// </summary>
        /// <param name="item">Строка для проверки</param>
        /// <returns>
        ///     True если строка пустая, null или
        ///     пробельный символ.
        /// </returns>
        internal bool IsNotEmptyString(string item)
        {
            return !(string.IsNullOrEmpty(item) && string.IsNullOrWhiteSpace(item));
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Move("EditContactScreen", new Dictionary<string, object>
            {
                [Parameters.Contact] = _contact
            });
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal void TopInfo_LeftButton_OnPressDown(object sender, EventArgs e)
        {
            ((Image)_topInfoComponent.LeftButtonControl).Source = ResourceManager.GetImage("topheading_back_active");
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_LeftButton_OnPressUp(object sender, EventArgs e)
        {
            ((Image)_topInfoComponent.LeftButtonControl).Source = ResourceManager.GetImage("topheading_back");
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_RightButton_OnPressDown(object sender, EventArgs e)
        {
            ((VerticalLayout)((TextView)_topInfoComponent.RightButtonControl).Parent).CssClass = "TopInfoButtonRightActive";
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_RightButton_OnPressUp(object sender, EventArgs e)
        {
            ((VerticalLayout)((TextView)_topInfoComponent.RightButtonControl).Parent).CssClass = "TopInfoButtonRight";
            _topInfoComponent.Refresh();
        }

        internal void CallButton_OnClick(object o, EventArgs e)
        {
            DConsole.WriteLine("Пытаемся позвонить");
            Phone.Call(_contact.Tel);
        }

        internal void CallButton_OnPressDown(object sender, EventArgs e)
        {
            Image image = (Image)((VerticalLayout)sender).GetControl(0);
            image.Source = GetResourceImage("contactscreen_phone_active");
            image.Refresh();
        }

        internal void CallButton_OnPressUp(object sender, EventArgs e)
        {
            Image image = (Image)((VerticalLayout)sender).GetControl(0);
            image.Source = GetResourceImage("contactscreen_phone");
            image.Refresh();
        }

        internal void SendMessageButton_OnClick(object o, EventArgs e)
        {
            Dialog.Message(Translator.Translate("under_construction"));
        }

        internal void SendMessageButton_OnPressDown(object sender, EventArgs e)
        {
            Image image = (Image)((VerticalLayout)sender).GetControl(0);
            image.Source = GetResourceImage("contactscreen_sms_active");
            image.Refresh();
        }

        internal void SendMessageButton_OnPressUp(object sender, EventArgs e)
        {
            Image image = (Image)((VerticalLayout)sender).GetControl(0);
            image.Source = GetResourceImage("contactscreen_sms");
            image.Refresh();
        }

        internal void WriteEMailButton_OnClick(object o, EventArgs e)
        {
            Dialog.Message(Translator.Translate("under_construction"));
        }

        internal void WriteEMailButton_OnPressDown(object sender, EventArgs e)
        {
            Image image = (Image)((VerticalLayout)sender).GetControl(0);
            image.Source = GetResourceImage("contactscreen_email_active");
            image.Refresh();
        }

        internal void WriteEMailButton_OnPressUp(object sender, EventArgs e)
        {
            Image image = (Image)((VerticalLayout)sender).GetControl(0);
            image.Source = GetResourceImage("contactscreen_email");
            image.Refresh();
        }

        internal void BackButton_OnClick(object o, EventArgs e)
        {
            Navigation.Back();
        }
    }
}