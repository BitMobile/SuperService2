using BitMobile.Common.Device.Providers;

namespace Test
{
    public static class FptrInstance
    {
        private static IFiscalRegistratorProvider _fptr;


        public static IFiscalRegistratorProvider Instance
        {
            get
            {
                if (_fptr != null) return _fptr;

                _fptr = ClientModel3.MD.FiscalRegistrator.GetProviderInstance();
                _fptr.Initialize();

                return _fptr;
            }
        }



    }
}