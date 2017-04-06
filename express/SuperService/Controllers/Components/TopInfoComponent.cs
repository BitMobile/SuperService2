using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Controls;
using System;

namespace Test.Components
{
    public class TopInfoComponent
    {
        private readonly Screen _parentScreen;
        private bool _arrowActive = true;
        private bool _arrowVisible = true;
        private VerticalLayout _leftButton;
        private bool _minimized = true;
        private VerticalLayout _rightButton;
        private Image _topInfoArrowImage;
        private TextView _topInfoHeadingTextView;
        private VerticalLayout _topInfoImageLayout;
        private TextView _topInfoSubHeadingTextView;
        private VerticalLayout _topInfoHeadingTextLayout;
        private VerticalLayout _topInfoRootLayout;

        /// <summary>
        ///     Конструктор контроллера компонента с заголовком, двумя кнопками и доп. инфой
        /// </summary>
        /// <param name="parentScreen">
        ///     Экран, на котором находится элемент
        /// </param>
        public TopInfoComponent(Screen parentScreen)
        {
            _parentScreen = parentScreen;
            OnLoading();
        }

        public bool Minimized
        {
            get { return _minimized; }
            set
            {
                _minimized = value;
                UpdateArrowImage();
                ExtraLayout.CssClass = value ? "NoHeight" : "TopInfoExtraLayout";
            }
        }

        public bool ArrowVisible
        {
            get { return _arrowVisible; }
            set
            {
                _arrowVisible = value;
                _topInfoImageLayout.CssClass = value ? "TopInfoImageLayout" : "NoHeight";
            }
        }

        public bool ArrowActive
        {
            get { return _arrowActive; }
            set
            {
                _arrowActive = value;
                UpdateArrowImage();
            }
        }

        public VerticalLayout CommentLayout { get; private set; }

        public IWrappedControl3 LeftButtonControl
        {
            get { return GetIfNotEmpty(_leftButton); }
            set
            {
                AddIfNotEmpty(_leftButton, value);
            }
        }

        public IWrappedControl3 RightButtonControl
        {
            get { return GetIfNotEmpty(_rightButton); }
            set { AddIfNotEmpty(_rightButton, value); }
        }

        public string Header
        {
            get { return _topInfoHeadingTextView.Text; }
            set { _topInfoHeadingTextView.Text = value; }
        }

        public string SubHeader
        {
            get { return _topInfoSubHeadingTextView.Text; }
            set
            {
                _topInfoSubHeadingTextView.Text = value;
                if (string.IsNullOrEmpty(value))
                {
                    _topInfoSubHeadingTextView.CssClass = "NoHeight";
                }
                else
                {
                    _topInfoSubHeadingTextView.CssClass = "TopInfoSubHeading";
                }
            }
        }

        public void ActivateBackButton()
        {
            _leftButton.OnEvent = "Back";
        }

        private void UpdateArrowImage()
        {
            string imageTag = $"topinfo_{(Minimized ? "down" : "up")}{(ArrowActive ? "" : "no")}arrow";
            _topInfoArrowImage.Source = ResourceManager.GetImage(imageTag);
        }

        public VerticalLayout ExtraLayout { get; private set; }

        private IWrappedControl3 GetIfNotEmpty(VerticalLayout layout)
        {
            return layout.Controls.Length == 0 ? null : (IWrappedControl3)layout.Controls[0];
        }

        private void AddIfNotEmpty(VerticalLayout verticalLayout, IWrappedControl3 control)
        {
            if (verticalLayout.Controls.Length == 0) verticalLayout.AddChild(control);
            if (LeftButtonControl == null || RightButtonControl == null)
                return;
            if (LeftButtonControl.GetType().Name == nameof(Image) && RightButtonControl.GetType().Name == nameof(Image))
            {
                _leftButton.CssClass = "TopInfoButtonLeftWithPicOnly";
                _rightButton.CssClass = "TopInfoButtonRightWithPicOnly";
                _topInfoHeadingTextLayout.CssClass = "TopInfoHeadingTextLayoutWithPicOnly";
                DConsole.WriteLine("They are both images!");
            }
        }

        private void OnLoading()
        {
            _leftButton = (VerticalLayout)_parentScreen.Variables["TopInfoLeftButton"];
            _rightButton = (VerticalLayout)_parentScreen.Variables["TopInfoRightButton"];
            _topInfoHeadingTextView = (TextView)_parentScreen.Variables["TopInfoHeadingTextView"];
            _topInfoSubHeadingTextView = (TextView)_parentScreen.Variables["TopInfoSubHeadingTextView"];
            _topInfoArrowImage = (Image)_parentScreen.Variables["TopInfoArrowImage"];
            _topInfoImageLayout = (VerticalLayout)_parentScreen.Variables["TopInfoImageLayout"];
            CommentLayout = (VerticalLayout)_parentScreen.Variables["TopInfoCommentLayout"];
            ExtraLayout = (VerticalLayout)_parentScreen.Variables["TopInfoExtraLayout"];
            _topInfoHeadingTextLayout = (VerticalLayout)_parentScreen.Variables["TopInfoHeadingTextLayout"];
            _topInfoRootLayout = (VerticalLayout)_parentScreen.Variables["TopInfoRootLayout"];
        }

        public void Refresh()
        {
            _topInfoRootLayout.Refresh();
        }

        internal void Arrow_OnClick(object sender, EventArgs eventArgs)
        {
            if (ArrowActive)
                Minimized = !Minimized;
            Refresh();
        }
    }
}