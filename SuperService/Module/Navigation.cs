using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using BitMobile.Common.Controls;
using Stack = BitMobile.ClientModel3.Stack;

namespace Test
{
    public static class Navigation
    {
        private const string DefaultStyle = @"Style\style.css";
        private static readonly Stack ScreenInfoStack = new Stack();
        private static readonly Stack ScreenStack = new Stack();

        private static bool _nonModalMove;

        private static readonly ArrayList CurrentScreenInfoRef = new ArrayList();

        private static readonly ArrayList CurrentScreenRef = new ArrayList();

        /// <summary>
        ///     Информация о текущем отображаемом экране
        /// </summary>
        public static ScreenInfo CurrentScreenInfo
        {
            get { return CurrentScreenInfoRef.Count == 0 ? null : (ScreenInfo)CurrentScreenInfoRef[0]; }
            private set
            {
                if (CurrentScreenInfoRef.Count == 0)
                {
                    CurrentScreenInfoRef.Add(value);
                }
                else
                {
                    CurrentScreenInfoRef[0] = value;
                }
            }
        }

        /// <summary>
        ///     Ссылка на отображемый экран
        /// </summary>
        public static Screen CurrentScreen
        {
            get { return CurrentScreenRef.Count == 0 ? null : (Screen)CurrentScreenRef[0]; }
            private set
            {
                if (CurrentScreenRef.Count == 0)
                {
                    CurrentScreenRef.Add(value);
                }
                else
                {
                    CurrentScreenRef[0] = value;
                }
            }
        }

        /// <summary>
        ///     Перейти на один экран по стеку назад
        /// </summary>
        /// <param name="reload">Перезагржать ли целевой экран (ВНИМАНИЕ, СТАВИТЬ СЮДА false ОПАСНО)</param>
        public static void Back(bool reload = true)
        {
            if (ScreenInfoStack.Count == 0)
            {
                DConsole.WriteLine("Can't go back when stack is empty");
                return;
            }
            var screenInfo = (ScreenInfo)ScreenInfoStack.Pop();
            var nextScreen = (Screen)ScreenStack.Pop();
            if (!reload)
                ModalMove(screenInfo, screen: nextScreen);
            else
                ModalMove(screenInfo, nextScreen.Variables);
        }

        /// <summary>
        ///     Передвинуться на экран, записав текущий экран на стек
        /// </summary>
        /// <param name="name">Имя целевого экрана</param>
        /// <param name="args">Словарь аргументов</param>
        /// <param name="css">Путь к файлу стиля</param>
        public static void Move(string name, IDictionary<string, object> args = null, string css = null)
        {
            _nonModalMove = true;
            var screenInfo = CreateScreenInfoFromName(name, css);
            Move(screenInfo, args);
        }

        private static ScreenInfo CreateScreenInfoFromName(string name, string css)
        {
            return new ScreenInfo
            {
                Name = name,
                Xml = $@"Screen\{name}.xml",
                Css = css ?? $@"Style\{name}.css"
            };
        }

        /// <summary>
        ///     Передвинуться на экран, записав текущий экран на стек
        /// </summary>
        /// <param name="screenInfo">Информация о следующем экране</param>
        /// <param name="args">Словарь аргументов</param>
        public static void Move(ScreenInfo screenInfo, IDictionary<string, object> args = null)
        {
            _nonModalMove = true;
            if (CurrentScreenInfo != null)
            {
                ScreenInfoStack.Push(CurrentScreenInfo);
                ScreenStack.Push(CurrentScreen);
            }
            ModalMove(screenInfo, args);
        }

        /// <summary>
        ///     Изменить отображаемый экран, не меняя стек.
        /// </summary>
        /// <param name="name">Имя экрана</param>
        /// <param name="args">Словарь аргументов</param>
        /// <param name="css">Путь к файлу стилей</param>
        public static void ModalMove(string name, IDictionary<string, object> args = null, string css = null, ShowAnimationType AnimationType = ShowAnimationType.GoNext)
        {
            var screenInfo = CreateScreenInfoFromName(name, css);
            ModalMove(screenInfo, args,null ,AnimationType);
        }

        /// <summary>
        ///     Изменить отображаемый экран, не меняя стек.
        /// </summary>
        /// <param name="screenInfo">Информация об экране</param>
        /// <param name="args">Словарь аргументов</param>
        /// <param name="screen">Ссылка на целевой экран</param>
        public static void ModalMove(ScreenInfo screenInfo, IDictionary<string, object> args = null,
            Screen screen = null,ShowAnimationType AnimationType = ShowAnimationType.GoNext)
        {
            try
            {
                DConsole.WriteLine($"Moving to {screenInfo.Name}");
                try
                {
                    screen = screen ?? (Screen)Application.CreateInstance($"Test.{screenInfo.Name}");
                }
                catch
                {
                    DConsole.WriteLine($"Can't load screen with name {screenInfo.Name}");
                    if (!_nonModalMove || ScreenInfoStack.Count < 1) return;
                    ScreenInfoStack.Pop();
                    ScreenStack.Pop();
                    return;
                }
                screen.SetData(args);
                try
                {
                    screen.LoadFromStream(Application.GetResourceStream(screenInfo.Xml));
                }
                catch (Exception e)
                {
                    DConsole.WriteLine($"Error while loading {screenInfo.Name}'s xml ({screenInfo.Xml})");
                    DConsole.WriteLine($"{e.Message}");
                }
                try
                {
                    screen.LoadStyleSheet(Application.GetResourceStream(screenInfo.Css));
                }
                catch
                {
                    screen.LoadStyleSheet(Application.GetResourceStream(DefaultStyle));
                }
                CurrentScreenInfo = screenInfo;
                CurrentScreen = screen;
                screen.Show(AnimationType);
                _nonModalMove = false;
            }
            catch (Exception e)
            {
                DConsole.WriteLine($"{e.GetType().FullName}:{e.Message}");
                DConsole.WriteLine($"{e.StackTrace}");
            }
        }

        /// <summary>
        ///     Очистить стек
        /// </summary>
        public static void CleanStack()
        {
            ScreenInfoStack.Clear();
            ScreenStack.Clear();
        }
    }

    public class ScreenInfo
    {
        public string Name { get; set; }
        public string Xml { get; set; }
        public string Css { get; set; }
    }
}