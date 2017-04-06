using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class StatsNeedNum : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(StatsNeedNumEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case StatsNeedNumEnum.New:
                    res = "aba20521-d3cb-0a3f-42c2-eee2ce12a07a";
                    break;
                case StatsNeedNumEnum.Done:
                    res = "bdd2411f-c0e4-2a47-4486-a87fa2ad1dfb";
                    break;
                case StatsNeedNumEnum.Confirmed:
                    res = "b1c2efd2-66a9-e59e-4211-09077ec95dc7";
                    break;
                case StatsNeedNumEnum.Cancel:
                    res = "b7b121a8-56ab-5a80-4a81-096647744ebd";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_StatsNeedNum]:{res}");
        }

        public StatsNeedNumEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "aba20521-d3cb-0a3f-42c2-eee2ce12a07a": 
                    return StatsNeedNumEnum.New;
                case "bdd2411f-c0e4-2a47-4486-a87fa2ad1dfb": 
                    return StatsNeedNumEnum.Done;
                case "b1c2efd2-66a9-e59e-4211-09077ec95dc7": 
                    return StatsNeedNumEnum.Confirmed;
                case "b7b121a8-56ab-5a80-4a81-096647744ebd": 
                    return StatsNeedNumEnum.Cancel;
            }
            return default(StatsNeedNumEnum);
        }
}



    public enum StatsNeedNumEnum
    {
        New,
        Done,
        Confirmed,
        Cancel,
    } 
}
    