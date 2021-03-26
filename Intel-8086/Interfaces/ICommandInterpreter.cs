using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface ICommandInterpreter
    {
        void InputCommand(string line);
    }
}
