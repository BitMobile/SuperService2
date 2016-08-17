using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using Test.Catalog;
using Test.Components;

namespace Test
{
    public class EditContactScreen : Screen
    {
        private string _clientId;
        private TopInfoComponent _topInfoComponent;

        private Contacts Contact => (Contacts)Variables[Parameters.Contact];

        private HorizontalLayout AddPhoneButton => (HorizontalLayout)Variables["AddPhoneButton"];
        private HorizontalLayout AddEmailButton => (HorizontalLayout)Variables["AddEmailButton"];
        private HorizontalLayout PhoneLayout => (HorizontalLayout)Variables["PhoneLayout"];
        private HorizontalLayout EmailLayout => (HorizontalLayout)Variables["EmailLayout"];

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("contact"),
                LeftButtonControl = new TextView(Translator.Translate("cancel")),
                RightButtonControl = new TextView(Translator.Translate("save")),
                ArrowVisible = false
            };
            _topInfoComponent.ActivateBackButton();

            if (!string.IsNullOrWhiteSpace(Contact.Tel))
            {
                AddPhoneButton.CssClass = "NoHeight";
            }
            else
            {
                PhoneLayout.CssClass = "NoHeight";
            }

            if (!string.IsNullOrWhiteSpace(Contact.EMail))
            {
                AddEmailButton.CssClass = "NoHeight";
            }
            else
            {
                EmailLayout.CssClass = "NoHeight";
            }

            _clientId = (string)Variables.GetValueOrDefault(Parameters.IdClientId);
        }

        internal void RemovePhoneButton_OnClick(object sender, EventArgs eventArgs)
        {
            PhoneLayout.CssClass = "NoHeight";
            AddPhoneButton.CssClass = "AddButton";
            ((EditText)Variables["PhoneEditText"]).Text = "";
            PhoneLayout.Refresh();
            AddPhoneButton.Refresh();
        }

        internal void RemoveEmailButton_OnClick(object sender, EventArgs eventArgs)
        {
            EmailLayout.CssClass = "NoHeight";
            AddEmailButton.CssClass = "AddButton";
            ((EditText)Variables["EMailEditText"]).Text = "";
            EmailLayout.Refresh();
            AddEmailButton.Refresh();
        }

        internal void AddPhoneButton_OnClick(object sender, EventArgs eventArgs)
        {
            AddPhoneButton.CssClass = "NoHeight";
            PhoneLayout.CssClass = "ContactFieldWithDelete";
            AddPhoneButton.Refresh();
            PhoneLayout.Refresh();
        }

        internal void AddEmailButton_OnClick(object sender, EventArgs eventArgs)
        {
            AddEmailButton.CssClass = "NoHeight";
            EmailLayout.CssClass = "ContactFieldWithDelete";
            AddEmailButton.Refresh();
            EmailLayout.Refresh();
        }

        internal string GetName(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) return "";
            var words = description.Split(null);
            return words.Length > 0 ? words[0] : "";
        }

        internal string GetSurname(string description)
        {
            return string.IsNullOrWhiteSpace(description) ? "" : description.Substring(GetName(description).Length).TrimStart();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
            var name = ((EditText)Variables["NameEditText"]).Text;
            var surname = ((EditText)Variables["SurnameEditText"]).Text;
            var position = ((EditText)Variables["PositionEditText"]).Text;
            var phone = ((EditText)Variables["PhoneEditText"]).Text;
            var email = ((EditText)Variables["EMailEditText"]).Text;
            // TODO: Разбраться с Code
            if (string.IsNullOrWhiteSpace(name))
            {
                Dialog.Message(Translator.Translate("forgot_name"));
                return;
            }

            Contact.Description = $"{name} {surname}";
            Contact.Position = position;
            Contact.Tel = phone;
            Contact.EMail = email;

            DBHelper.SaveEntity(Contact);

            if (_clientId != null)
            {
                // TODO Разобраться с LineNumber
                var clientContacts = new Client_Contacts
                {
                    Ref = DbRef.FromString(_clientId),
                    Id = DbRef.CreateInstance("Catalog_Client_Contacts", Guid.NewGuid()),
                    Contact = Contact.Id,
                    Actual = false // Actual на самом деле означает "уволен"
                };
                DBHelper.SaveEntity(clientContacts);
            }
            Navigation.Back();
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}