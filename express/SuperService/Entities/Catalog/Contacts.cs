using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class Contacts : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Position { get; set; }
        public string Tel { get; set; }
        public string EMail { get; set; }
}


}
    