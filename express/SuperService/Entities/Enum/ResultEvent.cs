using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class ResultEvent : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(ResultEventEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case ResultEventEnum.New:
                    res = "ae46ba49-b076-6c8c-4730-4709e7191d5b";
                    break;
                case ResultEventEnum.Done:
                    res = "81270b2c-190a-faf2-440f-4a593042495e";
                    break;
                case ResultEventEnum.NotDone:
                    res = "9956cf47-e216-522e-48f6-8aba06fc9a5e";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_ResultEvent]:{res}");
        }

        public ResultEventEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "ae46ba49-b076-6c8c-4730-4709e7191d5b": 
                    return ResultEventEnum.New;
                case "81270b2c-190a-faf2-440f-4a593042495e": 
                    return ResultEventEnum.Done;
                case "9956cf47-e216-522e-48f6-8aba06fc9a5e": 
                    return ResultEventEnum.NotDone;
            }
            return default(ResultEventEnum);
        }
}



    public enum ResultEventEnum
    {
        New,
        Done,
        NotDone,
    } 
}
    