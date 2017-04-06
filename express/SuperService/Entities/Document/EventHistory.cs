using System;
using BitMobile.DbEngine;

namespace Test.Document
{
    public class EventHistory : DbEntity
    {
        public DbRef Id { get; set; }
        public DateTime Date { get; set; }
        public bool DeletionMark { get; set; }
        public DbRef Status { get; set; }
        public DbRef Event { get; set; }
        public DbRef Author { get; set; }
        public DbRef UserMA { get; set; }
}


}
    