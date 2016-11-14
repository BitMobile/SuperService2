using System;
using BitMobile.DbEngine;

namespace Test.Catalog
{
    public class RoleWebactions : DbEntity
    {
        public DbRef Id { get; set; }
        public DbRef Role { get; set; }
        public DbRef Webaction { get; set; }
}


}
    