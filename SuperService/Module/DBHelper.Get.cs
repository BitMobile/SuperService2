using BitMobile.ClientModel3;
using BitMobile.DbEngine;
using System;
using Test.Document;
using DbRecordset = BitMobile.ClientModel3.DbRecordset;

//using Database = BitMobile.ClientModel3.Database;

namespace Test
{
    public static partial class DBHelper
    {
        /// <summary>
        ///     Method returns list of all events
        ///     Возвращает список всех событий
        /// </summary>
        public static DbRecordset GetEvents()
        {
            return GetEvents(DateTime.MinValue);
        }

        /// <summary>
        ///     Method returns list of events which plan start date biger then param
        ///     Получает список событий плановая дата начала которых больше передаваемого параметра
        /// </summary>
        /// <param name="eventSinceDate"> Дата начания с которой необходимо получить события</param>
        public static DbRecordset GetEvents(DateTime eventSinceDate)
        {
            var queryString = @"select
                                 event.Id,
                                 event.StartDatePlan,
                                 date(event.StartDatePlan) as startDatePlanDate, --date only
                                 event.EndDatePlan,
                                 ifnull(TypeDeparturesTable.description, '') as TypeDeparture,
                                 event.ActualStartDate as ActualStartDate, --4
                                 ifnull(Enum_StatusImportance.Description, '') as Importance,
                                 ifnull(Enum_StatusImportance.Name, '') as ImportanceName,
                                 ifnull(client.Description, '') as Description,
                                 ifnull(client.Address, '') as Address,
                                 ifnull(Enum_StatusyEvents.Name, '') as statusName,
                              --//имя значения статуса (служебное имя)
                                 ifnull(Enum_StatusyEvents.Description, '') as statusDescription
                              --//представление статуса
                               from
                                 Document_Event as event
                                   left join Catalog_Client as client
                                   on event.client = client.Id
                                     left join
                                        (select
                                            t1.Ref,
                                            Catalog_TypesDepartures.description
                                        from
                                           (select
                                               ref,
                                               min(ifnull(lineNumber,1)) as lineNumber
                                            from
                                               Document_Event_TypeDepartures
                                            where
                                               active = 1
                                            group by
                                               ref) as t1
                                                     left join Document_Event_TypeDepartures
                                                           on t1.ref= Document_Event_TypeDepartures.ref
                                                                   and t1.lineNumber = Document_Event_TypeDepartures.lineNumber
                                                     left join Catalog_TypesDepartures
                                                           on Document_Event_TypeDepartures.typeDeparture = Catalog_TypesDepartures.id) as TypeDeparturesTable
                                   on event.id = TypeDeparturesTable.Ref
                                        left join Enum_StatusImportance
                                             on event.Importance = Enum_StatusImportance.Id

                                left join Enum_StatusyEvents
                                    on event.status = Enum_StatusyEvents.Id
                                where
                                    event.DeletionMark = 0
                                    AND (event.StartDatePlan >= @eventDate OR (event.ActualEndDate > date('now','start of day') and Enum_StatusyEvents.Name IN (@statusDone, @statusCancel)))
                               order by
                                event.StartDatePlan";

            var query = new Query(queryString);

            query.AddParameter("eventDate", eventSinceDate);
            query.AddParameter("statusDone", EventStatusDoneName);
            query.AddParameter("statusCancel", EventStatusCancelName);

            return query.Execute();
        }

