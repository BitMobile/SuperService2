using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class RIM : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public bool IsFolder { get; set; }
        public DbRef Parent { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public bool Service { get; set; }
        public DbRef SKU { get; set; }
        public string Unit { get; set; }
        public DbRef VAT { get; set; }
}


}
    