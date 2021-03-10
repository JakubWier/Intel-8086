using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface NumeralConverter
    {
        string GetName { get; }
        string IntToString(int number);
    }
}
