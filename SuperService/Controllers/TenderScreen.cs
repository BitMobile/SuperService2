using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Test.Catalog;
using Test.Components;

namespace Test
{
    public class TenderScreen : Screen
    {
        private DbRecordset _currentEventRecordset;
        private bool _readonly;
        private Button _refuseButton;
        private DockLayout _rootLayout;
        private Button _startButton;

        private VerticalLayout _startFinishButton;
        private Image _statusImage;
        private bool _taskCommentTextExpanded;
        private TextView _taskCommentTextView;

        private TopInfoComponent _topInfoComponent;
        private Image _wrapUnwrapImage;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowVisible = false,
                ArrowActive = false
            };
            _topInfoComponent.ActivateBackButton();
            LoadControls();
            FillControls();

            Utils.TraceMessage($"{Variables.GetValueOrDefault(Parameters.IdTenderId, string.Empty)}");
        }

        private void FillControls()
        {
            _topInfoComponent.Header =
                ((string)_currentEventRecordset["Client_Description"]).CutForUIOutput(13, 2);
            _topInfoComponent.CommentLayout.AddChild(new TextView(
                ((string)_currentEventRecordset["Client_Address"]).CutForUIOutput(17, 2)));

            _topInfoComponent.LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") };
            _topInfoComponent.RightButtonControl = new Image { Source = ResourceManager.GetImage("tender_chat") };

            _taskCommentTextView = (TextView)GetControl("EventCommentTextView", true);
            _wrapUnwrapImage = (Image)GetControl("WrapUnwrapImage", true);

            var extraHorizontalLayout = new HorizontalLayout { CssClass = "ExtraHorizontalLayout" };
            var leftExtraLayout = new VerticalLayout { CssClass = "ExtraVerticalLayout" };
            var rightExtraLayout = new VerticalLayout { CssClass = "ExtraVerticalLayout" };

            _topInfoComponent.ExtraLayout.AddChild(extraHorizontalLayout);
            extraHorizontalLayout.AddChild(leftExtraLayout);
            extraHorizontalLayout.AddChild(rightExtraLayout);

            leftExtraLayout.AddChild(new Image
            {
                CssClass = "TopInfoSideImage",
                Source = ResourceManager.GetImage("topinfo_extra_map")
            });
            leftExtraLayout.AddChild(new TextView
            {
                Text = Translator.Translate("onmap"),
                CssClass = "TopInfoSideText"
            });
            rightExtraLayout.AddChild(new Image
            {
                CssClass = "TopInfoSideImage",
                Source = ResourceManager.GetImage("topinfo_extra_person")
            });

            var text = (string)_currentEventRecordset["Client_Description"];
            if (string.IsNullOrEmpty(text))
                Translator.Translate("contact_not_present");
            else
                rightExtraLayout.OnClick += RightExtraLayoutOnOnClick;

            rightExtraLayout.AddChild(new TextView
            {
                Text = text.CutForUIOutput(12, 2),
                CssClass = "TopInfoSideText"
            });
            leftExtraLayout.OnClick += GoToMapScreen_OnClick;
        }

        private void RightExtraLayoutOnOnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Move(nameof(ContactScreen), new Dictionary<string, object>
            {
                [Parameters.Contact] = (Contacts)DBHelper.LoadEntity(_currentEventRecordset["contactId"].ToString())
            });
        }

        public override void OnShow()
        {
        }

        private void LoadControls()
        {
            _rootLayout = (DockLayout)Variables["RootLayout"];
            _startFinishButton = (VerticalLayout)Variables.GetValueOrDefault("StartFinishButton");
            _startButton = (Button)Variables.GetValueOrDefault("StartButton");
            _refuseButton = (Button)Variables.GetValueOrDefault("RefuseButton");
            _statusImage = (Image)Variables.GetValueOrDefault("StatusImage");
        }

        internal void ClientInfoButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Move("ClientScreen");
        }

        internal void RefuseButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Move("CancelEventScreen");
        }

        internal string FormatEventStartDatePlanTime(string eventStartDatePlan)
        {
            return DateTime.Parse(eventStartDatePlan).ToString("HH:mm");
        }

        internal void StartButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void WrapUnwrapButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_taskCommentTextExpanded)
            {
                _taskCommentTextView.CssClass = "SubComment";
                _wrapUnwrapImage.Source = ResourceManager.GetImage("longtext_expand");
                _taskCommentTextExpanded = false;
            }
            else
            {
                _taskCommentTextView.CssClass = "SubCommentExpanded";
                _wrapUnwrapImage.Source = ResourceManager.GetImage("longtext_close");
                _taskCommentTextExpanded = true;
            }
            _taskCommentTextView.Refresh();
            _rootLayout.Refresh();
        }

        private void Event_OnStart()
        {
            GetCurrentTender();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
            => Navigation.Move(nameof(ChatScreen), new Dictionary<string, object>
            {{Parameters.IdTenderId, Variables[Parameters.IdTenderId]}});

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
            _topInfoComponent.Arrow_OnClick(sender, eventArgs);
            _rootLayout.Refresh();
        }

        internal DbRecordset GetCurrentTender()
            => _currentEventRecordset = DBHelper.GetTenderById(
                Variables.GetValueOrDefault(Parameters.IdTenderId, string.Empty));

        internal string GetResourceImage(string tag)
            => ResourceManager.GetImage(tag);

        internal void GoToMapScreen_OnClick(object sender, EventArgs e)
        {
        }

        internal string GetFormatDate(object date)
        {
            DateTime extractDate;

            if (!DateTime.TryParse(date.ToString(), out extractDate))
            {
                Utils.TraceMessage($"DateTime {date} don't parse");
            }
            return extractDate.ToString("dd.MM");
        }

        internal string FormatCurrency(object currency)
            => $"{currency:C2}";

        internal void AddTask_OnClick(object sender, EventArgs e)
            => Navigation.Move(nameof(AddTaskScreen), new Dictionary<string, object>
            { {Parameters.IdTenderId, Variables.GetValueOrDefault(Parameters.IdTenderId)}, {Parameters.IdClientId,_currentEventRecordset["Client_Id"]} });

        internal void OpenMarketplace_OnClick(object sender, EventArgs e)
        {
            var textView = (TextView)((HorizontalLayout)sender).GetControl("MarketPlace", true);
            var rxgUrl = new Regex(@"(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");

            try
            {
                if (!rxgUrl.IsMatch(textView.Text ?? ""))
                {
                    Toast.MakeToast(Translator.Translate("uri_error"));
                    return;
                }
            }
            catch (Exception exception)
            {
                Utils.TraceMessage($"{exception.Message}");
                Toast.MakeToast(Translator.Translate("uri_error"));
                return;
            }

            Navigation.Move(nameof(WebViewScreen),
                new Dictionary<string, object>
                {
                    {Parameters.WebUri, textView?.Text}
                });
        }

        internal string ActivityCount()
        {
            var totalCount = DBHelper.GetActivitiCountByTender
                (Variables.GetValueOrDefault(Parameters.IdTenderId, string.Empty));

            return totalCount < 2
                ? Translator.Translate("activity")
                : $"{Translator.Translate("activity")} +{totalCount - 1}";
        }

        internal void ActivityList_OnClick(object sender, EventArgs e)
            => Navigation.Move(nameof(ListScreen), new Dictionary<string, object>
            { {Parameters.IdTenderId, Variables[Parameters.IdTenderId]} });

        internal void DeliveryDate_OnClick(object sender, EventArgs e)
        {
            var hl = (HorizontalLayout)sender;
            var textView = (TextView)hl.GetControl("90c45940df684ba3a7df7357874daf85", true);
            Navigation.Move(nameof(DeliveryDateDescriptionScreen),
                new Dictionary<string, object>
                { { Parameters.DeliveryDateDescription, textView?.Text } });
        }
    }
}