using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class VATS : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(VATSEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case VATSEnum.Percent18:
                    res = "7cb0603a-e767-4e13-b86b-882242e9e05f";
                    break;
                case VATSEnum.Percent0:
                    res = "7a3bfd88-68bd-44e3-87b0-51a844828792";
                    break;
                case VATSEnum.PercentWithoOut:
                    res = "123f1daf-215e-417e-b975-223089e9f0d0";
                    break;
                case VATSEnum.Percent10:
                    res = "a100a27f-7f2f-488b-aab8-573db25f83c1";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_VATS]:{res}");
        }

        public VATSEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "7cb0603a-e767-4e13-b86b-882242e9e05f": 
                    return VATSEnum.Percent18;
                case "7a3bfd88-68bd-44e3-87b0-51a844828792": 
                    return VATSEnum.Percent0;
                case "123f1daf-215e-417e-b975-223089e9f0d0": 
                    return VATSEnum.PercentWithoOut;
                case "a100a27f-7f2f-488b-aab8-573db25f83c1": 
                    return VATSEnum.Percent10;
            }
            return default(VATSEnum);
        }
}



    public enum VATSEnum
    {
        Percent18,
        Percent0,
        PercentWithoOut,
        Percent10,
    } 
}
    