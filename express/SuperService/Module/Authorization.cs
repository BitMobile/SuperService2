using BitMobile.ClientModel3;
using System;
using System.Text;
using Test.Catalog;

namespace Test
{
    public static class Authorization
    {
        private static WebRequest _webRequest;
        public static bool Initialized { get; private set; }
        private static AuthScreen _screen;
        private static string _user;
        private static string _password;

        public static void Init()
        {
            _webRequest = new WebRequest
            {
                Host = Settings.Host,
                Timeout = new TimeSpan(0, 0, 5).ToString()
            };

            Initialized = true;
        }

        /// <summary>
        ///     Быстрая авторизация.
        /// </summary>
        /// <returns>
        ///     Возращает true если логин и пароль не пустые
        ///     , иначе false
        /// </returns>
        public static bool FastAuthorization()
        {
            return !string.IsNullOrEmpty(Settings.User) && !string.IsNullOrEmpty(Settings.Password);
        }

        public static void StartAuthorization(string userName, string password)
        {
            Init();
            _user = userName;
            _password = password;
            var userpass = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));
            _webRequest.AddHeader($"Authorization",$"Basic {userpass}");
            Utils.TraceMessage($"Header auth in BASE64:{userpass}");
            Application.InvokeOnMainThread(() => AuthScreen.EditableVisualElements(false));
            Utils.TraceMessage($"Url auth: {Settings.AuthUrl}");
            _webRequest.Get(Settings.AuthUrl, Callback);
        }

        private static void Callback(object sender, ResultEventArgs<WebRequest.WebRequestResult> args)
        {
            if (args.Result.Success)
            {
#if DEBUG
                DConsole.WriteLine("Авторизация успешна");
                DConsole.WriteLine($"UserId - {Settings.UserId} Web Request Result - {args.Result.Result}");
#endif

                //Проверяем, если пользователь уже сохранен, то делаем частичную синхронизацию, иначе полную.
                // ReSharper disable once StringCompareIsCultureSpecific.3
                if (string.Compare(Settings.User, _webRequest.UserName, true) == 0)
                {
#if DEBUG
                    DConsole.WriteLine($"Авторизировались, пользователь сохранен в системе.");
                    DConsole.WriteLine("Сохраняем пароль");
#endif

                    Settings.Password = _password;

#if DEBUG
                    DConsole.WriteLine($"Запустили частичную синхронизацию. From class {nameof(Authorization)}");
#endif
                    DBHelper.SyncAsync();
                    DConsole.WriteLine("Loading first screen...");
                    Navigation.ModalMove("EventListScreen");
                }
                else
                {
#if DEBUG
                    DConsole.WriteLine($"Авторизировались, пользователь НЕ сохранен в системе.");
                    DConsole.WriteLine("Сохраняем пользователя и пароль в системе");
#endif
                    Settings.User = _user;
                    Settings.Password = _password;
#if DEBUG
                    DConsole.WriteLine($"Запустили полную синхронизацию. From class {nameof(Authorization)}");
#endif
                    Application.InvokeOnMainThread(() => AuthScreen.EditableVisualElements(false));
                    DBHelper.FullSyncAsync(ResultEventHandler);
                }
            }
            else
            {
#if DEBUG
                DConsole.WriteLine($"Авторизация не удалась. Сбрасываем пароль.");
#endif
                Settings.Password = "";
                Application.InvokeOnMainThread(AuthScreen.ClearPassword);

                ErrorMessageWithToast(args);
                Application.InvokeOnMainThread(() => AuthScreen.EditableVisualElements(true));
            }
        }

        private static void ResultEventHandler(object sender, ResultEventArgs<bool> resultEventArgs)
        {
            if (!DBHelper.SuccessSync)
            {
                Settings.User = "";
                Settings.Password = "";
                Application.InvokeOnMainThread(AuthScreen.ClearPassword);
                Application.InvokeOnMainThread(() => AuthScreen.EditableVisualElements(true));
                return;
            }
#if DEBUG
            DConsole.WriteLine(Parameters.Splitter);
            DConsole.WriteLine("Синхронизация удачна");
            DConsole.WriteLine($"{nameof(DBHelper.SuccessSync)} - {DBHelper.SuccessSync}");
            DConsole.WriteLine($"In Class {nameof(Authorization)} Method {nameof(StartAuthorization)}");
            DConsole.WriteLine(Parameters.Splitter);
            DConsole.WriteLine("Loading first screen...");
#endif
            FileSystem.ClearPrivate();
            FileSystem.ClearShared();
            FileSystem.SyncShared(Settings.ImageServer, Settings.User, Settings.Password);
            Application.InvokeOnMainThread(() => Navigation.ModalMove(nameof(EventListScreen)));
        }

        private static void ErrorMessageWithToast(ResultEventArgs<WebRequest.WebRequestResult> args)
        {
            switch (args.Result.Error.StatusCode)
            {
                case -1:
                    Toast.MakeToast(Translator.Translate("сonnection_error"));
                    break;

                case 401:
                    Toast.MakeToast(Translator.Translate("uncorrect_login_or_pass"));
                    break;

                default:
                    Toast.MakeToast(Translator.Translate("unexpected_error"));
                    break;
            }
        }

        private static void ErrorInfo(ResultEventArgs<WebRequest.WebRequestResult> args)
        {
            switch (args.Result.Error.StatusCode)
            {
                case -1:
                    DConsole.WriteLine($"{Translator.Translate("сonnection_error")} Error - {args.Result.Error.Message}");
                    break;

                case 401:
                    DConsole.WriteLine(
                        $"{Translator.Translate("uncorrect_login_or_pass")} Error - {args.Result.Error.Message}");
                    break;

                default:
                    DConsole.WriteLine($"{Translator.Translate("unexpected_error")} - Error {args.Result.Error.Message}");
                    break;
            }
        }
    }
}