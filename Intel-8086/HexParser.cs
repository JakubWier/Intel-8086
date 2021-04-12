using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public class HexParser : NumeralConverter
    {
        public string GetName => "Hexadecimal";

        public string IntToString(int number)
        {
            string result = number.ToString("X").ToUpper();
            result = result.PadLeft(4, '0');
            result = result.Insert(0, "0x");
            return result;
        }
    }
}
