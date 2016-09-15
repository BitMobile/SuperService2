using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using Test.Catalog;

namespace Test
{
    internal class AddContactScreen : Screen
    {
        public override void OnLoading()
        {
            DConsole.WriteLine("Add Contackt Screen");
        }

        internal void Back_OnClick(object sender, EventArgs e)
        {
            //BusinessProcess.DoAction("Client");
            Navigation.Back();
        }

        internal void AddContactButton_OnClick(object sender, EventArgs e)
        {
            object clientId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdClientId, out clientId))
            {
                DConsole.WriteLine("Adding contact error. Can't find current client ID. Unnable to add contact to DB. Going to crash");
                return;
            }

            var name = ((EditText)Variables["name"]).Text;
            var position = ((EditText)Variables["position"]).Text;
            var tel = ((EditText)Variables["tel"]).Text;
            var email = ((EditText)Variables["email"]).Text;

            var newContact = new Contacts()
            {
                Id = DbRef.CreateInstance("Catalog_Contacts", Guid.NewGuid()),
                Description = name,
                Position = position,
                EMail = tel,
                Tel = email
            };

            DBHelper.SaveEntity(newContact);

            var newClientContact = new Client_Contacts()
            {
                Ref = DbRef.FromString((string)clientId),
                Contact = newContact.Id
            };

            DBHelper.SaveEntity(newClientContact);
            Navigation.Back(true);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            //BusinessProcess.DoAction("BackToEvent");
            Navigation.Back();
        }

        public override void OnShow()
        {
            GpsTracking.Start();
        }
    }
}