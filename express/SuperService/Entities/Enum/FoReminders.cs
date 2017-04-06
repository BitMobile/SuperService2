using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class FoReminders : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(FoRemindersEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case FoRemindersEnum.Sale:
                    res = "a0e50e05-99b8-58c2-4a45-180c4462723a";
                    break;
                case FoRemindersEnum.Problem:
                    res = "969c9dd5-db42-b32a-40c7-935be3a19251";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_FoReminders]:{res}");
        }

        public FoRemindersEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "a0e50e05-99b8-58c2-4a45-180c4462723a": 
                    return FoRemindersEnum.Sale;
                case "969c9dd5-db42-b32a-40c7-935be3a19251": 
                    return FoRemindersEnum.Problem;
            }
            return default(FoRemindersEnum);
        }
}



    public enum FoRemindersEnum
    {
        Sale,
        Problem,
    } 
}
    