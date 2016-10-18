using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.DbEngine;
using System;
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
            _memoEdit.AutoFocus = true;
            _memoEdit.SetFocus();
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

            Navigation.Back();
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal string GetResourceImage(object tag)
            => ResourceManager.GetImage($"{tag}");
    }
}