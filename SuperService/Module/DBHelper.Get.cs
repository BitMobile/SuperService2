using BitMobile.ClientModel3;
using BitMobile.DbEngine;
using System;
using Test.Catalog;
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

        public static bool CheckRole(String RoleDesc)
        {
            var RecSetUser = GetUserInfoByUserName(Settings.User);
            var role = (((User)((DbRef)RecSetUser["Id"])?.GetObject())?.Role);
            //Utils.TraceMessage($"{}");
            //DConsole.WriteLine();
            var queryString = @"Select CR.Id,CR.Description, EW.Name
                            from Catalog_Roles as CR
                            Left Join Catalog_RoleWebactions as CRW
                            On CR.Id = CRW.Role
                            Left Join Enum_Webactions as EW
                            On EW.Id = CRW.WebAction
                            Where CR.Id = @RoleId And EW.Name = @WebActionRoleName ";
            var query = new Query(queryString);
            query.AddParameter("RoleId", $"{role}");
            query.AddParameter("WebActionRoleName", RoleDesc);
            int count = query.ExecuteCount();
            if (count > 0) return true;
            return false;
        }

        /// <summary>
        ///     Method returns list of events which plan start date biger then param
        ///     Получает список событий плановая дата начала которых больше передаваемого параметра
        /// </summary>
        /// <param name="eventSinceDate"> Дата начания с которой необходимо получить события</param>
        public static DbRecordset GetEvents(DateTime eventSinceDate)
        {
            bool EventsShowSubordinate = CheckRole("EventsShowSubordinate");
            var queryString = @"select
                                 event.Id,
                                 event.StartDatePlan,
                                 date(event.StartDatePlan) as startDatePlanDate, --date only
                                 event.EndDatePlan,
                                 ifnull(ETE.description, '') as TypeDeparture,
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
                                    left join Catalog_User as CU
                                        ON CU.Id = event.UserMA
                                  LEFT JOIN Enum_TypesEvents AS ETE
                                    ON ETE.Id = event.KindEvent

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
                                where ";
            var RecSetUser = GetUserInfoByUserName(Settings.User);
            if (Filter.SelectedFilterId != null)
            {
                String idFilterSql = GetFiltersSql();
                Utils.TraceMessage(idFilterSql);
                if (idFilterSql.Trim() != "")
                {
                    queryString += @"" + idFilterSql + " AND ";
                }
            }

            String refUser = $"{(DbRef)RecSetUser["Id"]}";
            queryString += @"(event.UserMA = '" + refUser + "'";
            if (EventsShowSubordinate)
            {
                queryString += @" OR CU.Manager = '" + refUser + "') AND";
            }
            else
            {
                queryString += @") AND ";
            }
            queryString += @" event.DeletionMark = 0
                                    AND (event.StartDatePlan >= @eventDate
                                              AND ((event.ActualEndDate > date('now','start of day') and Enum_StatusyEvents.Name IN (@statusDone, @statusCancel))
                                              OR (Enum_StatusyEvents.Name IN (@statusAppointed, @statusInWork))))

                               order by
                                event.StartDatePlan";

            var query = new Query(queryString);
            query.AddParameter("eventDate", eventSinceDate);
            query.AddParameter("statusDone", EventStatusDoneName);
            query.AddParameter("statusCancel", EventStatusCancelName);
            query.AddParameter("statusAppointed", EventStatusAppointed);
            query.AddParameter("statusInWork", EventStatusInWork);
            return query.Execute();
        }

        /// <summary>
        ///     Полуает статистику по нарядам (событиям). Возвращает объект содержащий: количество нарядов на день, количество
        ///     закрытых
        ///     нарядов за день, количество нарядов с начала месяца, количество закрытых нарядов с начала месяца
        /// </summary>
        public static String GetFiltersSql()
        {
            var query = new Query(@"SELECT
                                    Query
                                    FROM Catalog_MobileTaskFilters
                                    Where Id = '" + Filter.SelectedFilterId + "'");
            //Utils.TraceMessage(query.ExecuteScalar().ToString());
            return query.ExecuteScalar().ToString();
        }

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
            var queryText = @"SELECT
                                  event.Id,
                                  --гуид события
                                  event.StartDatePlan,
                                  --плановая дата начала
                                  Date(event.StartDatePlan)         AS StartDatePlanDate,
                                  Time(event.StartDatePlan)         AS StartDatePlanTime,
                                  ETE.description   AS TypeDeparture,
                                  --вид работ - выбирается одна из табличной части
                                  event.ActualStartDate,
                                  --фактическая дата начала
                                  event.ActualEndDate,
                                  --фактическая дата конца
                                  Enum_StatusImportance.Description AS Importance,
                                  --важность
                                  Enum_StatusImportance.Name        AS ImportanceName,
                                  --важность
                                  event.UserMA                      AS Resp,
                                  event.Comment,
                                  docSUm.sumFact,
                                  docSUm.sumMaterials,
                                  docSUm.sumServices,
                                  docCheckList.Total                AS checkListTotal,
                                  --общее количество вопросов в чеклисте
                                  docCheckList.Answered             AS checkListAnswered,
                                  --//количество отвеченных вопросов в чеклисте
                                  client.id                         AS clientId,
                                  client.Description                AS clientDescription,
                                  --//имя клиента
                                  client.Address                    AS clientAddress,
                                  --адрес клиента
                                  docCheckList.Required             AS checkListRequired,
                                  --// количество обязательных вопросов в чеклистах
                                  docCheckList.RequiredAnswered     AS checkListRequiredAnswered,
                                  --//количество отвеченных обязательных вопросов в чеклистах
                                  CASE
                                  WHEN ifnull(docCheckList.Required, 0) = ifnull(docCheckList.RequiredAnswered, 0)
                                    THEN 1
                                  ELSE 0
                                  END                               AS checkListAllRequiredIsAnswered,
                                  --//признак, что все обязательные вопросы в чеклистах отвечены
                                  Enum_StatusyEvents.Name           AS statusName,
                                  --//наименование статуса (служебное имя)
                                  Enum_StatusyEvents.Description    AS statusDescription,
                                  --//представление статуса +
                                  event.DetailedDescription,
                                  --//описание события
                                  Catalog_Contacts.Description      AS ContactVisitingDescription,
                                  Catalog_Contacts.Id               AS contactId

                                FROM
                                  Document_Event AS event
                                  LEFT JOIN Catalog_Client AS client
                                    ON event.id = @id AND event.client = client.Id
                                  LEFT JOIN Enum_TypesEvents AS ETE
                                    ON ETE.Id = event.KindEvent
                                  LEFT JOIN
                                  (SELECT
                                     t1.Ref,
                                     Catalog_TypesDepartures.description
                                   FROM
                                     (SELECT
                                        ref,
                                        min(lineNumber) AS lineNumber
                                      FROM
                                        Document_Event_TypeDepartures
                                      WHERE
                                        ref = @id
                                        AND active = 1
                                      GROUP BY
                                        ref) AS t1

                                     LEFT JOIN Document_Event_TypeDepartures
                                       ON t1.ref = Document_Event_TypeDepartures.ref
                                          AND t1.lineNumber = Document_Event_TypeDepartures.lineNumber
                                     LEFT JOIN Catalog_TypesDepartures
                                       ON Document_Event_TypeDepartures.typeDeparture = Catalog_TypesDepartures.id) AS TypeDeparturesTable
                                    ON event.id = TypeDeparturesTable.Ref

                                  LEFT JOIN Enum_StatusImportance
                                    ON event.Importance = Enum_StatusImportance.Id

                                  LEFT JOIN (SELECT
                                               Document_Event_ServicesMaterials.Ref,
                                               TOTAL(SumFact)    AS sumFact,
                                               TOTAL(CASE WHEN (SELECT Catalog_RIM.Service
                                                                FROM Catalog_RIM
                                                                WHERE Document_Event_ServicesMaterials.SKU = Catalog_RIM.Id) = 1
                                                 THEN SumFact
                                                     ELSE 0 END) AS sumServices,
                                               TOTAL(CASE WHEN (SELECT Catalog_RIM.Service
                                                                FROM Catalog_RIM
                                                                WHERE Document_Event_ServicesMaterials.SKU = Catalog_RIM.Id) = 0
                                                 THEN SumFact
                                                     ELSE 0 END) AS sumMaterials
                                             FROM Document_Event_ServicesMaterials
                                             WHERE Document_Event_ServicesMaterials.Ref = @id
                                             GROUP BY Document_Event_ServicesMaterials.Ref) AS docSum
                                    ON event.id = docSUm.ref

                                  LEFT JOIN (SELECT
                                               Document_Event_CheckList.Ref,
                                               count(Document_Event_CheckList.Ref) AS Total,
                                               TOTAL(CASE WHEN result IS NULL OR result = ''
                                                 THEN 0
                                                     ELSE 1 END)                   AS Answered,
                                               TOTAL(CASE WHEN Required = 1
                                                 THEN 1
                                                     ELSE 0 END)                   AS Required,
                                               TOTAL(CASE WHEN Required = 1 AND result <> ''
                                                 THEN 1
                                                     ELSE 0 END)                   AS RequiredAnswered
                                             FROM
                                               Document_Event_CheckList
                                             WHERE
                                               Document_Event_CheckList.Ref = @id
                                             GROUP BY Document_Event_CheckList.Ref) AS docCheckList
                                    ON event.id = docCheckList.ref

                                  LEFT JOIN Catalog_Contacts
                                    ON event.ContactVisiting = Catalog_Contacts.Id

                                  LEFT JOIN Enum_StatusyEvents
                                    ON event.status = Enum_StatusyEvents.Id

                                WHERE
                                  event.id = @id  ";

            var query = new Query(queryText);
            query.AddParameter("id", eventID);
            var result = query.Execute();

            return result;
        }

        /// <summary>
        ///     Получает список задач наряда или клиента.
        /// </summary>
        /// <param name="eventId"> Идентификатор наряда.</param>
        /// <param name="clientId"> Индетефикатор клиента.</param>
        /// <param name="userId"></param>
        public static DbRecordset GetTaskList(string eventId, string clientId)
        {
            var query = new Query(@"SELECT
                                      Task.Id          AS Id,
                                      Task.Description AS Description,
                                      Task.TaskType    AS TaskType,
                                      Status.Name      AS StatusName
                                    FROM
                                      _Document_Task AS Task
                                      INNER JOIN
                                      _Document_Task_Status AS Task_Status
                                        ON Task_Status.Ref = Task.Id
                                      INNER JOIN
                                      _Enum_StatusTasks AS Status
                                        ON Task_Status.Status = Status.Id
                                    WHERE
                                      ((Task.Event = @eventId AND Task.Client = @clientId AND Status.Name LIKE 'New'
                                        OR
                                        Task.Event = @eventId AND Task.Client = @clientId AND Status.Name NOT LIKE 'New'
                                        AND Task_Status.CloseEvent = @eventId)
                                       OR
                                       (Task.Client LIKE @clientId
                                        AND Task.Event
                                            = '@ref[Document_Event]:00000000-0000-0000-0000-000000000000'
                                        AND Status.Name LIKE 'New'
                                        OR
                                        Task.Client LIKE @clientId
                                        AND Task.Event
                                            = '@ref[Document_Event]:00000000-0000-0000-0000-000000000000'
                                        AND Status.Name NOT LIKE 'New'
                                        AND Task_Status.CloseEvent = @eventId))
                                      AND Task.DeletionMark == 0");

            query.AddParameter("eventId", eventId);
            query.AddParameter("clientId", clientId);

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает перечень контактных лиц клиента
        /// </summary>
        public static DbRecordset GetContactsByClientID(string clientID)
        {
            var query = new Query(@"SELECT
                                      Contacts.Id,
                                      Contacts.DeletionMark,
                                      Contacts.Description,
                                      Contacts.Position,
                                      Contacts.Tel
                                    FROM
                                      Catalog_Client_Contacts AS ClientContacts
                                      LEFT JOIN Catalog_Contacts AS Contacts
                                        ON ClientContacts.Ref = @clientID
                                           AND ClientContacts.Contact = Contacts.Id

                                    WHERE
                                      Contacts.DeletionMark = 0
                                      AND ClientContacts.Ref = @clientID
                                      AND ifnull(ClientContacts.Actual, 0) = 0 ");

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
                                  "    equipmentLastChangeDate.Equipment as equipmentID, " +
                                  "    equipmentLastChangeDate.period as lastChange, " +
                                  "    Catalog_Equipment.Description " +
                                  "" +
                                  "from " +
                                  "       (select " +
                                  "            clients, " +
                                  "            Equipment, " +
                                  "            MAX(period) as period " +
                                  "        from " +
                                  "            Catalog_Equipment_Equipments " +
                                  "        where " +
                                  "            clients = @clientID " +
                                  "        group by " +
                                  "            clients, Equipment) as equipmentLastChangeDate " +
                                  "" +
                                  "        left join Catalog_Equipment_Equipments " +
                                  "        on equipmentLastChangeDate.clients = Catalog_Equipment_Equipments.clients " +
                                  "        and equipmentLastChangeDate.Equipment = Catalog_Equipment_Equipments.Equipment " +
                                  "        and equipmentLastChangeDate.Period = Catalog_Equipment_Equipments.Period " +
                                  "" +
                                  "        left join Catalog_Equipment " +
                                  "        on equipmentLastChangeDate.Equipment = Catalog_Equipment.id");

            query.AddParameter("clientID", clientID);

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает список всех клиентов
        /// </summary>
        public static DbRecordset GetClients(String filter = "")
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
                                      Catalog_Client.DeletionMark = 0
                                    ");
            if (!String.IsNullOrEmpty(filter))
            {
                query.Text += $@" AND (Contains(Description,'{filter}') OR Contains(Address,'{filter}'))";
            }
            query.Text += $@" LIMIT 100";
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

        public static DbRecordset GetClientParametersByClientId(string clientId, string profileId)
        {
            var query = new Query(@"SELECT
                                  parameters.Id            AS Id,
                                  parameters.Ref           AS ClientId,
                                  parameters.Val           AS Result,
                                  options.Id               AS OptionId,
                                  options.Description      AS Description,
                                  typesDataParameters.Name AS TypeName,
                                  options.Profile       AS Profile
                                FROM
                                  Catalog_Client_Parameters AS parameters
                                  LEFT JOIN Catalog_ClientOptions AS options
                                    ON parameters.Parameter = options.Id
                                  LEFT JOIN Enum_TypesDataParameters AS typesDataParameters
                                    ON options.DataTypeParameter = TypesDataParameters.Id
                                WHERE
                                  parameters.Ref = @clientId AND ifnull(options.Profile,'@ref[Catalog_Profile]:00000000-0000-0000-0000-000000000000') = @profile
                                ORDER BY parameters.LineNumber
                                  ASC");

            query.AddParameter("clientId", clientId);
            query.AddParameter("profile", profileId);
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
            var query = new Query(@"SELECT
                                      Task.Id                       AS Id,
                                      Task.Event                    AS Event,
                                      Task.Description              AS Description,
                                      Task.TaskType                 AS TaskType,
                                      Task.Equipment                AS EquipmentId,
                                      Status.Name                   AS Status,
                                      Equipment.Description         AS Equipment,
                                      Task_Status.CommentContractor AS Comment
                                    FROM
                                      _Document_Task AS Task
                                      LEFT JOIN
                                      _Document_Task_Status AS Task_Status
                                        ON Task.Id = Task_Status.Ref
                                      LEFT JOIN _Enum_StatusTasks AS Status
                                        ON Task_Status.Status = Status.Id
                                      LEFT JOIN _Catalog_Equipment AS Equipment
                                        ON Task.Equipment = Equipment.Id
                                    WHERE
                                      Task.Id = @taskID");
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
        ///     Возвращает список Id УиМ
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

        [Obsolete("В версии 3.1.3.0 больше не используется," +
                  " поменялся механизм задач.")]
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
        ///     Получаем значение связанное с тем,
        ///     что используется ли рюкзак или нет.
        ///     возращяет булевское значение упакованное в object
        /// </summary>
        /// <returns>
        ///     true используется рюкзак монтажника,
        ///     false если не используется. null если таблица пустая или не найдено значение
        /// </returns>
        public static bool GetIsUseServiceBag()
        {
            var query = new Query(@"SELECT LogicValue
                                    FROM Catalog_SettingMobileApplication
                                    WHERE Description = 'UsedServiceBag' ");

            var dbResult = query.Execute();

            return dbResult.Next() && dbResult.GetBoolean(0);
        }

        /// <summary>
        ///     Получаем значение связанное с тем, включено ли отображение цен или нет
        ///     возращяет булевское значение упакованное в object
        /// </summary>
        /// <returns>
        ///     true , если включено отображение цен
        ///     false если не используется. null если таблица пустая или не найдено значение
        /// </returns>
        public static bool GetIsUsedCalculateService()
        {
            var query = new Query(@"SELECT LogicValue
                                    FROM Catalog_SettingMobileApplication
                                    WHERE Description = 'UsedCalculateService' ");

            var dbResult = query.Execute();

            return dbResult.Next() && dbResult.GetBoolean(0);
        }

        /// <summary>
        ///     Получаем значение связанное с тем, включено ли отображение цен или нет
        ///     возращяет булевское значение упакованное в object
        /// </summary>
        /// <returns>
        ///     true , если включено отображение цен
        ///     false если не используется. null если таблица пустая или не найдено значение
        /// </returns>
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
        /// <param name="equipmentId">Идентификатор оборудования</param>
        public static DbRecordset GetEquipmentParametersById(string equipmentId)
        {
            var queryText = @"SELECT
                                  parameters.Id AS Id,
                                  parameters.Ref AS EquipmentId,
                                  parameters.Val AS Result,
                                  options.Id AS OptionId,
                                  options.Description as Description,
                                  typesDataParameters.Name AS TypeName
                                FROM
                                  _Catalog_Equipment_Parameters AS parameters
                                LEFT JOIN _Catalog_EquipmentOptions AS options
                                  ON parameters.Ref = @equipmentId
                                    AND parameters.Parameter = options.Id

                                LEFT JOIN Enum_TypesDataParameters AS typesDataParameters
                                ON options.DataTypeParameter = typesDataParameters.Id

                                WHERE
                                  parameters.Ref = @equipmentId AND options.DeletionMark = 0
                                ORDER BY parameters.LineNumber ASC";

            var query = new Query(queryText);
            query.AddParameter("equipmentId", equipmentId);

            return query.Execute();
        }

        /// <summary>
        ///     Возвращает историю оборудования начиная с указанноЙ даты
        /// </summary>
        /// <param name="equpmentId">Идентификатор оборудования</param>
        /// <param name="afterDate">Дата начиная с которой выводится история</param>
        public static DbRecordset GetEquipmentHistoryById(string equpmentId, DateTime afterDate)
        {
            var queryText = @"SELECT
                                  Task.Date          AS Date,
                                  Task.TaskType      AS Description,
                                  Status.Description AS Result,
                                  Status.Name        AS ResultName
                                FROM
                                  _Document_Task AS Task
                                  LEFT JOIN
                                  _Document_Task_Status AS Task_Status
                                    ON Task.Id = Task_Status.Ref
                                  LEFT JOIN
                                  Enum_StatusTasks AS Status
                                    ON Task_Status.Status = Status.Id
                                WHERE
                                  Task.Equipment = @equipmentId
                                  AND Task.Date > date(@startDate)
                                  AND Status.Name NOT LIKE 'New'
                                ORDER BY Date
                                  DESC";

            var query = new Query(queryText);
            query.AddParameter("equipmentId", equpmentId);
            query.AddParameter("startDate", afterDate.ToString("yyyy-MM-dd"));

            return query.Execute();
        }

        //TODO: удалить метод GetEquipmentById когда починят getObject у dbEntity

        /// <summary>
        ///     Возвращает описание оборудования
        /// </summary>
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
        ///     Получаем Id и Description пользователя
        ///     по его UserName
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
        ///     Достать максимальное число из колонки таблицы
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="column">Имя колонки</param>
        /// <param name="whereColumnName">Имя колонки, по которой будет поиск</param>
        /// <param name="whereColumnValue">Значения колонки, по которой будет поиск</param>
        /// <returns>Максимальное число</returns>
        public static int GetMaxNumberFromTableInColumn(string table, string column, string whereColumnName,
            string whereColumnValue)
        {
            var query =
                new Query($"select ifnull(max({column}), 0) as max from {table} where {whereColumnName} = @where");
            query.AddParameter("where", whereColumnValue);
            return (int)query.ExecuteScalar();
        }

        /// <summary>
        ///     Получить UserId из БД
        /// </summary>
        /// <returns>Возращается строка содержащая UserId, если в БД не найден UserId возращается пустая строка.</returns>
        public static string GetUserId()
        {
            var result = new Query("SELECT UserId FROM ___UserInfo  LIMIT 1").Execute();
            return result.Next() ? (string)result["UserId"] : "";
        }

        /// <summary>
        ///     Получаем актуальные координаты.
        /// </summary>
        /// <param name="timeSpan">
        ///     Указывает промежуток, который указывает,
        ///     из какого временного диапазона выбирать актуальные координаты.
        ///     По умолчанию, координаты выбираются из всего временного диапазона.
        /// </param>
        /// <returns>Возращяет координаты. Нулевые координаты указывают, что нет актуальных координат.</returns>
        public static DbRecordset GetCoordinate(uint timeSpan = uint.MaxValue)
        {
            return
                new Query(
                    $@"select id, ifnull(Latitude, 0.0) as Latitude, ifnull(Longitude, 0.0) as Longitude, max(datetime(EndTime,'localtime')) as EndTime from ___DbLocations
                                where datetime(EndTime, 'localtime') between datetime('now','localtime', '-{timeSpan} minutes') and datetime('now', 'localtime')")
                    .Execute();
        }

        /// <summary>
        ///     Получить статус задачи.
        /// </summary>
        /// <param name="taskId">Индетификатор задачи.</param>
        /// <returns></returns>
        public static Task_Status GetTaskStatusByTaskId(string taskId)
        {
            var query = new Query(@"SELECT * FROM _Document_Task_Status WHERE Ref = @taskId");
            query.AddParameter("taskId", taskId);

            var result = query.Execute();
            return new Task_Status
            {
                Id = (DbRef)result[nameof(Task_Status.Id)],
                ActualEndDate = (DateTime)result[nameof(Task_Status.ActualEndDate)],
                CommentContractor = (string)result[nameof(Task_Status.CommentContractor)],
                Ref = (DbRef)result[nameof(Task_Status.Ref)],
                LineNumber = (int)result[nameof(Task_Status.LineNumber)],
                Status = (DbRef)result[nameof(Task_Status.Status)],
                UserMA = (DbRef)result[nameof(Task_Status.UserMA)],
                CloseEvent = (DbRef)result[nameof(Task_Status.CloseEvent)]
            };
        }

        public static DbRecordset GetTaskTargetsByTaskId(object taskId)
        {
            var query = new Query(@"SELECT
                                      Id,
                                      Description,
                                      IsDone
                                    FROM _Document_Task_Targets
                                    WHERE
                                      Ref = @taskId");

            query.AddParameter("taskId", taskId);

            return query.Execute();
        }

        public static long GetTotalTaskByEventIdOrClientId(object eventId, object clientId)
        {
            var query = new Query(@"SELECT ifnull(count(*), 0) AS TotalTask
                                    FROM
                                      _Document_Task AS Task
                                      INNER JOIN _Document_Task_Status AS Task_Status
                                      ON Task_Status.Ref = Task.Id
                                      INNER JOIN _Enum_StatusTasks AS Status
                                      ON Task_Status.Status = Status.Id
                                    WHERE
                                      ((Task.Event = @eventId AND Task.Client = @clientId AND Status.Name LIKE 'New'
                                        OR
                                        Task.Event = @eventId AND Task.Client = @clientId AND Status.Name NOT LIKE 'New'
                                        AND Task_Status.CloseEvent = @eventId)
                                       OR
                                       (Task.Client LIKE @clientId
                                        AND Task.Event
                                            = '@ref[Document_Event]:00000000-0000-0000-0000-000000000000'
                                        AND Status.Name LIKE 'New'
                                        OR
                                        Task.Client LIKE @clientId
                                        AND Task.Event
                                            = '@ref[Document_Event]:00000000-0000-0000-0000-000000000000'
                                        AND Status.Name NOT LIKE 'New'
                                        AND Task_Status.CloseEvent = @eventId))
                                      AND Task.DeletionMark == 0");
            query.AddParameter("eventId", eventId);
            query.AddParameter("clientId", clientId);
            var result = query.Execute();

            return (long)result["TotalTask"];
        }

        public static long GetTotalTaskAnsweredByEventIdOrClientId(object eventId, object clientId)
        {
            var query = new Query(@"SELECT ifnull(count(*), 0) AS TaskAnswered
                                    FROM
                                      _Document_Task AS Task
                                      INNER JOIN _Document_Task_Status AS Task_Status
                                      ON Task_Status.Ref = Task.Id
                                      INNER JOIN _Enum_StatusTasks AS Status
                                      ON Task_Status.Status = Status.Id
                                    WHERE
                                      ((Task.Event = @eventId AND Task.Client = @clientId AND Status.Name NOT LIKE 'New'
                                        AND Task_Status.CloseEvent = @eventId)
                                       OR
                                       (Task.Client LIKE @clientId
                                        AND Task.Event
                                            = '@ref[Document_Event]:00000000-0000-0000-0000-000000000000'
                                        AND Status.Name NOT LIKE 'New'
                                        AND Task_Status.CloseEvent = @eventId))
                                      AND Task.DeletionMark == 0");
            query.AddParameter("eventId", eventId);
            query.AddParameter("clientId", clientId);

            var result = query.Execute();

            return (long)result["TaskAnswered"];
        }

        public static DbRecordset GetEquipmentOptionValueList(string optionId)
        {
            var query = new Query(@"SELECT
                                      Catalog_EquipmentOptions_ListValues.Id,
                                      Catalog_EquipmentOptions_ListValues.Val
                                    FROM
                                      Catalog_EquipmentOptions_ListValues
                                    WHERE
                                      Catalog_EquipmentOptions_ListValues.Ref = @optionId
                                    ORDER BY LineNumber ASC");
            query.AddParameter("optionId", optionId);

            return query.Execute();
        }

        public static DbRecordset GetActivitiByTender(object Tender)
        {
            var query = new Query(@"SELECT CA.Description
                                        FROM _Catalog_Tender AS CT
                                          LEFT JOIN _Catalog_Tender_ActivityTypes AS CTA ON CT.Id = CTA.Ref
                                          LEFT JOIN _Catalog_ActivityTypes AS CA ON CA.Id = CTA.ActivityType
                                        WHERE CT.Id = @tender");
            query.AddParameter("tender",Tender);
            return query.Execute();
        }
        public static DbRecordset GetTenderList(object userId)
        {
            var query = new Query(@"SELECT DISTINCT
                                      Tender.Id                    AS Id,
                                      Tender.Number                AS TenderNumber,
                                      Tender.DueDateTime           AS DueDateTime,
                                      Tender.Sum                   AS TotalSum,
                                      Tender.Responsible           AS Responsible,
                                      Tender.Manager               AS Manager,
                                      Client.Description           AS ClientDescription
                                    FROM
                                      _Catalog_Tender AS Tender
                                      LEFT JOIN
                                      _Catalog_Tender_ActivityTypes AS ActivityTypes
                                        ON Tender.Id = ActivityTypes.Ref
                                      LEFT JOIN
                                      _Catalog_ActivityTypes As ActivityTypeC
                                        ON ActivityTypes.ActivityType = ActivityTypeC.Id
                                      LEFT JOIN
                                        _Catalog_ActivityTypes_Users AS ActivityTypeUser
                                        ON ActivityTypeUser.Ref = ActivityTypeC.Id
                                      LEFT JOIN
                                      _Catalog_Client AS Client
                                        ON Tender.Client = Client.Id
                                    WHERE
                                      (ActivityTypeUser.User = @userId
                                        OR
                                        ActivityTypeUser.User IN
                                        (Select CUPodch.ID From Catalog_User CUPodch
                                            LEFT JOIN Catalog_User AS CUNach
                                            ON CUNach.ID = CUPodch.Manager WHERE CUNach.ID = @userId)
                                        OR
                                        Tender.Responsible = @userId
                                        OR
                                        Tender.Responsible IN
                                        (Select CUPodch.ID From Catalog_User CUPodch
                                            LEFT JOIN Catalog_User AS CUNach
                                            ON CUNach.ID = CUPodch.Manager WHERE CUNach.ID = @userId)
                                        OR
                                        Tender.Manager = @userId
                                        OR
                                        Tender.Manager IN
                                        (Select CUPodch.ID From Catalog_User CUPodch
                                            LEFT JOIN Catalog_User AS CUNach
                                            ON CUNach.ID = CUPodch.Manager WHERE CUNach.ID = @userId)
                                        )
                                      AND Tender.DeletionMark = 0
                                      AND Tender.Closed = 0
                                    ORDER BY Tender.DueDateTime ASC");

            query.AddParameter("userId", userId);

            return query.Execute();
        }

        public static DbRecordset GetTenderById(object tenderId)
        {
            var query = new Query(@"SELECT
                                      Tender.Id                 AS Id,
                                      Tender.DueDateTime        AS DueDateTime,
                                      Tender.Description        AS Tender_Description,
                                      Tender.DeliveryDateTime   AS DeliveryDate,
                                      Tender.Marketplace        AS Marketplace,
                                      Tender.Sum                AS Sum,
                                      Tender.Responsible           AS Responsible,
                                      Tender.Manager               AS Manager,
                                      Client.Address            AS Client_Address,
                                      Client.Description        AS Client_Description,
                                      ActivityTypes.Description AS ActivityType
                                    FROM
                                      _Catalog_Tender AS Tender
                                      INNER JOIN
                                        _Catalog_Tender_ActivityTypes AS ActivityTypesTC
                                        ON ActivityTypesTC.Ref = Tender.Id
                                      INNER JOIN
                                      _Catalog_Client AS Client
                                        ON Tender.Client = Client.Id
                                      INNER JOIN
                                      _Catalog_ActivityTypes AS ActivityTypes
                                        ON ActivityTypesTC.ActivityType = ActivityTypes.Id
                                    WHERE
                                      Tender.Id = @tenderId
                                      AND Tender.DeletionMark = 0");

            query.AddParameter("tenderId", tenderId);

            return query.Execute();
        }

        public static DbRecordset GetMessages(object tenderId)
        {
            var query = new Query(@"SELECT
                                      Chat.Id          AS Id,
                                      Chat.DateTime    AS Date,
                                      Chat.Message     AS Message,
                                      User.Description AS UserDescription
                                    FROM
                                      _Catalog_Chat AS Chat
                                      LEFT JOIN
                                      _Catalog_User AS User
                                        ON Chat.User = User.Id

                                    WHERE
                                      Chat.Tender = @tenderId
                                      AND Chat.DeletionMark = 0
                                    ORDER BY DateTime");

            query.AddParameter("tenderId", tenderId);
            return query.Execute();
        }

        public static DbRecordset GetFilters(String filter = "")
        {
            var query = new Query(@"SELECT
                          Id,
                          Description
                        FROM Catalog_MobileTaskFilters");

            return query.Execute();
        }

        public static DbRecordset GetUsers(String filter = "")
        {
            var query = new Query(@"SELECT
                          Id,
                          Description
                        FROM _Catalog_User");
            if (!String.IsNullOrEmpty(filter))
            {
                query.Text += $@" Where Contains(Description,'{filter}')";
            }
            return query.Execute();
        }

        public static DbRecordset GetTaskTypes()
            => new Query(@"SELECT
                              Id,
                              Description
                            FROM Enum_TypesEvents").Execute();

        public static DbRecordset GetStatusImportance()
            => new Query(@"SELECT
                          Id,
                          Description
                        FROM Enum_StatusImportance").Execute();

        public static DbRecordset GetEventResults()
            => new Query(@"SELECT
                          Id,
                          Description
                        FROM _Catalog_EventResults
                        WHERE DeletionMark = 0").Execute();

        public static DbRecordset GetClientProfile()
            => new Query(@"SELECT
                              Id,
                              Description
                            FROM _Catalog_Profile").Execute();
    }
}