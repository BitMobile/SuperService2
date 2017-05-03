using System;
using BitMobile.Common.Device.Providers;
using BitMobile.Common.FiscalRegistrator;

namespace Test
{
    public static class FptrAction
    {
        public static readonly TimeSpan DefaultTimeOut = new TimeSpan(0,0,10);
        public static readonly TimeSpan PrintingTimeOut = new TimeSpan(0,0,15);

        public static void CheckError(this IFiscalRegistratorProvider provider)
        {
            var resultCode = provider.GetResultCode();

            if (resultCode >= 0) return;

            var resultDescription = provider.GetResultDescription();
            if (resultDescription.Contains("Устройство не включено"))
                resultDescription = BitMobile.ClientModel3.Translator.Translate("device_not_found");

            string badParameter = null;

            if (resultCode == -6)
                badParameter = provider.GetBadParamDescription();

            if (badParameter != null)
                throw new FPTRException(resultCode, badParameter);
            throw new FPTRException(resultCode, resultDescription);
        }

        public static void OpenCheck(this IFiscalRegistratorProvider fptr, int type)
        {
            if (fptr.PutMode(FiscalRegistratorConsts.ModeRegistration) < 0)
                fptr.CheckError();
            if (fptr.SetMode() < 0)
                fptr.CheckError();
            if (fptr.PutCheckType(type) < 0)
                fptr.CheckError();
            if (fptr.OpenCheck() < 0)
                fptr.CheckError();
        }

        public static void CloseCheck(this IFiscalRegistratorProvider fptr, int typeClose)
        {
            if (fptr.PutTypeClose(typeClose) < 0)
                fptr.CheckError();
            if (fptr.CloseCheck() < 0)
                fptr.CheckError();
        }

        [Obsolete("Fix в следующем релизе")]
        public static void RegistrationFz54(this IFiscalRegistratorProvider fptr, string name, double price,
            double quantity, int discountType,
            double discount, int taxNumber)
        {

            Utils.TraceMessage($"Name: {name} Price: {price}{Environment.NewLine}" +
                               $"Quantity: {quantity} {nameof(discountType)}: {discountType}" +
                               $"{Environment.NewLine}{nameof(discount)}: {discount} {nameof(taxNumber)}: {taxNumber}");

            if (fptr.PutDiscountType(discountType) < 0)
                fptr.CheckError();
            if (fptr.PutSumm(discount) < 0)
                fptr.CheckError();
            if (fptr.PutTaxNumber(taxNumber) < 0)
                fptr.CheckError();
            if (fptr.PutQuantity(quantity) < 0)
                fptr.CheckError();
            if (fptr.PutPrice(price) < 0)
                fptr.CheckError();
            if (fptr.PutTextWrap(FiscalRegistratorConsts.WrapWord) < 0)
                fptr.CheckError();
            if (fptr.PutName(name) < 0)
                fptr.CheckError();
            if (fptr.Registration() < 0)
                fptr.CheckError();
        }

        public static void RegistrationFz54(this IFiscalRegistratorProvider fptr, string name, double price,
            double quantity, double positionSum, int taxNumber)
        {
            Utils.TraceMessage($"Name: {name} Price: {price}{Environment.NewLine}" +
                              $"Quantity: {quantity}" +
                              $"{Environment.NewLine}{nameof(taxNumber)}: {taxNumber}");

            if(fptr.PutPositionSum(positionSum) < 0)
                fptr.CheckError();
            if (fptr.PutQuantity(quantity) < 0)
                fptr.CheckError();
            if (fptr.PutPrice(price) < 0)
                fptr.CheckError();
            if (fptr.PutTaxNumber(taxNumber) < 0)
                fptr.CheckError();
            if (fptr.PutTextWrap(FiscalRegistratorConsts.WrapWord) < 0)
                fptr.CheckError();
            if (fptr.PutName(name) < 0)
                fptr.CheckError();
            if (fptr.Registration() < 0)
                fptr.CheckError();

        }

        public static void Payment(this IFiscalRegistratorProvider fptr, double sum, int type)
        {
            if (fptr.PutSumm(sum) < 0)
                fptr.CheckError();
            if (fptr.PutTypeClose(type) < 0)
                fptr.CheckError();
            if (fptr.Payment() < 0)
                fptr.CheckError();
        }

        public static void PrintZ(this IFiscalRegistratorProvider fptr)
        {
            if (fptr.PutMode(FiscalRegistratorConsts.ModeReportClear) < 0)
                fptr.CheckError();
            if (fptr.SetMode() < 0)
                fptr.CheckError();
            if (fptr.PutReportType(FiscalRegistratorConsts.ReportZ) < 0)
                fptr.CheckError();
            if (fptr.Report() < 0)
                fptr.CheckError();
        }

        public static void PrintX(this IFiscalRegistratorProvider fptr)
        {
            if (fptr.PutMode(FiscalRegistratorConsts.ModeReportNoClear) < 0)
                fptr.CheckError();
            if (fptr.SetMode() < 0)
                fptr.CheckError();
            if (fptr.PutReportType(FiscalRegistratorConsts.ReportX) < 0)
                fptr.CheckError();
            if (fptr.Report() < 0)
                fptr.CheckError();
        }
    }
}