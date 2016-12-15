
using System;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    class FiltersScreen : Screen
    {
        private ScrollView _grScrollView;
        private TopInfoComponent _topInfoComponent;
        private String startFilterId;
        //private int startFilterWho;
        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ArrowVisible = false,
                ArrowActive = false,
                Header = Translator.Translate("FilteList"),
                LeftButtonControl = new Image { Source = ResourceManager.GetImage("topheading_back") }
            };
            _grScrollView = (ScrollView)Variables["c9c77e671ef64d128a4ecfea7cdf5bbf"];
            startFilterId = Filter.SelectedFilterId;
            //startFilterWho = Filter.FilterWho;
            //Utils.TraceMessage();
            _topInfoComponent.ActivateBackButton();
        }
        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Filter.SelectedFilterId = startFilterId;
            Navigation.Back();
        }
        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
        }

        internal bool ShowAdditionFilter() => DBHelper.CheckRole("EventsShowSubordinate");
        internal string GetResourceImage(object tag)
            => ResourceManager.GetImage($"{tag}");

        internal DbRecordset GetFilters()
        => DBHelper.GetFilters();
        internal string GetCurrentStatus(Guid filtersId)
        {
            bool status = Filter.SelectedFilterId == filtersId.ToString();
            Utils.TraceMessage($"{Filter.SelectedFilterId}" + $"{filtersId.ToString()}");
            var result = status ? GetResourceImage("task_target_done")
                 : GetResourceImage("task_target_not_done");
            Utils.TraceMessage($"Time: {DateTime.Now.ToString("HH:mm:ss:ffff")}" +
                               $"{Environment.NewLine}In XML Target Status = {result}");
            return result;
        }

        internal string GetCurrentStatusForOur(String TypeOur)
        {
            bool status = Filter.SelectedFilterId == TypeOur;
            Utils.TraceMessage($"{Filter.SelectedFilterId}" + $"{TypeOur.ToString()}");
            var result = status ? GetResourceImage("task_target_done")
                 : GetResourceImage("task_target_not_done");
            Utils.TraceMessage($"Time: {DateTime.Now.ToString("HH:mm:ss:ffff")}" +
                               $"{Environment.NewLine}In XML Target Status = {result}");
            return result;
        }

        internal void SelectFilter_OnClick(object sender, EventArgs e)
        {
            var hl = (HorizontalLayout)sender;
            if (Filter.SelectedFilterId != null)
            {
                var traget = (Image)((HorizontalLayout)_grScrollView.GetControl(Filter.SelectedFilterId, true)).GetControl("Img" + Filter.SelectedFilterId);
                traget.Source = GetResourceImage("task_target_not_done");
                traget.Refresh();
            }
            Filter.SelectedFilterId = ((HorizontalLayout) sender).Id;
            Utils.TraceMessage(Filter.SelectedFilterId);
            var tragetStatus = (Image)hl.GetControl("Img" + Filter.SelectedFilterId);
            tragetStatus.Source = GetResourceImage("task_target_done");
            tragetStatus.Refresh();
        }

        internal void SetButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back();
        }

    }
    }
