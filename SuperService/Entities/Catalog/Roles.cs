using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class Roles : DbEntity
    {
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Name { get; set; }
        public string Ident { get; set; }
        public string Description { get; set; }
        public bool CanManageSelf { get; set; }
}


}
    