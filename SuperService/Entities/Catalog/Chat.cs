using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class Chat : DbEntity
    {
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public DateTime DateTime { get; set; }
        public DbRef Tender { get; set; }
        public DbRef User { get; set; }
        public string Message { get; set; }
}


}
    