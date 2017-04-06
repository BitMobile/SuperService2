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
                case StatusyEventsEnum.New:
                    res = "5f99e04e-10a8-4224-949a-7ed7863e1fb9";
                    break;
                case StatusyEventsEnum.OnHarmonization:
                    res = "e97536d7-061f-4b9a-9257-871ed9ad9229";
                    break;
                case StatusyEventsEnum.Agreed:
                    res = "46428625-7b29-41dc-a2a2-9c07cfbd85b2";
                    break;
                case StatusyEventsEnum.Accepted:
                    res = "dfbe38ad-0b3b-462c-b4fe-e026b4701604";
                    break;
                case StatusyEventsEnum.Cancel:
                    res = "57e71cfb-3d2e-4f57-9764-8f1a8a41d387";
                    break;
                case StatusyEventsEnum.InWork:
                    res = "a00d846a-3d09-46c0-4a19-b6a10e055c9e";
                    break;
                case StatusyEventsEnum.Done:
                    res = "3943413c-cf47-4f3b-8795-b93c9f78d1c6";
                    break;
                case StatusyEventsEnum.DoneWithTrouble:
                    res = "1220b261-dbcf-4df5-aec1-6630c250d643";
                    break;
                case StatusyEventsEnum.OnTheApprovalOf:
                    res = "be45c12b-267a-4c59-89b1-ac06af6dbc86";
                    break;
                case StatusyEventsEnum.Close:
                    res = "c36d3c89-3dbb-4b12-aef0-c970347e3961";
                    break;
                case StatusyEventsEnum.NotDone:
                    res = "7ecb70fa-fc91-49f0-a7fd-b905bd994a02";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_StatusyEvents]:{res}");
        }

        public StatusyEventsEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "5f99e04e-10a8-4224-949a-7ed7863e1fb9": 
                    return StatusyEventsEnum.New;
                case "e97536d7-061f-4b9a-9257-871ed9ad9229": 
                    return StatusyEventsEnum.OnHarmonization;
                case "46428625-7b29-41dc-a2a2-9c07cfbd85b2": 
                    return StatusyEventsEnum.Agreed;
                case "dfbe38ad-0b3b-462c-b4fe-e026b4701604": 
                    return StatusyEventsEnum.Accepted;
                case "57e71cfb-3d2e-4f57-9764-8f1a8a41d387": 
                    return StatusyEventsEnum.Cancel;
                case "a00d846a-3d09-46c0-4a19-b6a10e055c9e": 
                    return StatusyEventsEnum.InWork;
                case "3943413c-cf47-4f3b-8795-b93c9f78d1c6": 
                    return StatusyEventsEnum.Done;
                case "1220b261-dbcf-4df5-aec1-6630c250d643": 
                    return StatusyEventsEnum.DoneWithTrouble;
                case "be45c12b-267a-4c59-89b1-ac06af6dbc86": 
                    return StatusyEventsEnum.OnTheApprovalOf;
                case "c36d3c89-3dbb-4b12-aef0-c970347e3961": 
                    return StatusyEventsEnum.Close;
                case "7ecb70fa-fc91-49f0-a7fd-b905bd994a02": 
                    return StatusyEventsEnum.NotDone;
            }
            return default(StatusyEventsEnum);
        }
}



    public enum StatusyEventsEnum
    {
        New,
        OnHarmonization,
        Agreed,
        Accepted,
        Cancel,
        InWork,
        Done,
        DoneWithTrouble,
        OnTheApprovalOf,
        Close,
        NotDone,
    } 
}
    