using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class Profile : DbEntity
    {
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Description { get; set; }
}


}
    