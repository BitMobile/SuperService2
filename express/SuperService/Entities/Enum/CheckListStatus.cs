using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class CheckListStatus : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(CheckListStatusEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case CheckListStatusEnum.Blank:
                    res = "854946f6-fc1d-bec2-4968-1f7e3c8d3c61";
                    break;
                case CheckListStatusEnum.Active:
                    res = "ba4d325a-f1b7-072d-4c3e-fd4bf9f33901";
                    break;
                case CheckListStatusEnum.Disactive:
                    res = "ab0acbab-556c-7058-4ba9-e4cd72c2958d";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_CheckListStatus]:{res}");
        }

        public CheckListStatusEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "854946f6-fc1d-bec2-4968-1f7e3c8d3c61": 
                    return CheckListStatusEnum.Blank;
                case "ba4d325a-f1b7-072d-4c3e-fd4bf9f33901": 
                    return CheckListStatusEnum.Active;
                case "ab0acbab-556c-7058-4ba9-e4cd72c2958d": 
                    return CheckListStatusEnum.Disactive;
            }
            return default(CheckListStatusEnum);
        }
}



    public enum CheckListStatusEnum
    {
        Blank,
        Active,
        Disactive,
    } 
}
    