using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086.Console
{
    static class CommandFormatter
    {
        public static int FindAndRemoveArgSeparator(string[] args)
        {
            int pos = 0;
            int index = -1;

            for (; pos < args.Length; pos++)
            {
                index = args[pos].IndexOf(',');
                if (index != -1)
                {
                    args[pos] = args[pos].Remove(index, 1);
                    break;
                }
            }
            if (pos == args.Length)
                return -1;

            return pos;
        }

        public static void CheckAndReduceOverflow(ref int value, out string log, char registryPostfix = '0')
        {
            log = "";
            if (registryPostfix == 'L' || registryPostfix == 'H')
            {
                if (value > byte.MaxValue)
                {
                    value = BitConverter.GetBytes(value)[0]; //255;
                    log = "Expected 8bit value.\nData loss due to conversion.\nMoving first byte of value.\n";
                }
            }
            else
            {
                if (value > ushort.MaxValue)
                {
                    byte[] convert = BitConverter.GetBytes(value);
                    value = convert[1] * 256 + convert[0];
                    log = "Expected 16bit value.\nData loss due to conversion.\nMoving first and second byte of value.\n";
                }
            }
        }

        public static bool IsValue(string arg, out int value, out string log)
        {
            log = "";
            if (!arg.EndsWith("H"))
            {
                if (int.TryParse(arg, out int result))
                {
                    log = $"Parsing value \"{arg}\" as decimal.\r\n";
                    value = result;
                    return true;
                }
            }
            else
            {
                if (int.TryParse(arg.Remove(arg.Length - 1, 1), System.Globalization.NumberStyles.HexNumber, null, out int resultFromHex))
                {
                    log = $"Parsing value \"{arg.Substring(0, arg.Length - 1)}\" as hexadecimal.\r\n";
                    value = resultFromHex;
                    return true;
                }
            }

            value = int.MinValue;
            return false;
        }
    }
}
