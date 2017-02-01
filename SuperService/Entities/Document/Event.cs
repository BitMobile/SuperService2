using System;
using BitMobile.DbEngine;

namespace Test.Document
{
    public class Event : DbEntity
    {
        public bool Posted { get; set; }
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; }
        public string ApplicationJustification { get; set; }
        public DbRef Client { get; set; }
        public string DivisionSource { get; set; }
        public DbRef KindEvent { get; set; }
        public bool AnySale { get; set; }
        public bool AnyProblem { get; set; }
        public DateTime StartDatePlan { get; set; }
        public DateTime EndDatePlan { get; set; }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }
        public DbRef Author { get; set; }
        public DbRef UserMA { get; set; }
        public string Comment { get; set; }
        public string DetailedDescription { get; set; }
        public string CommentContractor { get; set; }
        public string TargInteractions { get; set; }
        public string ResultInteractions { get; set; }
        public DbRef Status { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime GPSTime { get; set; }
        public DbRef ContactVisiting { get; set; }
        public DbRef TypesDepartures { get; set; }
        public DbRef Importance { get; set; }
}
    public class Event_Files : DbEntity
    {
        public DbRef Id { get; set; }
        public DbRef Ref { get; set; }
        public int LineNumber { get; set; }
        public string FullFileName { get; set; }
        public DbRef FileName { get; set; }

   }
    public class Event_Equipments : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef Equipment { get; set; }
        public string Terget { get; set; }
        public DbRef Result { get; set; }
        public string Comment { get; set; }
        public DbRef SID { get; set; }

   }
    public class Event_Photos : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef UIDPhoto { get; set; }
        public DbRef Equipment { get; set; }

   }
    public class Event_Parameters : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef Parameter { get; set; }
        public string Val { get; set; }

   }
    public class Event_CheckList : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef Action { get; set; }
        public DbRef CheckListRef { get; set; }
        public string Result { get; set; }
        public DbRef ActionType { get; set; }
        public bool Required { get; set; }

   }
    public class Event_TypeDepartures : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef TypeDeparture { get; set; }
        public bool Active { get; set; }

   }
    public class Event_ServicesMaterials : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public DbRef SKU { get; set; }
        public decimal Price { get; set; }
        public decimal AmountPlan { get; set; }
        public decimal SumPlan { get; set; }
        public decimal AmountFact { get; set; }
        public decimal SumFact { get; set; }

   }
    public class Event_EventFiskalProperties : DbEntity
    {
        public DbRef Id { get; set; }
        public int LineNumber { get; set; }
        public DbRef Ref { get; set; }
        public int CheckNumber { get; set; }
        public DateTime Date { get; set; }
        public int ShiftNumber { get; set; }
        public string NumberFtpr { get; set; }
        public int PaymentType { get; set; }
        public decimal PaymentAmount { get; set; }
        public DbRef User { get; set; }

   }


}
    