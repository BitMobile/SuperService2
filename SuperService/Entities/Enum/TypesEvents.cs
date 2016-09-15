using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class TypesEvents : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(TypesEventsEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case TypesEventsEnum.Visit:
                    res = "83c0e4e3-0164-81bf-4804-a18e953859f5";
                    break;
                case TypesEventsEnum.Task:
                    res = "be62d056-4a2d-3ff4-45bf-b48e8f3b30fb";
                    break;
                case TypesEventsEnum.Call:
                    res = "ad7e2b2c-b856-ed78-4d06-4b44a4a4cd57";
                    break;
                case TypesEventsEnum.Letter:
                    res = "8824e263-6025-070c-4ea0-55807f97b5e3";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_TypesEvents]:{res}");
        }

        public TypesEventsEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "83c0e4e3-0164-81bf-4804-a18e953859f5": 
                    return TypesEventsEnum.Visit;
                case "be62d056-4a2d-3ff4-45bf-b48e8f3b30fb": 
                    return TypesEventsEnum.Task;
                case "ad7e2b2c-b856-ed78-4d06-4b44a4a4cd57": 
                    return TypesEventsEnum.Call;
                case "8824e263-6025-070c-4ea0-55807f97b5e3": 
                    return TypesEventsEnum.Letter;
            }
            return default(TypesEventsEnum);
        }
}



    public enum TypesEventsEnum
    {
        Visit,
        Task,
        Call,
        Letter,
    } 
}
    