using System;
using BitMobile.DbEngine;

namespace Test.Document
{
    public class NeedMat : DbEntity
    {
        public bool Posted { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public DbRef DocIn { get; set; }
        public DbRef StatsNeed { get; set; }
        public DbRef SR { get; set; }
        public bool FillFull { get; set; }
        public string SRMComment { get; set; }
        public string SRComment { get; set; }
}
    public class NeedMat_Matireals : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef SKU { get; set; }
        public decimal Count { get; set; }

   }


}
    