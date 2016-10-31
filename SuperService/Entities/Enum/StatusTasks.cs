using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class StatusTasks : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(StatusTasksEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case StatusTasksEnum.New:
                    res = "a0a9e67f-483e-419a-a714-859f13c1245c";
                    break;
                case StatusTasksEnum.Done:
                    res = "a0a9e67f-483e-423a-a714-859f13c1245c";
                    break;
                case StatusTasksEnum.Rejected:
                    res = "a0a9e67f-483e-426b-a714-859f13c1245c";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_StatusTasks]:{res}");
        }

        public StatusTasksEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "a0a9e67f-483e-419a-a714-859f13c1245c": 
                    return StatusTasksEnum.New;
                case "a0a9e67f-483e-423a-a714-859f13c1245c": 
                    return StatusTasksEnum.Done;
                case "a0a9e67f-483e-426b-a714-859f13c1245c": 
                    return StatusTasksEnum.Rejected;
            }
            return default(StatusTasksEnum);
        }
}



    public enum StatusTasksEnum
    {
        New,
        Done,
        Rejected,
    } 
}
    