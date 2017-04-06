using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class User : DbEntity
    {
        public bool Predefined { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserDB { get; set; }
        public string EMail { get; set; }
        public DbRef UserID { get; set; }
        public string Phone { get; set; }
        public DbRef Role { get; set; }
        public string Language { get; set; }
        public string AspNetUserID { get; set; }
}
    public class User_Bag : DbEntity
    {
        public DbRef Id { get; set; }
        public DbRef Materials { get; set; }
        public decimal Count { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }

   }
    public class User_RemainsNorms : DbEntity
    {
        public DbRef Id { get; set; }
        public DbRef Materials { get; set; }
        public decimal Count { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }

   }


}
    