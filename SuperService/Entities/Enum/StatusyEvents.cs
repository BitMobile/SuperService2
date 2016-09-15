using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class StatusyEvents : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(StatusyEventsEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case StatusyEventsEnum.Appointed:
                    res = "8d300e1c-3261-e745-4046-0ae57541898b";
                    break;
                case StatusyEventsEnum.Done:
                    res = "81998d6c-e971-8f4d-4fbb-bd4d3b61e737";
                    break;
                case StatusyEventsEnum.Cancel:
                    res = "81ec69ec-e546-b95d-4879-1cb04ea0a1e6";
                    break;
                case StatusyEventsEnum.InWork:
                    res = "a00d846a-3d09-46c0-4a19-b6a10e055c9e";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_StatusyEvents]:{res}");
        }

        public StatusyEventsEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "8d300e1c-3261-e745-4046-0ae57541898b": 
                    return StatusyEventsEnum.Appointed;
                case "81998d6c-e971-8f4d-4fbb-bd4d3b61e737": 
                    return StatusyEventsEnum.Done;
                case "81ec69ec-e546-b95d-4879-1cb04ea0a1e6": 
                    return StatusyEventsEnum.Cancel;
                case "a00d846a-3d09-46c0-4a19-b6a10e055c9e": 
                    return StatusyEventsEnum.InWork;
            }
            return default(StatusyEventsEnum);
        }
}



    public enum StatusyEventsEnum
    {
        Appointed,
        Done,
        Cancel,
        InWork,
    } 
}
    