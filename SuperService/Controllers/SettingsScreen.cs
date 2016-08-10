using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using Test.Components;

namespace Test
{
    public class SettingsScreen : Screen
    {
        private TabBarComponent _tabBarComponent;
        private string _userDescription;
        private string _version;

        public override void OnLoading()
        {
            DConsole.WriteLine("SettingsScreen init");
            _tabBarComponent = new TabBarComponent(this);
        }

        internal void TabBarFirstTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Events_OnClick(sender, eventArgs);
            DConsole.WriteLine("Settings Events");
        }

        internal void TabBarSecondTabButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Bag_OnClick(sender, eventArgs);
            DConsole.WriteLine("Settings Bag");
        }

        internal void TabBarThirdButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabBarComponent.Clients_OnClick(sender, eventArgs);
            DConsole.WriteLine("Settings Clients");
        }

        internal void TabBarFourthButton_OnClick(object sender, EventArgs eventArgs)
        {
            //_tabBarComponent.Settings_OnClick(sender, eventArgs);
            DConsole.WriteLine("Settings Settings");
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal string GetVersion()
        {
            try
            {
                using (var stream = Application.GetResourceStream("settings.xml"))
                {
                    var xDocument = new XmlDocument();
                    xDocument.Load(stream);
                    if (xDocument.DocumentElement != null)
                        _version = xDocument.DocumentElement.ChildNodes[0].ChildNodes[0].Attributes?["version"].Value;
                }
            }
            catch (Exception)
            {
                DConsole.WriteLine($"File settings.xml Not Found");
            }

            return _version != null ? $"v. {_version}" : "v. 0.0.0.0";
        }

        /// <summary>
        /// Возращает подстрок из строки
        /// </summary>
        /// <param name="str"> Строка из которой будт извлекаться подстроки </param>
        /// <param name="maxCount"> Максимальное кол-во извлекаемых подстрок </param>
        /// <returns> Извлеченные подстроки </returns>
        private ArrayList ReturnCountOfWords(string str, int maxCount)
        {
            var resultArrayList = new ArrayList();

            var i = 0;
            foreach (var item in str.Split(null))
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                if (i < maxCount)
                {
                    resultArrayList.Add(item);
                    ++i;
                }
                else
                {
                    return resultArrayList;
                }
            }

            return resultArrayList;
        }

        internal string GetUserInitials()
        {
            var result = "";
            var strings = ReturnCountOfWords(_userDescription, 2);

            foreach (var str in strings)
            {
                result += $"{((string)str).Substring(0, 1).ToUpper()}";
            }

            return result;
        }

        internal string GetUserDescription()
        {
            var result = "";

            var strings = ReturnCountOfWords(_userDescription, 2);

            foreach (var str in strings)
            {
                result += $"{str} ";
            }

            return result.Trim();
        }

        internal bool Init()
        {
            //TODO: Опасно брать юзера отсюда.
            var result = DBHelper.GetUserInfoByUserName(Settings.User);
            _userDescription = result.Next() ? (string)result["Description"] : "";

#if DEBUG
            DConsole.WriteLine(_userDescription);
#endif

            return true;
        }

        internal void Logout_OnClick(object sender, EventArgs e)
        {
            Dialog.Ask(Translator.Translate("exit"), (o, args) =>
            {
                if (args.Result != Dialog.Result.Yes) return;
                Logout();
            });
        }

        private static void Logout()
        {
            DBHelper.Sync();
            Settings.Password = "";
            Navigation.CleanStack();
            Navigation.ModalMove("AuthScreen");
        }

        internal void Twitter_OnClick(object sender, EventArgs e)
        {
        }

        internal void Facebook_OnClick(object sender, EventArgs e)
        {
        }

        internal void SendErrorReport_OnClick(object sender, EventArgs e)
        {
            var uploader = new PrivateUploader();
            uploader.Start();
        }

        internal void SendLog_OnClick(object sender, EventArgs e)
        {
        }
    }

    internal class PrivateUploader : Thread
    {
        public override void Execute()
        {
            FileSystem.UploadPrivate(Settings.Server, Settings.User, Settings.Password);
            FileSystem.SyncShared(Settings.Server, Settings.User, Settings.Password);
            Toast.MakeToast(Translator.Translate("upload_finished"));
        }
    }
}