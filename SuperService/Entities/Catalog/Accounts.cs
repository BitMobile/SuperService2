using System;
using BitMobile.DbEngine;

namespace Test.Entities.Catalog
{
    public class Accounts : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public bool IsFolder { get; set; }
        public DbRef Parent { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        
        public Accounts(DbRef id = null)
        {
            Id = id ?? DbRef.CreateInstance("Catalog_Accounts", Guid.NewGuid());
        }
}


}
    