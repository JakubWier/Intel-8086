using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Intel_8086
{
    class RegistersView
    {
        public string GetFormattedValue(RegistryType registry) => registry switch
        {
            RegistryType.AX => numeralSystem.To16Bit(axValue),
            RegistryType.BX => numeralSystem.To16Bit(bxValue),
            RegistryType.CX => numeralSystem.To16Bit(cxValue),
            RegistryType.DX => numeralSystem.To16Bit(dxValue)
        };
        public string AX => numeralSystem.To16Bit(axValue);
        public string BX => numeralSystem.To16Bit(bxValue);
        public string CX => numeralSystem.To16Bit(cxValue);
        public string DX => numeralSystem.To16Bit(dxValue);
        public string GetNumeralSystemName => numeralSystem.GetName;

        private NumeralSystem numeralSystem;
        private int axValue = 0;
        private int bxValue = 0;
        private int cxValue = 0;
        private int dxValue = 0;

        public RegistersView(NumeralSystem numeralSystem)
        {
            this.numeralSystem = numeralSystem;
        }

        public void RegistryChanged(object sender, RegistryChangedEventArgs eventArgs)
        {
            switch (eventArgs.registry)
            {
                case RegistryType.AX:
                    axValue = BitConverter.ToUInt16(eventArgs.newBytes);
                    break;
                case RegistryType.BX:
                    bxValue = BitConverter.ToUInt16(eventArgs.newBytes);
                    break;
                case RegistryType.CX:
                    cxValue = BitConverter.ToUInt16(eventArgs.newBytes);
                    break;
                case RegistryType.DX:
                    dxValue = BitConverter.ToUInt16(eventArgs.newBytes);
                    break;
            }
        }

    }
}
