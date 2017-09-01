using System;
using BitMobile.Common.Device.Providers;
using BitMobile.Common.FiscalRegistrator;
using System.Runtime.CompilerServices;

namespace Test
{
    public static class FptrAction
    {
        public static readonly TimeSpan DefaultTimeOut = new TimeSpan(0,0,10);
        public static readonly TimeSpan PrintingTimeOut = new TimeSpan(0,0,15);

        public static void CheckError(this IFiscalRegistratorProvider provider,
            [CallerMemberName] string callerMemberName = "")
        {
            var resultCode = provider.GetResultCode();

            if (resultCode >= 0)
            {
                Utils.TraceFiscalRegistratorActions($"Caller Member Name {callerMemberName}{Environment.NewLine}" +
                                                    $"FR state (ResultCode): {resultCode} {Environment.NewLine}");
                return;
            }

            var resultDescription = provider.GetResultDescription();
            if (resultDescription.Contains("Устройство не включено"))
                resultDescription = BitMobile.ClientModel3.Translator.Translate("device_not_found");

            string badParameter = null;

            if (resultCode == -6)
                badParameter = provider.GetBadParamDescription();

            Utils.TraceFiscalRegistratorActions($"Caller Member Name {callerMemberName}" +
                                                $"ResultCode: {resultCode} {Environment.NewLine}" +
                                                $"ResultDescription: {resultDescription}{Environment.NewLine}" +
                                                $"BadParameters: {badParameter}");

            if (badParameter != null)
            {
                Utils.TraceFiscalRegistratorActions($"Caller Member Name {callerMemberName}" +
                                                    $"Throw exception {Environment.NewLine}" +
                                                    $"ResultCode: {resultCode} {Environment.NewLine}" +
                                                    $"ResultDescription: {resultDescription}{Environment.NewLine}" +
                                                    $"BadParameters: {badParameter}");
                throw new FPTRException(resultCode, badParameter);
            }

            Utils.TraceFiscalRegistratorActions($"Caller Member Name {callerMemberName}" +
                                                $"Throw exception {Environment.NewLine}" +
                                                $"ResultCode: {resultCode} {Environment.NewLine}" +
                                                $"ResultDescription: {resultDescription}{Environment.NewLine}" +
                                                $"BadParameters: {badParameter}");

            throw new FPTRException(resultCode, resultDescription);
        }

        public static void OpenCheck(this IFiscalRegistratorProvider fptr, int type)
        {
            Utils.TraceFiscalRegistratorActions($"Start {nameof(OpenCheck)}");
            if (fptr.PutMode(FiscalRegistratorConsts.ModeRegistration) < 0)
                fptr.CheckError();
            if (fptr.SetMode() < 0)
                fptr.CheckError();
            if (fptr.PutCheckType(type) < 0)
                fptr.CheckError();
            if (fptr.OpenCheck() < 0)
                fptr.CheckError();
            Utils.TraceFiscalRegistratorActions($"Cheque is open");
        }

        public static void CloseCheck(this IFiscalRegistratorProvider fptr, int typeClose)
        {
            Utils.TraceFiscalRegistratorActions($"Start {nameof(CloseCheck)}");
            if (fptr.PutTypeClose(typeClose) < 0)
                fptr.CheckError();
            if (fptr.CloseCheck() < 0)
                fptr.CheckError();
            Utils.TraceFiscalRegistratorActions($"Cheque is closed");
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
            Utils.TraceFiscalRegistratorActions($"Register position" +
                                                $"Name: {name} Price: {price}{Environment.NewLine}" +
                                                $"Quantity: {quantity}{Environment.NewLine}" +
                                                $"{nameof(taxNumber)}: {taxNumber}{Environment.NewLine}");

            if (fptr.PutPositionSum(positionSum) < 0)
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
            Utils.TraceFiscalRegistratorActions($"Position registered");
        }

        public static void Payment(this IFiscalRegistratorProvider fptr, double sum, int type)
        {
            Utils.TraceFiscalRegistratorActions($"Start {nameof(Payment)}{Environment.NewLine}" +
                                                $"Sum: {sum}{Environment.NewLine}" +
                                                $"Type: {type}");
            if (fptr.PutSumm(sum) < 0)
                fptr.CheckError();
            if (fptr.PutTypeClose(type) < 0)
                fptr.CheckError();
            if (fptr.Payment() < 0)
                fptr.CheckError();
            Utils.TraceFiscalRegistratorActions($"Payment is over");
        }

        public static void PrintZ(this IFiscalRegistratorProvider fptr)
        {
            Utils.TraceFiscalRegistratorActions($"Start {nameof(PrintZ)}");
            if (fptr.PutMode(FiscalRegistratorConsts.ModeReportClear) < 0)
                fptr.CheckError();
            if (fptr.SetMode() < 0)
                fptr.CheckError();
            if (fptr.PutReportType(FiscalRegistratorConsts.ReportZ) < 0)
                fptr.CheckError();
            if (fptr.Report() < 0)
                fptr.CheckError();
            Utils.TraceFiscalRegistratorActions($"Complete {nameof(PrintZ)}");
        }

        public static void PrintX(this IFiscalRegistratorProvider fptr)
        {
            Utils.TraceFiscalRegistratorActions($"Start {nameof(PrintX)}");
            if (fptr.PutMode(FiscalRegistratorConsts.ModeReportNoClear) < 0)
                fptr.CheckError();
            if (fptr.SetMode() < 0)
                fptr.CheckError();
            if (fptr.PutReportType(FiscalRegistratorConsts.ReportX) < 0)
                fptr.CheckError();
            if (fptr.Report() < 0)
                fptr.CheckError();
            Utils.TraceFiscalRegistratorActions($"Complete {nameof(PrintX)}");
        }
    }
}