using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class Client : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }
        public DbRef Contractor { get; set; }
}
    public class Client_Files : DbEntity
    {
        public DbRef Id { get; set; }
        public DbRef Ref { get; set; }
        public int LineNumber { get; set; }
        public string FullFileName { get; set; }
        public DbRef FileName { get; set; }

   }
    public class Client_Contacts : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef Contact { get; set; }
        public bool Actual { get; set; }

   }
    public class Client_Parameters : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef Parameter { get; set; }
        public string Val { get; set; }

   }


}
    