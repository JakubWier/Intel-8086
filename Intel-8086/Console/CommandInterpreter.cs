using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086.Console
{
    public interface CommandInterpreter
    {
        void InputCommand(string line);
    }
}
