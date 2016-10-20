using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class MobileTaskFilters : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Description { get; set; }
        public string Query { get; set; }
}


}
    