using BitMobile.ClientModel3;
using BitMobile.Common.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using static System.String;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Test
{
    public static class Utils
    {
        public static object GetValueOrDefault(this IDictionary<string, object> dictionary, string key,
            object @default = null)
        {
            object res;
            return dictionary.TryGetValue(key, out res) ? res : @default;
        }

        /// <summary>
        ///     Distance in meters
        /// </summary>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            //Haversine formula:
            //a = sin²(Δf/2) + cos f1 ⋅ cos f2 ⋅ sin²(Δl/2)
            //c = 2 ⋅ atan2( √a, √(1−a) )
            //d = R ⋅ c
            //where	f is latitude, l is longitude, R is earth’s radius (mean radius = 6,371km);
            //note that angles need to be in radians to pass to trig functions!

            lat1 = Convert.ToDouble(lat1);
            lon1 = Convert.ToDouble(lon1);
            lat2 = Convert.ToDouble(lat2);
            lon2 = Convert.ToDouble(lon2);
            const double r = 6371;
            var f1 = lat1 * Math.PI / 180;
            var f2 = lat2 * Math.PI / 180;
            var deltaf = f2 - f1;
            var deltal = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Pow(Math.Sin(deltaf / 2), 2)
                    + Math.Cos(f1) * Math.Cos(f2) * Math.Pow(Math.Sin(deltal / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var result = r * c;

            return Math.Abs(result * 1000);
        }

        /// <summary>
        ///     Преобразует входящую строку к виду, который помесится в указанное количество строк интерфейса
        /// </summary>
        /// <param name="str">Строка для красивого обрезания</param>
        /// <param name="outputLineLength">Длина одной строки в интерфейсе</param>
        /// <param name="outputLinesAmount">Количество строк в интерфейсе</param>
        /// <returns>Преобразованная строка</returns>
        public static string CutForUIOutput(this string str, int outputLineLength, int outputLinesAmount)
        {
            if (IsNullOrWhiteSpace(str) || str.Length < outputLineLength)
                return str;
            var split = str.Split(null);
            outputLineLength = Convert.ToInt32(outputLineLength);
            outputLinesAmount = Convert.ToInt32(outputLinesAmount);
            var words = new ArrayList();
            foreach (var word in split)
            {
                if (!IsNullOrWhiteSpace(word))
                    words.Add(word);
            }
            bool fitAll;
            var lines = CreateLinesFromWords(outputLineLength, outputLinesAmount, words, out fitAll);
            bool? test = fitAll;
            string res = null;
            foreach (var line in lines)
            {
                res = res == null ? (string)line : $"{res}{Environment.NewLine}{line}";
            }
            return (bool)test ? res : $"{res?.TrimEnd()}...";
        }

        private static ArrayList CreateLinesFromWords(int outputLineLength, int outputLinesAmount, IList words,
            out bool fitAll)
        {
            var lines = new ArrayList { "" };
            var lastLineNumber = 0;
            var currentWordNumber = 0;
            while (currentWordNumber < words.Count && lastLineNumber < outputLinesAmount)
            {
                var word = (string)words[currentWordNumber];
                if (((string)lines[lastLineNumber]).Length + 1 + word.Length <= outputLineLength)
                {
                    lines[lastLineNumber] = IsNullOrEmpty((string)lines[lastLineNumber])
                        ? word
                        : $"{lines[lastLineNumber]} {word}";
                    currentWordNumber++;
                    continue;
                }
                if (word.Length >= outputLineLength)
                {
                    if (IsNullOrEmpty((string)lines[lastLineNumber]))
                    {
                        lines[lastLineNumber] = ((string)words[currentWordNumber]).Substring(0, outputLineLength);
                        words[currentWordNumber] = ((string)words[currentWordNumber]).Substring(outputLineLength);
                    }
                    lines.Add("");
                    lastLineNumber++;
                }
                else
                {
                    lines.Add(word);
                    currentWordNumber++;
                    lastLineNumber++;
                }
            }
            fitAll = currentWordNumber >= words.Count && ((string)words[words.Count - 1]).Length <= outputLineLength;
            if (lines.Count > outputLinesAmount)
                lines[lines.Count - 1] = Empty;

            return lines;
        }

        /* TODO: Не удалять до тех пор пока платформа не даст разъяснений по поводу корректного удаления элементов.*/

        /// <summary>
        /// Пока данным методом не пользоваться, до выяснения обстоятельств
        /// </summary>
        /// <param name="container"></param>
        /// <param name="index"></param>
        public static void RemoveChild(this IContainer container, int index)
        {
            if (index < 0)
                throw new ArgumentException("Индекс удаляемого элемента не может быть отрицательным.");
            if (index > container.Controls.Length)
                throw new ArgumentException(
                    $"Индекс превышает количество элементов фактически содержащихся в {container.GetType().Name}");

            ((ILayoutableContainer)((IWrappedControl3)container).GetNativeControl()).Withdraw(index);
            DConsole.WriteLine($"Deleted");
        }

        /// <summary>
        /// Пока данным методом не пользоваться, до выяснения обстоятельств
        /// </summary>
        /// <param name="container"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public static bool RemoveChild(this IContainer container, string childId)
        {
            int index;

            if (!TryGetIndexByChildId(container, childId, out index)) return false;
            DConsole.WriteLine($"Index = {index}");
            RemoveChild(container, index + 1);
            return true;
        }

        /// <summary>
        /// Пока данным методом не пользоваться, до выяснения обстоятельств
        /// </summary>
        /// <param name="container"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public static int GetChildIndexByChildId(this IContainer container, string childId)
        {
            for (var i = 0; i < container.Controls.Length; i++)
            {
                var id = (IIdentifiable)container.Controls[i];

                if (CompareOrdinal(childId, id.Id) == 0)
                {
                    return i;
                }
            }
            throw new ArgumentException($"Control with Id {childId} not found");
        }

        /// <summary>
        /// Пока данным методом не пользоваться, до выяснения обстоятельств
        /// </summary>
        /// <param name="container"></param>
        /// <param name="childId"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TryGetIndexByChildId(this IContainer container, string childId, out int index)
        {
            DConsole.WriteLine($"Container Lenght = {container.Controls.Length}");
            foreach (var control in container.Controls)
            {
                var item = (IIdentifiable)control;
                DConsole.WriteLine($"ID = {item.Id}");
            }
            DConsole.WriteLine("--------------");
            for (var i = 0; i < container.Controls.Length; i++)
            {
                var id = (IIdentifiable)container.Controls[i];
                DConsole.WriteLine($"{id.Id}");
                if (CompareOrdinal(childId, id.Id) == 0)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;

            return false;
        }

        /// <summary>
        /// Возвращает номер недели указанной даты
        /// </summary>
        public static int GetWeekNumber(this DateTime date)
        {
            var thursday = date + new TimeSpan(3 - ((int)date.DayOfWeek + 6) % 7, 0, 0, 0);
            return 1 + (thursday.DayOfYear - 1) / 7;
        }

        /// <summary>
        /// Метод, позволяет получить сведения о вызывающем объекте и подставляющий
        /// их в необязательные параметры
        /// </summary>
        /// <param name="message">Сообщение которое необходимо вывести.</param>
        /// <param name="memberName">авто параметер</param>
        /// <param name="filePath">авто параметер</param>
        /// <param name="sourceLineNumber">авто параметер</param>
        [Conditional("DEBUG")]
        public static void TraceMessage(string message = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            DConsole.WriteLine($"{Environment.NewLine}Message: {message} ");
            DConsole.WriteLine($"Member Name: {memberName} ");
            DConsole.WriteLine($"Source file path: {filePath} ");
            DConsole.WriteLine($"Source line number: {sourceLineNumber} {Environment.NewLine}");
        }

        [Conditional("DEBUG")]
        public static void SendDatabase()
            => Application.SendDatabase(Settings.SolutionUrl, Settings.User, Settings.Password);
    }
}