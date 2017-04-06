using System;
using BitMobile.DbEngine;

namespace Test.Document
{
    public class Reminder : DbEntity
    {
        public bool Posted { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public DbRef Reminders { get; set; }
        public DbRef ViewReminder { get; set; }
        public string Comment { get; set; }
}
    public class Reminder_Photo : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef IDPhoto { get; set; }

   }


}
    