using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class SettingMobileApplication : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DbRef DataType { get; set; }
        public bool LogicValue { get; set; }
        public int NumericValue { get; set; }
}


}
    