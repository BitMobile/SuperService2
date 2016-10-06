using BitMobile.ClientModel3.UI;

namespace Test
{
    public class TenderScreen : Screen
    {
        public override void OnShow()
        {
            base.OnShow();
            Utils.TraceMessage($"{nameof(TenderScreen)}");
        }
    }
}