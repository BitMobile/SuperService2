using System;
using BitMobile.ClientModel3;

namespace Test
{
    class EventListElement
    {
        public string Id { get; set; }
        public DateTime StartDatePlan { get; set; }
        public DateTime StartDatePlanDate { get; set; } //date only - begining of the day
        public string TypeDeparture { get; set; }
        public DateTime ActualStartDate { get; set; }
        public string Importance { get; set; }
        public string ImportanceName { get; set; }
        public string ClientDescription { get; set; }
        public string ClientAddress { get; set; }


        public static EventListElement CreateFromRecordSet(DbRecordset recordSet)
        {
            var eventElement = new EventListElement();
            eventElement.Id = recordSet.GetString(0);
            eventElement.StartDatePlan = recordSet.GetDateTime(1);
            eventElement.StartDatePlanDate = recordSet.GetDateTime(2);
            eventElement.TypeDeparture = recordSet.GetString(3);
            eventElement.ActualStartDate = recordSet.GetDateTime(4);
            eventElement.Importance = recordSet.GetString(5);
            eventElement.ImportanceName = recordSet.GetString(6);
            eventElement.ClientDescription = recordSet.GetString(7);
            eventElement.ClientAddress = recordSet.GetString(8);
            DConsole.WriteLine(nameof(CreateFromRecordSet));

            return eventElement;
        }
    }

 
}
