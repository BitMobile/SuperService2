using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class Webactions : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(WebactionsEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case WebactionsEnum.EventsEditing:
                    res = "0236eb9f-eed9-489b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.EventsDeleting:
                    res = "0236eb9f-eed9-490b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.EventsOptionsEditing:
                    res = "0236eb9f-eed9-491b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.EventsAllAvaliable:
                    res = "0236eb9f-eed9-492b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.EventsShowAVR:
                    res = "0236eb9f-eed9-494b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.UsersEditing:
                    res = "0236eb9f-eed9-495b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.UsersDeleting:
                    res = "0236eb9f-eed9-496b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.UsersManageRoles:
                    res = "0236eb9f-eed9-497b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.ClientsEditing:
                    res = "0236eb9f-eed9-498b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.ClientsDeleting:
                    res = "0236eb9f-eed9-499b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.ClientsOptionsEditing:
                    res = "0236eb9f-eed9-500b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.TasksEditing:
                    res = "0236eb9f-eed9-501b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.EquipmentsEditing:
                    res = "0236eb9f-eed9-502b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.EquipmentsDeleting:
                    res = "0236eb9f-eed9-503b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.EquipmentsOptionsEditing:
                    res = "0236eb9f-eed9-504b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.RIMEditing:
                    res = "0236eb9f-eed9-505b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.RIMDeleting:
                    res = "0236eb9f-eed9-506b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.CheckListsEditing:
                    res = "0236eb9f-eed9-507b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.CheckListManageActivityStatus:
                    res = "0236eb9f-eed9-508b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.AnaliticAccess:
                    res = "0236eb9f-eed9-509b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.WebInterfaceAccess:
                    res = "0236eb9f-eed9-510b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.MobileAppAccess:
                    res = "0236eb9f-eed9-511b-9152-86b037e79a79";
                    break;
                case WebactionsEnum.MobileFPRAccess:
                    res = "f22c49d3-42b4-4ccc-9646-970ee8f7db23";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_Webactions]:{res}");
        }

        public WebactionsEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "0236eb9f-eed9-489b-9152-86b037e79a79": 
                    return WebactionsEnum.EventsEditing;
                case "0236eb9f-eed9-490b-9152-86b037e79a79": 
                    return WebactionsEnum.EventsDeleting;
                case "0236eb9f-eed9-491b-9152-86b037e79a79": 
                    return WebactionsEnum.EventsOptionsEditing;
                case "0236eb9f-eed9-492b-9152-86b037e79a79": 
                    return WebactionsEnum.EventsAllAvaliable;
                case "0236eb9f-eed9-494b-9152-86b037e79a79": 
                    return WebactionsEnum.EventsShowAVR;
                case "0236eb9f-eed9-495b-9152-86b037e79a79": 
                    return WebactionsEnum.UsersEditing;
                case "0236eb9f-eed9-496b-9152-86b037e79a79": 
                    return WebactionsEnum.UsersDeleting;
                case "0236eb9f-eed9-497b-9152-86b037e79a79": 
                    return WebactionsEnum.UsersManageRoles;
                case "0236eb9f-eed9-498b-9152-86b037e79a79": 
                    return WebactionsEnum.ClientsEditing;
                case "0236eb9f-eed9-499b-9152-86b037e79a79": 
                    return WebactionsEnum.ClientsDeleting;
                case "0236eb9f-eed9-500b-9152-86b037e79a79": 
                    return WebactionsEnum.ClientsOptionsEditing;
                case "0236eb9f-eed9-501b-9152-86b037e79a79": 
                    return WebactionsEnum.TasksEditing;
                case "0236eb9f-eed9-502b-9152-86b037e79a79": 
                    return WebactionsEnum.EquipmentsEditing;
                case "0236eb9f-eed9-503b-9152-86b037e79a79": 
                    return WebactionsEnum.EquipmentsDeleting;
                case "0236eb9f-eed9-504b-9152-86b037e79a79": 
                    return WebactionsEnum.EquipmentsOptionsEditing;
                case "0236eb9f-eed9-505b-9152-86b037e79a79": 
                    return WebactionsEnum.RIMEditing;
                case "0236eb9f-eed9-506b-9152-86b037e79a79": 
                    return WebactionsEnum.RIMDeleting;
                case "0236eb9f-eed9-507b-9152-86b037e79a79": 
                    return WebactionsEnum.CheckListsEditing;
                case "0236eb9f-eed9-508b-9152-86b037e79a79": 
                    return WebactionsEnum.CheckListManageActivityStatus;
                case "0236eb9f-eed9-509b-9152-86b037e79a79": 
                    return WebactionsEnum.AnaliticAccess;
                case "0236eb9f-eed9-510b-9152-86b037e79a79": 
                    return WebactionsEnum.WebInterfaceAccess;
                case "0236eb9f-eed9-511b-9152-86b037e79a79": 
                    return WebactionsEnum.MobileAppAccess;
                case "f22c49d3-42b4-4ccc-9646-970ee8f7db23": 
                    return WebactionsEnum.MobileFPRAccess;
            }
            return default(WebactionsEnum);
        }
}



    public enum WebactionsEnum
    {
        EventsEditing,
        EventsDeleting,
        EventsOptionsEditing,
        EventsAllAvaliable,
        EventsShowAVR,
        UsersEditing,
        UsersDeleting,
        UsersManageRoles,
        ClientsEditing,
        ClientsDeleting,
        ClientsOptionsEditing,
        TasksEditing,
        EquipmentsEditing,
        EquipmentsDeleting,
        EquipmentsOptionsEditing,
        RIMEditing,
        RIMDeleting,
        CheckListsEditing,
        CheckListManageActivityStatus,
        AnaliticAccess,
        WebInterfaceAccess,
        MobileAppAccess,
        MobileFPRAccess,
    } 
}
    