using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    class RegistersView
    {
        GeneralPurposeRegisters registers;
        string numeralSystem = "Decimal";
        public string NumeralSystem => numeralSystem;

        public RegistersView(GeneralPurposeRegisters registers)
        {
            this.registers = registers;
        }

    }
}
