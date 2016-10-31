using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class Equipment : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DbRef SKU { get; set; }
}
    public class Equipment_Equipments : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DateTime Period { get; set; }
        public DbRef Clients { get; set; }
        public DbRef Ref { get; set; }
        public DbRef StatusEquipment { get; set; }
        public DbRef ContractSale { get; set; }
        public DbRef CantractService { get; set; }
        public string ContactForEquipment { get; set; }
        public string Info { get; set; }
        public DbRef Equipment { get; set; }

   }
    public class Equipment_EquipmentsHistory : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DateTime Period { get; set; }
        public DbRef Client { get; set; }
        public DbRef Ref { get; set; }
        public DbRef Equipments { get; set; }
        public string Target { get; set; }
        public DbRef Result { get; set; }
        public string ObjectGet { get; set; }
        public string Comment { get; set; }
        public DbRef Executor { get; set; }

   }
    public class Equipment_Files : DbEntity
    {
        public DbRef Id { get; set; }
        public DbRef Ref { get; set; }
        public int LineNumber { get; set; }
        public string FullFileName { get; set; }
        public DbRef FileName { get; set; }

   }
    public class Equipment_Parameters : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef Parameter { get; set; }
        public string Val { get; set; }

   }


}
    