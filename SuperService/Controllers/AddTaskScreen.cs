using BitMobile.ClientModel3.UI;
using System;

namespace Test
{
    public class AddTaskScreen : Screen
    {
        public override void OnLoading()
        {
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

        internal void CreateTask_OnClick(object sender, EventArgs e)
        {
        }
    }