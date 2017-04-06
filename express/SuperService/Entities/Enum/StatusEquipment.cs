using System;
using BitMobile.DbEngine;

namespace Test.Enum
{
    public class StatusEquipment : DbEntity
    {
        public DbRef Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public static DbRef GetDbRefFromEnum(StatusEquipmentEnum @enum)
        {
            string res = null;
            switch (@enum)
            {
                case StatusEquipmentEnum.Rent:
                    res = "951b5831-923e-0439-4ef7-9198a2cf19fe";
                    break;
                case StatusEquipmentEnum.Seales:
                    res = "b526c98c-9cca-ddd2-4c8f-b6be4e172e26";
                    break;
                case StatusEquipmentEnum.Loan:
                    res = "a9c2af13-ed6d-3af3-48f9-bdcbb4065442";
                    break;
                case StatusEquipmentEnum.Died:
                    res = "9a558829-062a-01f9-4c0d-3f32723dd886";
                    break;
            }
            if (string.IsNullOrEmpty(res)) return null;
            return DbRef.FromString($"@ref[Enum_StatusEquipment]:{res}");
        }

        public StatusEquipmentEnum GetEnum() 
        {
            switch(Id.Guid.ToString())
            {
                case "951b5831-923e-0439-4ef7-9198a2cf19fe": 
                    return StatusEquipmentEnum.Rent;
                case "b526c98c-9cca-ddd2-4c8f-b6be4e172e26": 
                    return StatusEquipmentEnum.Seales;
                case "a9c2af13-ed6d-3af3-48f9-bdcbb4065442": 
                    return StatusEquipmentEnum.Loan;
                case "9a558829-062a-01f9-4c0d-3f32723dd886": 
                    return StatusEquipmentEnum.Died;
            }
            return default(StatusEquipmentEnum);
        }
}



    public enum StatusEquipmentEnum
    {
        Rent,
        Seales,
        Loan,
        Died,
    } 
}
    