        /// <summary>
        ///     Полуает статистику по нарядам (событиям). Возвращает объект содержащий: количество нарядов на день, количество
        ///     закрытых
        ///     нарядов за день, количество нарядов с начала месяца, количество закрытых нарядов с начала месяца
        /// </summary>
        public static EventsStatistic GetEventsStatistic()
        {
            var statistic = new EventsStatistic();
            var query = new Query(@"select
                                      TOTAL(CASE
                                           when StartDatePlan >= date('now','start of day') and StartDatePlan < date('now','start of day', '+1 day') then 1
                                           else 0
                                      End) as DayTotalAmount,
                                       TOTAL(CASE
                                           when Enum_StatusyEvents.name like 'Done' and StartDatePlan >= date('now','start of day') and StartDatePlan < date('now','start of day', '+1 day') then 1
                                           else 0
                                      End) as DayCompleteAmout,
                                      TOTAL(CASE
                                           when StartDatePlan > date('now', 'start of month') and StartDatePlan < date('now', 'start of month', '+1 month') then 1
                                           else 0
                                      End) as MonthCompleteAmout,
                                      TOTAL(CASE
                                           when Enum_StatusyEvents.name like 'Done' and StartDatePlan > date('now', 'start of month') and StartDatePlan < date('now', 'start of month', '+1 month') then 1
                                           else 0
                                      End) as MonthTotalAmount
                                     from
                                         Document_Event as event
                                          left join Enum_StatusyEvents
                                            on event.Status = Enum_StatusyEvents.Id

                                   where
                                        event.DeletionMark = 0");
            var result = query.Execute();

            if (result.Next())
            {
                statistic.DayTotalAmount = (int)result["DayTotalAmount"];
                statistic.DayCompleteAmout = (int)result["DayCompleteAmout"];
                statistic.MonthTotalAmount = (int)result["MonthCompleteAmout"];
                statistic.MonthCompleteAmout = (int)result["MonthTotalAmount"];
            }

            return statistic;
        }

        /// <summary>
        ///     Получает полную информацию по событию
        /// </summary>
        /// <param name="eventID"> Идентификатор события</param>
        public static DbRecordset GetEventByID(string eventID)
        {
            var queryText = @"select
                                event.Id,                                         --гуид события
                                event.StartDatePlan,                              --плановая дата начала
                                Date(event.StartDatePlan) as StartDatePlanDate,
                                Time(event.StartDatePlan) as StartDatePlanTime,
                                TypeDeparturesTable.description as TypeDeparture, --вид работ - выбирается одна из табличной части
                                event.ActualStartDate,                            --фактическая дата начала
                                event.ActualEndDate,                              --фактическая дата конца
                                Enum_StatusImportance.Description as Importance,  --важность
                                Enum_StatusImportance.Name as ImportanceName,     --важность
                                event.Comment,
                                docSUm.sumFact,
                                docSUm.sumMaterials,
                                docSUm.sumServices,
                                docCheckList.Total as checkListTotal,             --общее количество вопросов в чеклисте
                                docCheckList.Answered as checkListAnswered,
                            --//количество отвеченных вопросов в чеклисте
                                docEquipment.Total as equipmentTotal,  --количество оборудования (задач)
                                docEquipment.Answered as equipmentAnswered,
                            --//количество оборудования (задач) с заполненным результатом
                                client.id as clientId,
                                client.Description as clientDescription,  --//имя клиента
                                client.Address as clientAddress,  --адрес клиента
                                docCheckList.Required as checkListRequired,
                            --// количество обязательных вопросов в чеклистах
                                docCheckList.RequiredAnswered as checkListRequiredAnswered,
                            --//количество отвеченных обязательных вопросов в чеклистах
                                case
                                    when ifnull(docCheckList.Required, 0) = ifnull(docCheckList.RequiredAnswered, 0) then 1
                                    else 0
                                end as checkListAllRequiredIsAnswered,
                            --//признак, что все обязательные вопросы в чеклистах отвечены
                                Enum_StatusyEvents.Name as statusName, --//наименование статуса (служебное имя)
                                Enum_StatusyEvents.Description as statusDescription, --//представление статуса +
                                event.DetailedDescription, --//описание события
                                Catalog_Contacts.Description as ContactVisitingDescription,
                                Catalog_Contacts.Id as contactId

                            from
                                Document_Event as event
                                    left join Catalog_Client as client
                                    on  event.id = @id and event.client = client.Id

                                    left join
                                        (select
                                              t1.Ref,
                                              Catalog_TypesDepartures.description
                                         from
                                             (select
                                                  ref,
                                                  min(lineNumber) as lineNumber
                                              from
                                                  Document_Event_TypeDepartures
                                              where
                                                  ref = @id
                                                  and active = 1
                                              group by
                                                  ref) as t1

                                           left join Document_Event_TypeDepartures
                                                on t1.ref= Document_Event_TypeDepartures.ref
                                                   and t1.lineNumber = Document_Event_TypeDepartures.lineNumber
                                           left join Catalog_TypesDepartures
                                                on Document_Event_TypeDepartures.typeDeparture =  Catalog_TypesDepartures.id) as TypeDeparturesTable
                                    on event.id = TypeDeparturesTable.Ref

                                    left join Enum_StatusImportance
                                       on event.Importance = Enum_StatusImportance.Id

                                    left join (select Document_Event_ServicesMaterials.Ref,
                                                   TOTAL(SumFact) as sumFact,
                                                   TOTAL(case when (select
                                                                      Catalog_RIM.Service
                                                                    from Catalog_RIM
                                                                    where Document_Event_ServicesMaterials.SKU = Catalog_RIM.Id) = 1
                                                         then SumFact else 0 end) as sumServices,
                                                   TOTAL(case when (select
                                                                      Catalog_RIM.Service
                                                                    from Catalog_RIM
                                                                    where Document_Event_ServicesMaterials.SKU = Catalog_RIM.Id) = 0
                                                         then SumFact else 0 end) as sumMaterials
                                               from Document_Event_ServicesMaterials
                                               where Document_Event_ServicesMaterials.Ref = @id group by Document_Event_ServicesMaterials.Ref ) as docSum
                                       on event.id = docSUm.ref

                                    left join (select
                                                   Document_Event_CheckList.Ref,
                                                   count(Document_Event_CheckList.Ref) as Total,
                                                   TOTAL(case when result is null or result = '' then 0 else 1 end) as Answered,
                                                   TOTAL(case when Required = 1 then 1 else 0 end) as Required,
                                                   TOTAL(case when Required = 1 and result <> ''  then 1 else 0 end) as RequiredAnswered
                                               from
                                                   Document_Event_CheckList
                                               where
                                                   Document_Event_CheckList.Ref = @id group by Document_Event_CheckList.Ref ) as docCheckList
                                       on event.id = docCheckList.ref

                                    left join (select
                                                   Document_Event_Equipments.Ref,
                                                   count(Document_Event_Equipments.Ref) as Total,
                                                   TOTAL(case when result is null or result = '' or result not in (select Id from Enum_ResultEvent where Name like 'Done') then 0 else 1 end) as Answered
                                               from
                                                   Document_Event_Equipments
                                               where
                                                   Document_Event_Equipments.Ref = @id group by Document_Event_Equipments.Ref ) as docEquipment
                                       on event.id = docEquipment.ref

                                    left join Catalog_Contacts
                                       on event.ContactVisiting = Catalog_Contacts.Id

                                    left join Enum_StatusyEvents
                                       on event.status = Enum_StatusyEvents.Id

                            where
                                event.id = @id  ";

            var query = new Query(queryText);
            query.AddParameter("id", eventID);
            var result = query.Execute();

            return result;
        }

        /// <summary>
        ///     Получает список задач события
        /// </summary>
        /// <param name="eventID"> Идентификатор события</param>
        public static DbRecordset GetTasksByEventID(string eventID)
        {
            var query = new Query("select " +
                                  "    tasks.Id,  " + //ид задачи
                                  "    tasks.Ref, " + //ид документа События
                                  "    tasks.Terget, " + //цель
                                  "    equipment.Description as equipmentDescription, " +
                                  "    ResultEvent.Description as ResultEventDescription, " +
                                  "    ResultEvent.Name as ResultEventName, " +
                                  "    case  " +
                                  "        when ResultEvent.Name like 'Done' then 1 " +
                                  "        else 0 " +
                                  "    end as isDone " +
                                  "  from " +
                                  "     Document_Event_Equipments as tasks " +
                                  "      left join Catalog_Equipment as equipment " +
                                  "       on tasks.Equipment = equipment.Id " +
                                  " " +
                                  "      left join Enum_ResultEvent as ResultEvent " +
                                  "       on tasks.Result = ResultEvent.Id " +
                                  "       " +
                                  " where " +
                                  "   tasks.Ref = @id  ");
            query.AddParameter("id", eventID);
            var result = query.Execute();

            return result;
        }

        /// <summary>
        ///     Возвращает перечень контактных лиц клиента
        /// </summary>
        public static DbRecordset GetContactsByClientID(string clientID)
        {
            var query = new Query("select " +
                                  "     Contacts.Id, " + //гиуд контакноголица
                                  "     Contacts.DeletionMark, " + // признак пометки удаления
                                  "     Contacts.Description, " + //имя
                                  "     Contacts.Position, " + // должность
                                  "     Contacts.Tel " + //телефон
                                  "from " +
                                  "  Catalog_Client_Contacts as ClientContacts " +
                                  "    left join Catalog_Contacts as Contacts " +
                                  "      on ClientContacts.Ref = @clientID " +
                                  "        and  ClientContacts.Contact = Contacts.Id " +
                                  " " +
                                  "where " +
                                  "    Contacts.DeletionMark = 0" +
                                  "    and ClientContacts.Ref = @clientID " +
                                  "    and ClientContacts.Actual = 0 ");
            //выбираем только неактуальных сотрудников, потому что актуальные являются уволенными

            query.AddParameter("clientID", clientID);

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает перечень оборудования клиента. Возвращается все оборудование во всех статусах
        /// </summary>
        /// <param name="clientID"> Идентификатор клиента</param>
        public static DbRecordset GetEquipmentByClientID(string clientID)
        {
            var query = new Query("select " +
                                  "    equipmentLastChangeDate.Equiement as equipmentID, " +
                                  "    equipmentLastChangeDate.period as lastChange, " +
                                  "    Catalog_Equipment.Description " +
                                  "" +
                                  "from " +
                                  "       (select " +
                                  "            clients, " +
                                  "            Equiement, " +
                                  "            MAX(period) as period " +
                                  "        from " +
                                  "            Catalog_Equipment_Equiements " +
                                  "        where " +
                                  "            clients = @clientID " +
                                  "        group by " +
                                  "            clients, Equiement) as equipmentLastChangeDate " +
                                  "" +
                                  "        left join Catalog_Equipment_Equiements " +
                                  "        on equipmentLastChangeDate.clients = Catalog_Equipment_Equiements.clients " +
                                  "        and equipmentLastChangeDate.Equiement = Catalog_Equipment_Equiements.Equiement " +
                                  "        and equipmentLastChangeDate.Period = Catalog_Equipment_Equiements.Period " +
                                  "" +
                                  "        left join Catalog_Equipment " +
                                  "        on equipmentLastChangeDate.Equiement = Catalog_Equipment.id");

            query.AddParameter("clientID", clientID);

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает список всех клиентов
        /// </summary>
        public static DbRecordset GetClients()
        {
            var query = new Query(@"select
                                        Catalog_Client.Id,
                                        Catalog_Client.Description,
                                        Catalog_Client.Address,
                                        Catalog_Client.Latitude,
                                        Catalog_Client.Longitude
                                    from
                                        Catalog_Client

                                  where
                                      Catalog_Client.DeletionMark = 0");

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает информацию по клиенту
        /// </summary>
        /// <param name="clientID">
        ///     Идентификатор клиента
        /// </param>
        public static DbRecordset GetClientByID(string clientID)
        {
            var query = new Query("select " +
                                  "    Catalog_Client.Id, " +
                                  "    Catalog_Client.Description, " +
                                  "    Catalog_Client.Address, " +
                                  "    Catalog_Client.Latitude, " +
                                  "    Catalog_Client.Longitude " +
                                  "from " +
                                  "    Catalog_Client " +
                                  "where " +
                                  "    Catalog_Client.Id = @clientID");

            query.AddParameter("clientID", clientID);
            return query.Execute();
        }

        /// <summary>
        ///     Получает список вопросов чек-листов по идентификаторы события
        /// </summary>
        /// <param name="eventID"> Идентификатор события</param>
        public static DbRecordset GetCheckListByEventID(string eventID)
        {
            var query = new Query("select " +
                                  "   checkList.Id as CheckListId, " +
                                  "   checkList.Ref as EventId, " +
                                  "   checkList.Required as Required, " + //признак обязательности
                                  "   checkList.Result as Result, " + //значение результата
                                  "   checkList.Action as ActionId, " +
                                  "   actions.Description as Description, " + //название пункта чек-листа
                                  "   typesDataParameters.Name as TypeName " +
                                  //Тип значения чек-листа: ValList - выбор из списка значений; Snapshot - фото; остальное понятно из названий
                                  "from " +
                                  "   Document_Event_CheckList as checkList " +
                                  "   left join Catalog_Actions as actions " +
                                  "     ON checkList.Ref = @eventId " +
                                  "       AND checkList.Action = actions.Id " +
                                  "    " +
                                  "   left join Enum_TypesDataParameters as typesDataParameters " +
                                  "     ON checkList.ActionType = TypesDataParameters.Id " +
                                  "    " +
                                  "where " +
                                  "    checkList.Ref = @eventId " +
                                  "order by checkList.LineNumber asc");

            query.AddParameter("eventId", eventID);
            return query.Execute();
        }

        public static DbRecordset GetClientParametersByClientId(string clientId)
        {
            var query = new Query("select " +
                                  "   parameters.Id as Id, " +
                                  "   parameters.Ref as ClientId, " +
                                  "   parameters.Val as Result, " +
                                  "   options.Id as OptionId, " + //значение результата
                                  "   options.Description as Description, " +
                                  "   typesDataParameters.Name as TypeName " +
                                  "from " +
                                  "   Catalog_Client_Parameters as parameters " +
                                  "   left join Catalog_ClientOptions as options " +
                                  "     ON parameters.Ref = @clientId " +
                                  "       AND parameters.Parameter = options.Id " +
                                  "    " +
                                  "   left join Enum_TypesDataParameters as typesDataParameters " +
                                  "     ON options.DataTypeParameter = TypesDataParameters.Id " +
                                  "    " +
                                  "where " +
                                  "    parameters.Ref = @clientId " +
                                  "order by parameters.LineNumber asc");

            query.AddParameter("clientId", clientId);
            return query.Execute();
        }

        /// <summary>
        ///     Получает список вариантов ответов для действий (вопросов)  с типом результата "выбор из списка"
        /// </summary>
        /// <param name="actionID"> Идентификатор действия</param>
        public static DbRecordset GetActionValuesList(string actionID)
        {
            var query = new Query("select " +
                                  "     Catalog_Actions_ValueList.Id, " + //идентификатор ответа
                                  "     Catalog_Actions_ValueList.Val " + //представление ответа
                                  "from " +
                                  "     Catalog_Actions_ValueList " +
                                  "where " +
                                  "     Catalog_Actions_ValueList.Ref = @actionID " +
                                  "order by LineNumber asc");
            query.AddParameter("actionID", actionID);
            return query.Execute();
        }

        public static DbRecordset GetClientOptionValuesList(string optionId)
        {
            DConsole.WriteLine($"{nameof(optionId)} = {optionId}");
            var query = new Query("select " +
                                  "     Catalog_ClientOptions_ListValues.Id, " + //идентификатор ответа
                                  "     Catalog_ClientOptions_ListValues.Val " + //представление ответа
                                  "from " +
                                  "     Catalog_ClientOptions_ListValues " +
                                  "where " +
                                  "     Catalog_ClientOptions_ListValues.Ref = @actionID " +
                                  "order by LineNumber asc");
            query.AddParameter("actionID", optionId);
            return query.Execute();
        }

        /// <summary>
        ///     Возвращает задачу по ее идентификатору
        /// </summary>
        /// <param name="taskID">
        ///     Идентификатор задачи
        /// </param>
        public static DbRecordset GetTaskById(string taskID)
        {
            var query = new Query("select  " +
                                  "      tasks.id as taskID, " + //гуид задачи
                                  "      tasks.Ref as EventID, " + //гуид наряда (события) к которому относится задача
                                  "      tasks.terget as Target, " + //Цель
                                  "      tasks.Comment as Comment, " + // комментарий
                                  "      equipment.Id as EquipmentId, " + //идентификатор оборудования
                                  "      equipment.Description as EquipmentDescription, " + //наименование оборудование
                                  "      Enum_ResultEvent.Name as resultName, " + //результат имя
                                  "      Enum_ResultEvent.Description as resultDescription, " +
                                  //результат представление
                                  "      TypeDeparturesTable.TypeDepartures " +
                                  //вид работ - выбирается первая активная из списка наряда
                                  " " +
                                  "from " +
                                  "    Document_Event_Equipments as tasks " +
                                  "       left join Catalog_Equipment as equipment " +
                                  "         on tasks.Id = @taskID " +
                                  "         and tasks.Equipment = equipment.Id " +
                                  " " +
                                  "       left join Enum_ResultEvent " +
                                  "          on tasks.Result = Enum_ResultEvent.Id " +
                                  " " +
                                  "        left join " +
                                  "                (select " +
                                  "                     Document_Event_TypeDepartures.Ref, " +
                                  "                     Catalog_TypesDepartures.description as TypeDepartures " +
                                  "                from " +
                                  "                    (select " +
                                  "                          ref, " +
                                  "                          min(lineNumber) as lineNumber " +
                                  "                     from " +
                                  "                          Document_Event_TypeDepartures " +
                                  "                     where " +
                                  "                          ref = (select Ref from Document_Event_Equipments where id = @taskID limit 1) " +
                                  "                          and active = 1 " +
                                  "                     group by " +
                                  "                          ref) as trueTypeDepartures " +
                                  " " +
                                  "                        left join Document_Event_TypeDepartures " +
                                  "                             on trueTypeDepartures.ref = Document_Event_TypeDepartures.ref " +
                                  "                                and trueTypeDepartures.lineNumber = Document_Event_TypeDepartures.lineNumber " +
                                  " " +
                                  "                        left join Catalog_TypesDepartures " +
                                  "                             on Document_Event_TypeDepartures.typeDeparture = Catalog_TypesDepartures.id) as TypeDeparturesTable " +
                                  " " +
                                  "            on tasks.Ref = TypeDeparturesTable.Ref " +
                                  " " +
                                  "where " +
                                  "      tasks.Id = @taskID");
            query.AddParameter("taskID", taskID);

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает суммы по АВР
        /// </summary>
        /// <param name="eventId">Идентификатор наряда</param>
        /// <param name="isPlanSums">Собирать фактическую или плановую сумму</param>
        /// <returns>
        ///     DbRecordset со следующими полями:
        ///     Sum - сумма по наряду
        ///     SumMaterials - сумма только по материалам
        ///     SumServices - сумма только по услугам
        /// </returns>
        public static DbRecordset GetCocSumsByEventId(string eventId, bool isPlanSums = false)
        {
            var column = isPlanSums ? "SumPlan" : "SumFact";
            var query = new Query("select " +
                                  $"    TOTAL({column}) as Sum, " +
                                  $"    TOTAL(case when Service = 0 then {column} else 0 end) as SumMaterials, " +
                                  $"    TOTAL(case when Service = 1 then {column} else 0 end) as SumServices " +
                                  "from " +
                                  "    Document_Event_ServicesMaterials " +
                                  "    join Catalog_RIM " +
                                  "        on Document_Event_ServicesMaterials.SKU = Catalog_RIM.Id " +
                                  "where Ref = @eventId");
            query.AddParameter("eventId", eventId);
            return query.Execute();
        }

        /// <summary>
        ///     Возвращает информацию по материалам
        /// </summary>
        /// <param name="eventId">Идентификатор наряда</param>
        /// <returns></returns>
        public static DbRecordset GetMaterialsByEventId(string eventId)
        {
            // TODO: Написать запрос

            var query = new Query("select " +
                                  "    Document_Event_ServicesMaterials.Id," +
                                  "    Document_Event_ServicesMaterials.SKU," +
                                  "    Document_Event_ServicesMaterials.Price," +
                                  "    AmountPlan," +
                                  "    SumPlan," +
                                  "    AmountFact," +
                                  "    SumFact," +
                                  "    Description," +
                                  "    Code," +
                                  "    Unit " +
                                  "from" +
                                  "    Document_Event_ServicesMaterials " +
                                  "    join Catalog_RIM " +
                                  "        on Document_Event_ServicesMaterials.SKU = Catalog_RIM.Id " +
                                  " where Catalog_RIM.Service = 0 and " +
                                  " (Document_Event_ServicesMaterials.AmountFact != 0 or Document_Event_ServicesMaterials.AmountPlan != 0) and" +
                                  "    Document_Event_ServicesMaterials.Ref = @eventId");
            query.AddParameter("eventId", eventId);
            return query.Execute();
        }

        /// <summary>
        ///     Возвращает информацию по услугам
        /// </summary>
        /// <param name="eventId">Идентификатор наряда</param>
        /// <returns></returns>
        public static DbRecordset GetServicesByEventId(string eventId)
        {
            var query = new Query("select " +
                                  "    Document_Event_ServicesMaterials.Id," +
                                  "    Document_Event_ServicesMaterials.SKU," +
                                  "    Document_Event_ServicesMaterials.Price," +
                                  "    AmountPlan," +
                                  "    SumPlan," +
                                  "    AmountFact," +
                                  "    SumFact," +
                                  "    Description," +
                                  "    Code," +
                                  "    Unit " +
                                  "from" +
                                  "    Document_Event_ServicesMaterials " +
                                  "       join Catalog_RIM" +
                                  "        on Document_Event_ServicesMaterials.SKU = Catalog_RIM.Id " +
                                  " where Catalog_RIM.Service = 1 and " +
                                  " (Document_Event_ServicesMaterials.AmountFact != 0 or Document_Event_ServicesMaterials.AmountPlan != 0) and" +
                                  "    Document_Event_ServicesMaterials.Ref = @eventId");
            query.AddParameter("eventId", eventId);
            return query.Execute();
        }

        /// <summary>
        /// Возвращает список Id УиМ
        /// </summary>
        /// <param name="eventId">Идентификатор ивента</param>
        /// <returns></returns>
        public static DbRecordset GetServicesAndMaterialsByEventId(string eventId)
        {
            var query = new Query("select Id from Document_Event_ServicesMaterials where Ref = @eventId");
            query.AddParameter("eventId", eventId);
            return query.Execute();
        }

        /// <summary>
        ///     Возвращает список материалов и услуг по указанному типу
        /// </summary>
        /// <param name="rimType">необходимый тип элементов работы и услуги</param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public static DbRecordset GetRIMByType(RIMType rimType, string eventId)
        {
            var query = new Query(@"select
                                      id,
                                      Description,
                                      Price,
                                      Unit,
                                      service
                                  from
                                      Catalog_RIM
                                  where
                                      deletionMark = 0
                                      and isFolder = 0
                                      and service = @rim_type
                                      and id not in " +
                                  @"(select SKU from Document_Event_ServicesMaterials where Document_Event_ServicesMaterials.Ref = @eventId) ");

            DConsole.WriteLine("rimType = " + rimType);
            query.AddParameter("rim_type", rimType == RIMType.Material ? 0 : 1);
            query.AddParameter("eventId", eventId);

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает строку табличной части "услуги и материалы" документа Событие с указанным идентификатором номенклатуры.
        ///     Используется для определения наличия в ТЧ документа номенклатуры с заданным ИД (проверка есть уже такая или нет)
        /// </summary>
        /// <param name="docEventID">Идентификатор документа событие</param>
        /// <param name="rimID">Идентификатор искомого элемента справочинка Товары и услуги</param>
        /// <returns>
        ///     null - если в указанном документе нету номенклатуры с указанным идентификатором;
        ///     Заполнненую структуру EventServicesMaterialsLine в случае если строка есть
        /// </returns>
        public static EventServicesMaterialsLine GetEventServicesMaterialsLineByRIMID(string docEventID, string rimID)
        {
            EventServicesMaterialsLine result = null;
            var queryText = "select " +
                            "    id, " +
                            "    LineNumber, " +
                            "    Ref, " +
                            "    SKU, " +
                            "    Price, " +
                            "    AmountPlan, " +
                            "    SumPlan, " +
                            "    AmountFact, " +
                            "    SumFact, " +
                            "    isDirty " +
                            "from " +
                            "    Document_Event_ServicesMaterials " +
                            "where " +
                            "    Document_Event_ServicesMaterials.Ref = @EventDocRef " +
                            "    and Document_Event_ServicesMaterials.SKU = @SKUID";

            var query = new Query(queryText);
            query.AddParameter("EventDocRef", docEventID);
            query.AddParameter("SKUID", rimID);

            var queryResult = query.Execute();

            if (queryResult.Next())
            {
                DConsole.WriteLine("зашли в обработку результата запроса");
                result = new EventServicesMaterialsLine
                {
                    ID = queryResult.GetString(0),
                    LineNumber = queryResult.GetInt32(1),
                    Ref = queryResult.GetString(2),
                    SKU = queryResult.GetString(3),
                    Price = queryResult.GetDecimal(4),
                    AmountPlan = queryResult.GetDecimal(5),
                    SumPlan = queryResult.GetDecimal(6),
                    AmountFact = queryResult.GetDecimal(7),
                    SumFact = queryResult.GetDecimal(8)
                };
            }

            return result;
        }

        /// <summary>
        ///     Возвращает строку табличной части "услуги и материалы" документа Событие по ее идентификатору
        /// </summary>
        /// <param name="lineID">Идентификатор строки ТЧ услуги и материалы</param>
        /// <returns>
        ///     строка табличной части
        /// </returns>
        public static DbRecordset GetEventServicesMaterialsLineById(string lineID)
        {
            var queryString = "select " +
                              "    id, " +
                              "    LineNumber, " +
                              "    Ref, " +
                              "    SKU, " +
                              "    Price, " +
                              "    AmountPlan, " +
                              "    AmountFact, " +
                              "    SumPlan, " +
                              "    SumFact " +
                              "from " +
                              "  Document_Event_ServicesMaterials " +
                              "where " +
                              "   Document_Event_ServicesMaterials.id = @lineId";

            var query = new Query(queryString);
            query.AddParameter("lineId", lineID);

            return query.Execute();
        }

        /// <summary>
        ///     Получает текущие остатки рюкзака монтажника
        /// </summary>
        /// <param name="userID">Идентификатор пользоателя для получения остатков рюкзака</param>
        /// <returns>
        ///     текущие остатки рюкзака
        /// </returns>
        public static DbRecordset GetUserBagByUserId(string userID)
        {
            var queryString = "select " +
                              "    Catalog_RIM.id,  " +
                              "    Catalog_RIM.Description, " +
                              "    Catalog_RIM.Unit,   " +
                              "    Catalog_User_Bag.Count " +
                              "  from " +
                              "    Catalog_User_Bag " +
                              "        left join Catalog_RIM " +
                              "            on Catalog_User_Bag.Materials = Catalog_RIM.id " +
                              "  where " +
                              "    Catalog_User_Bag.Ref = @userId";
            var query = new Query(queryString);
            query.AddParameter("userId", userID);

            return query.Execute();
        }

        public static DbRecordset GetAllMaterials()
        {
            var query =
                new Query(
                    "select id, Description, Unit, 0 as Count from Catalog_RIM where Service = 0 and DeletionMark = 0");
            return query.Execute();
        }

        /// <summary>
        ///     Получает список документов заявка на материалы
        /// </summary>
        /// <returns>
        ///     список документов заявка на материалы
        /// </returns>
        public static DbRecordset GetNeedMats()
        {
            var queryText = @"select
                               Document_NeedMat.id,
                               Document_NeedMat.Date,
                               Time(Document_NeedMat.Date) as docTime,
                               Document_NeedMat.Number,
                               Enum_StatsNeedNum.name as statusName,
                               Enum_StatsNeedNum.Description as statusDescription

                            from
                               Document_NeedMat
                                   left join Enum_StatsNeedNum
                                       on Document_NeedMat.StatsNeed = Enum_StatsNeedNum.id
                            order by
                               Document_NeedMat.Date desc";

            var query = new Query(queryText);

            return query.Execute();
        }

        /// <summary>
        ///     Получает информацию по строке материалов и услуг документа Наряд
        /// </summary>
        /// ///
        /// <param name="lineId">Идентификатор строки</param>
        public static DbRecordset GetServiceMaterialPriceByLineID(string lineId)
        {
            var query = new Query("select " +
                                  "      Document_Event_ServicesMaterials.Id, " +
                                  "      Document_Event_ServicesMaterials.SKU as RIMID, " +
                                  "      Catalog_RIM.Description, " +
                                  "      Document_Event_ServicesMaterials.Price, " +
                                  "      Document_Event_ServicesMaterials.AmountFact, " +
                                  "      Document_Event_ServicesMaterials.SumFact " +
                                  "from " +
                                  "    Document_Event_ServicesMaterials " +
                                  "         left join Catalog_RIM " +
                                  "            on Document_Event_ServicesMaterials.SKU = Catalog_RIM.Id " +
                                  " " +
                                  "where " +
                                  "    Document_Event_ServicesMaterials.id = @lineId");
            query.AddParameter("lineId", lineId);
            return query.Execute();
        }

        /// <summary>
        ///     Получает информацию по строке материалов и услуг документа Наряд
        /// </summary>
        /// ///
        /// <param name="rimId">Идентификатор строки</param>
        /// <param name="minimum">AmountFact по-умолчанию</param>
        public static DbRecordset GetServiceMaterialPriceByRIMID(string rimId, int minimum = 1)
        {
            var query = new Query("select " +
                                  "      Catalog_RIM.id, " +
                                  "      Catalog_RIM.Description, " +
                                  "      Catalog_RIM.Price, " +
                                  "      Catalog_RIM.Unit,  " +
                                  "      @minimum as AmountFact, " +
                                  "      Catalog_RIM.Price as SumFact " +
                                  "from " +
                                  "    Catalog_RIM " +
                                  " " +
                                  "where " +
                                  "    Catalog_RIM.id = @RIMId");
            query.AddParameter("RIMId", rimId);
            query.AddParameter("minimum", minimum);
            return query.Execute();
        }

        /// <summary>
        ///     возвращает перечень невыполненных нарядов с их координатами
        /// </summary>
        public static DbRecordset GetEventsLocation()
        {
            var queryText = "";
            var query = new Query(queryText);

            return query.Execute();
        }

        public static Event_Equipments GetEventEquipmentsById(string id)
        {
            var query = new Query("select * from Document_Event_Equipments where id = @id");
            query.AddParameter("id", id);
            var dbRecordset = query.Execute();
            return new Event_Equipments
            {
                Id = (DbRef)dbRecordset["Id"],
                Comment = (string)dbRecordset["Comment"],
                Equipment = (DbRef)dbRecordset["Equipment"],
                LineNumber = (int)dbRecordset["LineNumber"],
                Ref = (DbRef)dbRecordset["Ref"],
                Result = (DbRef)dbRecordset["Result"],
                SID = (DbRef)dbRecordset["SID"],
                Terget = (string)dbRecordset["Terget"]
            };
        }

        public static DbRecordset GetClientLocationByClientId(string clientId)
        {
            var query = new Query(@"select
                                    client.Description as Description,
                                    client.Latitude as Latitude,
                                    client.Longitude as Longitude
                                from
                                    Catalog_Client as client
                                where
                                    ifnull(client.Latitude, 0) != 0 and client.Latitude not like ''
                                    and
                                    ifnull(client.Longitude, 0) != 0 and client.Longitude not like ''
                                    and client.Id = @clientId");

            query.AddParameter("clientId", clientId);

            return query.Execute();
        }

        public static DbRecordset GetEventsLocationToday()
        {
            var query = new Query(@"select
                                        client.Description as Description,
                                        client.Latitude as Latitude,
                                        client.Longitude as Longitude
                                    from
                                        Document_Event as event
                                    left join Catalog_Client as client
                                        on event.client = client.id
                                    where
                                        event.DeletionMark = 0
                                        and date(event.StartDatePlan) = date('now','start of day')
                                        and client.Latitude != 0
                                        and client.Longitude != 0");

            return query.Execute();
        }

        /// <summary>
        /// Получаем значение связанное с тем,
        /// что используется ли рюкзак или нет.
        /// возращяет булевское значение упакованное в object
        /// </summary>
        /// <returns>true используется рюкзак монтажника,
        /// false если не используется. null если таблица пустая или не найдено значение</returns>
        public static bool GetIsUseServiceBag()
        {
            var query = new Query(@"SELECT LogicValue
                                    FROM Catalog_SettingMobileApplication
                                    WHERE Description = 'UsedServiceBag' ");

            var dbResult = query.Execute();

            return dbResult.Next() && dbResult.GetBoolean(0);
        }

        /// <summary>
        /// Получаем значение связанное с тем, включено ли отображение цен или нет
        /// возращяет булевское значение упакованное в object
        /// </summary>
        /// <returns>true , если включено отображение цен
        /// false если не используется. null если таблица пустая или не найдено значение</returns>
        public static bool GetIsUsedCalculateService()
        {
            var query = new Query(@"SELECT LogicValue
                                    FROM Catalog_SettingMobileApplication
                                    WHERE Description = 'UsedCalculateService' ");

            var dbResult = query.Execute();

            return dbResult.Next() && dbResult.GetBoolean(0);
        }

        /// <summary>
        /// Получаем значение связанное с тем, включено ли отображение цен или нет
        /// возращяет булевское значение упакованное в object
        /// </summary>
        /// <returns>true , если включено отображение цен
        /// false если не используется. null если таблица пустая или не найдено значение</returns>
        public static bool GetIsUsedCalculateMaterials()
        {
            var query = new Query(@"SELECT LogicValue
                                    FROM Catalog_SettingMobileApplication
                                    WHERE Description = 'UsedCalculateMaterials' ");

            var dbResult = query.Execute();

            //
            /*dbResult.Next();
            var res1 = dbResult.GetBoolean(0);
            var res2 = (bool) dbResult["LogicValue"];
            DConsole.WriteLine("GetIsUsedCalculateMaterials() getBoolean = " + res1);
            DConsole.WriteLine("GetIsUsedCalculateMaterials() (bool) = " + res1);

            return true;*/

            return dbResult.Next() && dbResult.GetBoolean(0);
        }

        public static DbRecordset GetRIMFromBag(RIMType type = RIMType.Material)
        {
            var query = new Query(@"SELECT
                                        Catalog_RIM.Id as id,
                                        Catalog_RIM.Description as Description,
                                        Catalog_RIM.Price as Price,
                                        Catalog_RIM.Unit as Unit,
                                        Catalog_RIM.service
                                    FROM
                                           Catalog_User_Bag
                                              LEFT JOIN Catalog_Rim
                                                 ON Catalog_User_Bag.Materials =  Catalog_RIM.Id
                                    WHERE Catalog_RIM.IsFolder = 0 and
                                          Catalog_RIM.DeletionMark = 0 and
                                           service = @isService ");

            query.AddParameter("isService", (int)type);

            return query.Execute();
        }

        /// <summary>
        ///     возвращает параметры оборудования с их значениями по ИД оборудования
        /// </summary>
        ///
        /// <param name="equipmentId">Идентификатор оборудования</param>
        public static DbRecordset GetEquipmentParametersById(string equipmentId)
        {
            var queryText = @"select
                               param.Description as Parameter,
                               equipParam.val as Value
                            from
                               Catalog_Equipment_Parameters as equipParam
                                  left join Catalog_EquipmentOptions as param
                                     on equipParam.Ref = @equipId and equipParam.Parameter = param.Id

                                where
                                    equipParam.Ref = @equipId and param.DeletionMark = 0";

            var query = new Query(queryText);
            query.AddParameter("equipId", equipmentId);

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает историю оборудования начиная с указанноЙ даты
        /// </summary>
        ///
        /// <param name="equpmentId">Идентификатор оборудования</param>
        /// <param name="afterDate">Дата начиная с которой выводится история</param>
        public static DbRecordset GetEquipmentHistoryById(string equpmentId, DateTime afterDate)
        {
            DConsole.WriteLine("GetEquipmentHistoryById");
            var queryText = "select " +
                            "   history.Period as Date, " +
                            "   history.Target as Description, " +
                            "   Enum_ResultEvent.Description as result, " +
                            "   Enum_ResultEvent.Name as ResultName " +
                            "from " +
                            "   Catalog_Equipment_EquiementsHistory as history " +
                            "       left join Enum_ResultEvent " +
                            "            on history.Result = Enum_ResultEvent.Id " +
                            "where " +
                            "     history.Equiements = @equipmentId " +
                            "     and history.Period > date(@startDate) " +
                            " " +
                            " order by Date desc";

            var query = new Query(queryText);
            query.AddParameter("equipmentId", equpmentId);
            query.AddParameter("startDate", afterDate);

            DConsole.WriteLine("GetEquipmentHistoryById");

            return query.Execute();
        }

        //TODO: удалить метод GetEquipmentById когда починят getObject у dbEntity

        /// <summary>
        ///     Возвращает описание оборудования
        /// </summary>
        ///
        /// <param name="equipmentId">Идентификатор оборудования</param>
        public static DbRecordset GetEquipmentById(string equipmentId)
        {
            var queryText = "select " +
                            "   Description " +
                            "from " +
                            "   Catalog_Equipment " +
                            "where " +
                            "   id = @equipmentId";
            var query = new Query(queryText);
            query.AddParameter("equipmentId", equipmentId);

            return query.Execute();
        }

        public static DbRecordset GetContactById(string contactId)
        {
            var query = new Query("select * " +
                                  "from Catalog_Contacts " +
                                  "where id = @contactId");
            query.AddParameter("contactId", contactId);
            return query.Execute();
        }

        public static DbRecordset GetTotalFinishedRequireQuestionByEventId(string eventId)
        {
            var query = new Query(@"select
                                        count(*) as count
                                    from
                                        Document_Event_CheckList
                                        where
                                        Ref = @ref and Required = 1 and ifnull(result, '') like '' ");

            query.AddParameter("ref", eventId);

            return query.Execute();
        }

        /// <summary>
        /// Получаем Id и Description пользователя
        /// по его UserName
        /// </summary>
        /// <param name="userName">UserName пользователя</param>
        /// <returns>Возращяет DbRecordset с данными</returns>
        public static DbRecordset GetUserInfoByUserName(string userName)
        {
            var query = new Query(@"select
                                    Id,
                                    Description
                                    from Catalog_User
                                    where UserName like @userName");
            query.AddParameter("userName", userName);

            return query.Execute();
        }

        public static DbRecordset GetSettings()
        {
            var query = new Query(@"select Description, LogicValue, NumericValue
                                    from Catalog_SettingMobileApplication
                                    where DeletionMark = 0");

            return query.Execute();
        }

        /// <summary>
        /// Достать максимальное число из колонки таблицы
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="column">Имя колонки</param>
        /// <param name="whereColumnName">Имя колонки, по которой будет поиск</param>
        /// <param name="whereColumnValue">Значения колонки, по которой будет поиск</param>
        /// <returns>Максимальное число</returns>
        public static int GetMaxNumberFromTableInColumn(string table, string column, string whereColumnName,
            string whereColumnValue)
        {
            var query = new Query($"select ifnull(max({column}), 0) as max from {table} where {whereColumnName} = @where");
            query.AddParameter("where", whereColumnValue);
            return (int)query.ExecuteScalar();
        }

        /// <summary>
        /// Получить UserId из БД
        /// </summary>
        /// <returns>Возращается строка содержащая UserId, если в БД не найден UserId возращается пустая строка.</returns>
        public static string GetUserId()
        {
            var result = new Query("SELECT UserId FROM ___UserInfo  LIMIT 1").Execute();
            return result.Next() ? (string)result["UserId"] : "";
        }

        /// <summary>
        /// Получаем актуальные координаты.
        /// </summary>
        /// <param name="timeSpan">Указывает промежуток, который указывает,
        ///  из какого временного диапазона выбирать актуальные координаты.
        /// По умолчанию, координаты выбираются из всего временного диапазона.</param>
        /// <returns>Возращяет координаты. Нулевые координаты указывают, что нет актуальных координат.</returns>
        public static DbRecordset GetCoordinate(uint timeSpan = uint.MaxValue)
        {
            return new Query($@"select id, ifnull(Latitude, 0.0) as Latitude, ifnull(Longitude, 0.0) as Longitude, max(datetime(EndTime,'localtime')) as EndTime from ___DbLocations
                                where datetime(EndTime, 'localtime') between datetime('now','localtime', '-{timeSpan} minutes') and datetime('now', 'localtime')").Execute();
        }
    }
}