using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    class Unit16BitConverter
    {
        public static string FromDecToHex(string DecimalNumber)
        {
            return Convert.ToInt16(DecimalNumber).ToString("X");
        }
    }
}
