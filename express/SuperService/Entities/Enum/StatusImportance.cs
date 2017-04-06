using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class StatusImportance : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(StatusImportanceEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case StatusImportanceEnum.Standart:
                    res = "9deb314e-1bd6-1ee0-4eb2-ac621ba09b74";
                    break;
                case StatusImportanceEnum.High:
                    res = "9495d0f0-6ef5-a7fe-473f-8e2d6e8586e2";
                    break;
                case StatusImportanceEnum.Critical:
                    res = "a570aeea-0f88-54c3-4075-d0cb82f0dd95";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_StatusImportance]:{res}");
        }

        public StatusImportanceEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "9deb314e-1bd6-1ee0-4eb2-ac621ba09b74": 
                    return StatusImportanceEnum.Standart;
                case "9495d0f0-6ef5-a7fe-473f-8e2d6e8586e2": 
                    return StatusImportanceEnum.High;
                case "a570aeea-0f88-54c3-4075-d0cb82f0dd95": 
                    return StatusImportanceEnum.Critical;
            }
            return default(StatusImportanceEnum);
        }
}



    public enum StatusImportanceEnum
    {
        Standart,
        High,
        Critical,
    } 
}
    