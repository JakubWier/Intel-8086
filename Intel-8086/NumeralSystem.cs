using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface NumeralSystem
    {
        string GetName { get; }
        string To16Bit(int number);
    }
}
