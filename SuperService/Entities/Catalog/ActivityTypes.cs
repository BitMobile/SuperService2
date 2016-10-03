using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class ActivityTypes : DbEntity
    {
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DbRef Responsible { get; set; }
}


}
    