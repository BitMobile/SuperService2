namespace Test
{
    internal static class Parameters
    {
        //Идентификаторы передаваемых между экранами параметров
        public const string IdCurrentEventId = "currentEventId";

        public const string IdScreenStateId = "screenState";
        public const string IdClientId = "clientId";
        public const string IdIsService = "isService";
        public const string IdBehaviour = "behaviour";
        public const string IdLineId = "lineId";
        public const string IdEquipmentId = "idEquipmentId";
        public const string IdIsMaterialsRequest = "isMaterialsRequest";
        public const string IdIsReadonly = "isReadonly";
        public const string IdImage = "image";
        public const string IdWasEventStarted = "wasEventStarted";
        public const string Contact = "contact";

        /// <summary>
        /// Используется для передачи параметров на экран EditServiceOrMaterials,
        /// для того чтобы сохранить состояние экрана RIMScreen.
        /// </summary>
        public const string PreviousScreen = "previousScreen";

        public const string IsEdit = "isEdit";

        //Представление пустой суммы для отобажения на экранах с отключенной настройкой отображения цен
        public const string EmptyPriceDescription = " -- ";

        //Параметры для получения настроек.
        public const string AllowGallery = "AllowGalery";

        public const string PictureSize = "PictureSize";
        public const string EquipmentEnabled = "UsedEquipment";
        public const string CheckListEnabled = "UsedCheckLists";
        public const string COCEnabled = "UsedCalculate";
        public const string BagEnabled = "UsedServiceBag";
        public const string ShowServicePrice = "UsedCalculateService";
        public const string ShowMaterialPrice = "UsedCalculateMaterials";
        public const string LogicValue = "LogicValue";
        public const string NumericValue = "NumericValue";
        public const string EnableFPTR = "EnableFPTR";
        /// <summary>
        /// Индетефикатор задачи
        /// </summary>
        public const string IdTaskId = "TaskId";

        /// <summary>
        ///     Представляет разделитель строк для режима DEBUG
        /// </summary>
        public const string DateEnd = "DateEnd";


        /// <summary>
        ///     Представляет разделитель строк для режима DEBUG
        /// </summary>
        public const string Splitter = "-----------------------------------------------";

        public const string UserSolutionName = "UserSolutionName";

        #region Clients enviroment types

        public const string ClientEnviromentStandAlone = "StandAlone";
        public const string ClientEnviromentBox = "Box";

        #endregion

        #region StyleNames

        /// <summary>
        ///     Стиль для обычного времени
        /// </summary>
        public const string EventListScreenStyleTextVeiw = "EventListTimeTV";
        /// <summary>
        ///     Стиль для длинного времени
        /// </summary>
        public const string EventListScreenStyleLongTextVeiw = "EventListTimeLongTV";
        /// <summary>
        ///     Стиль для иконки соединенного фискального аппарата
        /// </summary>
        public const string FiscalRegistratorScreenConnectStyle = "IconConnectStyle";
        /// <summary>
        ///     Стиль для иконки разъединенного фискального аппарата
        /// </summary>
        public const string FiscalRegistratorScreenDisconnectStyle = "IconDisconnectStyle";
        /// <summary>
        ///     Стиль для иконки разъединенного фискального аппарата
        /// </summary>
        public const string FiscalRegistratorScreenDisconnectedButtonStyle = "DisconnectedButtonVL";
        /// <summary>
        ///     Стиль для иконки разъединенного фискального аппарата
        /// </summary>
        public const string FiscalRegistratorScreenConnectedButtonStyle = "ConnectedButtonVL";

        #endregion
    }
}