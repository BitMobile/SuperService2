using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Catalog;
using Test.Components;

namespace Test
{
    public class MapScreen : Screen
    {
        private WebMapGoogle _map;
        private ArrayList _location;
        private DbRecordset _data;
        private bool _isClientScreen = false;
        private bool _isEventListScreen = false;
        private bool _isEventScreen = false;
        private bool _isInit = false;
        private bool _isNotEmptyLocationData = false;
        private TopInfoComponent _topInfoComponent;
        private bool _isDefault = false;
        private string _clientId;
        private decimal _clientLatitude;
        private decimal _clientLongitude;

        public override void OnLoading()
        {
            _map = (WebMapGoogle)GetControl("Map", true);
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("map"),
                LeftButtonControl = new Image() { Source = ResourceManager.GetImage("topheading_back") },
                ArrowVisible = false
            };
            _topInfoComponent.ActivateBackButton();
            DConsole.WriteLine("MapScreen");
        }

        public override void OnShow()
        {
            FillMap();
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            DConsole.WriteLine("Back to screen .....");
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal bool GetIsEventListScreen()
        {
            return _isEventListScreen;
        }

        internal bool GetIsClientOrEventScreen()
        {
            return _isClientScreen || _isEventScreen;
        }

        private void IsNotEmptyCoordinate()
        {
            DConsole.WriteLine(nameof(IsNotEmptyCoordinate));
            if (_data != null)
            {
                DConsole.WriteLine($"{_data.IsClosed}");
                if (!_data.IsClosed)
                {
                    while (_data.Next())
                    {
                        var latitude = (double)_data["Latitude"];
                        var longitude = (double)_data["Longitude"];

                        Dictionary<string, object> dictionary =
                            new Dictionary<string, object>()
                            {
                                {"Description",_data["Description"] },
                                {"Latitude", latitude},
                                {"Longitude",longitude }
                            };
                        _location.Add(dictionary);
                    }

                    _isNotEmptyLocationData = _location.Count > 0;
                }
                else
                {
                    DConsole.WriteLine($"Error in {nameof(IsNotEmptyCoordinate)}");
                }
            }
            else
            {
                DConsole.WriteLine($"{_data} is null");
            }
            DConsole.WriteLine(nameof(_isNotEmptyLocationData) + " = " +
                               _isNotEmptyLocationData);
        }

        internal bool IsZeroArrayLenght()
        {
            return _location.Count == 0;
        }

        internal bool Init()
        {
            if (_isInit) return _isInit;
            DConsole.WriteLine("Start " + nameof(Init));
            _location = new ArrayList();

            var screenState = BusinessProcess.GlobalVariables.GetValueOrDefault(Parameters.IdScreenStateId, MapScreenStates.Default);
            var locationData = BusinessProcess.GlobalVariables.GetValueOrDefault(Parameters.IdClientId);

            var state = (MapScreenStates)screenState;
            DConsole.WriteLine(state.ToString());

            switch (state)
            {
                case MapScreenStates.EventListScreen:
                    _isEventListScreen = true;
                    DConsole.WriteLine("MapScreenStates = EventListScreen");
                    _data = DBHelper.GetEventsLocationToday();
                    break;

                case MapScreenStates.ClientScreen:
                    _isClientScreen = true;
                    if (locationData != null)
                    {
                        _data = DBHelper.GetClientLocationByClientId((string)locationData);
                        _clientId = (string)locationData;
                    }
                    break;

                case MapScreenStates.EventScreen:
                    _isEventScreen = true;
                    if (locationData != null)
                    {
                        _data = DBHelper.GetClientLocationByClientId((string)locationData);
                        _clientId = (string)locationData;
                    }
                    break;

                default:
                    DConsole.WriteLine("is Default");
                    _data = null;
                    _location = null;
                    _isDefault = true;
                    break;
            }

            _isInit = true;
            IsNotEmptyCoordinate();
            DConsole.WriteLine("End " + nameof(Init));
            return true;
        }

        private void FillMap()
        {
            DConsole.WriteLine("start FillMap");
            DConsole.WriteLine($"{nameof(_location)}.{nameof(_location.Count)} = {_location.Count}");
            foreach (var element in _location)
            {
                var item = (Dictionary<string, object>)element;
                var description = (string)item["Description"];
                var latitude = (double)(decimal)item["Latitude"];
                var longitude = (double)(decimal)item["Longitude"];

                DConsole.WriteLine($"{nameof(description)}:{description} " +
                                   $"{Environment.NewLine} " +
                                   $"{nameof(latitude)}={latitude} " +
                                   $"{nameof(longitude)}={longitude}");

                _map.AddMarker(description, latitude, longitude, "red");
            }
            DConsole.WriteLine("end FillMap");
        }

        internal bool GetIsDefault()
        {
            return _isDefault;
        }

        internal bool GetIsNotEmptyLocationData()
        {
            return _isNotEmptyLocationData;
        }

        internal void GetLocation_OnClick(object sender, EventArgs e)
        {
            var coordinate = DBHelper.GetCoordinate(TimeRangeCoordinate.DefaultTimeRange);
            var latitude = Converter.ToDouble(coordinate["Latitude"]);
            var longitude = Converter.ToDouble(coordinate["Longitude"]);

            if (!latitude.Equals(0.0) && !longitude.Equals(0.0))
            {
                _clientLatitude = Convert.ToDecimal(latitude);
                _clientLongitude = Convert.ToDecimal(longitude);

#if DEBUG
                DConsole.WriteLine($"{nameof(_clientLatitude)}:{_clientLatitude} " +
                                   $"{Environment.NewLine}" +
                                   $"{nameof(_clientLongitude)}:{_clientLongitude}");
#endif

                var btn = (Button)sender;
                btn.Text = Translator.Translate("save_coordinates");
                btn.OnClick -= GetLocation_OnClick;
                btn.OnClick += SaveClientLocation_OnClick;
            }
            else
            {
                ((Button)sender).Text = Translator.Translate("failed_coordinates");
            }
        }

        internal void SaveClientLocation_OnClick(object sender, EventArgs e)
        {
            var client = (Client)DBHelper.LoadEntity(_clientId.ToString());
            client.Latitude = _clientLatitude;
            client.Longitude = _clientLongitude;
            DBHelper.SaveEntity(client);
            var btn = (Button)sender;
            btn.Text = Translator.Translate("get_coordinates");
            btn.OnClick -= SaveClientLocation_OnClick;
            btn.OnClick += GetLocation_OnClick;
        }
    }

    public enum MapScreenStates
    {
        EventListScreen,
        ClientScreen,
        EventScreen,
        Default
    }
}