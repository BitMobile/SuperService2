using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections.Generic;
using Test.Components;

namespace Test
{
    public class ChatScreen : Screen
    {
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowVisible = false,
                ArrowActive = false,
                Header = Translator.Translate("discussion_feed"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") }
            };

            _topInfoComponent.ActivateBackButton();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal string GetResourceImage(object tag)
            => ResourceManager.GetImage($"{tag}");

        internal DbRecordset GetMessages()
            => DBHelper.GetMessages(Variables[Parameters.IdTenderId]);

        internal string FormatMessageTime(object date)
        {
            DateTime parseDate;

            if (!DateTime.TryParse($"{date}", out parseDate))
            {
                Utils.TraceMessage($"Не удалось распарсить дату {Environment.NewLine}" +
                                   $"{date}");
            }

            return parseDate.ToString("g");
        }

        internal void WriteMessage_OnClick(object sender, EventArgs e)
            => Navigation.Move(nameof(WriteMessageScreen), new Dictionary<string, object>
            {{Parameters.IdTenderId, Variables[Parameters.IdTenderId]}});
    }
}