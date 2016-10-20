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
}
    public class ActivityTypes_Users : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef User { get; set; }

   }


}
    