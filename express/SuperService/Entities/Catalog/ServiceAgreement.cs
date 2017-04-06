using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class ServiceAgreement : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DbRef Client { get; set; }
        public string Organization { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
}


}
    