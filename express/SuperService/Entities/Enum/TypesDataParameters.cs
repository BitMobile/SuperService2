using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class TypesDataParameters : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(TypesDataParametersEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case TypesDataParametersEnum.String:
                    res = "8404c846-e63b-425c-40fb-e3081e1b97c0";
                    break;
                case TypesDataParametersEnum.Integer:
                    res = "a41d9f6f-7392-f28b-4894-2fdff7b03f74";
                    break;
                case TypesDataParametersEnum.Decimal:
                    res = "a93579ae-b709-07f2-4228-f017f13e594f";
                    break;
                case TypesDataParametersEnum.Boolean:
                    res = "aab753ce-1c3b-26b2-4d84-1461cd69c8ab";
                    break;
                case TypesDataParametersEnum.DateTime:
                    res = "be371ba8-80cb-b12e-4445-d9dbd848cd08";
                    break;
                case TypesDataParametersEnum.ValList:
                    res = "988d759f-473c-bdde-42c8-3dcc69bcc24b";
                    break;
                case TypesDataParametersEnum.Snapshot:
                    res = "ae3d67f7-50b3-4e00-4ab8-fc9cb4f1bf64";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_TypesDataParameters]:{res}");
        }

        public TypesDataParametersEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "8404c846-e63b-425c-40fb-e3081e1b97c0": 
                    return TypesDataParametersEnum.String;
                case "a41d9f6f-7392-f28b-4894-2fdff7b03f74": 
                    return TypesDataParametersEnum.Integer;
                case "a93579ae-b709-07f2-4228-f017f13e594f": 
                    return TypesDataParametersEnum.Decimal;
                case "aab753ce-1c3b-26b2-4d84-1461cd69c8ab": 
                    return TypesDataParametersEnum.Boolean;
                case "be371ba8-80cb-b12e-4445-d9dbd848cd08": 
                    return TypesDataParametersEnum.DateTime;
                case "988d759f-473c-bdde-42c8-3dcc69bcc24b": 
                    return TypesDataParametersEnum.ValList;
                case "ae3d67f7-50b3-4e00-4ab8-fc9cb4f1bf64": 
                    return TypesDataParametersEnum.Snapshot;
            }
            return default(TypesDataParametersEnum);
        }
}



    public enum TypesDataParametersEnum
    {
        String,
        Integer,
        Decimal,
        Boolean,
        DateTime,
        ValList,
        Snapshot,
    } 
}
    