using System;
using BitMobile.DbEngine;

namespace Test.Document
{
    public class Task : DbEntity
    {
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public bool Posted { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public DbRef Client { get; set; }
        public DbRef Equipment { get; set; }
        public DbRef Event { get; set; }
        public string TaskType { get; set; }
}
    public class Task_Targets : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }

   }
    public class Task_Status : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public string CommentContractor { get; set; }
        public DbRef Status { get; set; }
        public DbRef UserMA { get; set; }
        public DateTime ActualEndDate { get; set; }
        public DbRef CloseEvent { get; set; }

   }


}
    