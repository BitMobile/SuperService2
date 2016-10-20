using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class Tender : DbEntity
    {
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public DbRef Client { get; set; }
        public DateTime DueDateTime { get; set; }
        public string Description { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string Marketplace { get; set; }
        public decimal Sum { get; set; }
}
    public class Tender_ActivityTypes : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef ActivityType { get; set; }

   }


}
    