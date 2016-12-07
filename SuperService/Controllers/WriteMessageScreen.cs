using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
using ClientModel3.MD;
using Test.Catalog;
using Test.Components;
using Test.Document;

namespace Test
{
    public class WriteMessageScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;
        private MemoEdit _memoEdit;

        public override void OnLoading()
        {
            _memoEdit = (MemoEdit)GetControl("ADF2E7DC-DB28-4CAA-90BC-C7C1C231F791", true);

            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowVisible = false,
                ArrowActive = false,
                Header = Translator.Translate("write_message"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") },
                RightButtonControl = new TextView { Text = Translator.Translate("send") }
            };

            _topInfoComponent.ActivateBackButton();
        }

        public override void OnShow()
        {
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(_memoEdit.Text))
            {
                Toast.MakeToast(Translator.Translate("empty_message"));
                return;
            }

            if (_memoEdit.Text.Length > 500)
            {
                Toast.MakeToast(Translator.Translate("max_lenght"));
                return;
            }

            var userId = (DbRef)DBHelper.GetUserInfoByUserName(Settings.User)["Id"];
            var entity = new Chat()
            {
                DateTime = DateTime.Now,
                Tender = DbRef.FromString($"{Variables[Parameters.IdTenderId]}"),
                User = userId,
                Message = _memoEdit.Text,
                Id = DbRef.CreateInstance($"{nameof(Catalog)}_{nameof(Chat)}", Guid.NewGuid())
            };

            DBHelper.SaveEntity(entity);
            SendMessage();

            Navigation.Back();
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal string GetResourceImage(object tag)
            => ResourceManager.GetImage($"{tag}");

        private string[] GetRecipiences()
        {
            var totalRecipience = DBHelper.GetTenderMessageRecipiencesCount(Variables[Parameters.IdTenderId],
                Settings.UserDetailedInfo.Id);
            Utils.TraceMessage($"TotalRecipiences: {totalRecipience}");
            if (totalRecipience < 1)
                return null;

            var result = new string[totalRecipience];

            var recipiences = DBHelper.GetTenderMessageRecipiences(Variables[Parameters.IdTenderId],
                Settings.UserDetailedInfo.Id);

            for (int i = 0; (i < result.Length) && recipiences.Next(); i++)
            {
                var buf = (DbRef) recipiences["UserId"];
                result[i] = buf.Id.ToString();
            }

            return result;
        }

        private void SendMessage()
        {
            var recipiences = GetRecipiences();

            if (recipiences == null)
            {
                Utils.TraceMessage($"Не нашлось пользователей, которым нужно отправлять сообщения.");
                return;
            }

            var currenTender = (Tender) DBHelper.LoadEntity($"{Variables[Parameters.IdTenderId]}");

            PushNotification.PushMessage($"Новое сообщение по тендеру {currenTender.Number}", recipiences);
        }
    }
}