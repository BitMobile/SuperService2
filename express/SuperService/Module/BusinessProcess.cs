using System;
using System.Collections.Generic;

namespace Test
{
    [Obsolete("Паарметры передаются череза Variable класса Screen")]
    public static class BusinessProcess
    {
        public static Dictionary<string, object> GlobalVariables { get; } = new Dictionary<string, object>();
    }
}