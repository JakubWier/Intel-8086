using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Intel_8086
{
    class RegistryView
    {
        public string GetFormattedValue(RegistryType registry) => registry switch
        {
            RegistryType.AX => numeralSystem.IntToString(axValue),
            RegistryType.BX => numeralSystem.IntToString(bxValue),
            RegistryType.CX => numeralSystem.IntToString(cxValue),
            RegistryType.DX => numeralSystem.IntToString(dxValue)
        };
        public string AX => numeralSystem.IntToString(axValue);
        public string BX => numeralSystem.IntToString(bxValue);
        public string CX => numeralSystem.IntToString(cxValue);
        public string DX => numeralSystem.IntToString(dxValue);
        public string GetNumeralSystemName => numeralSystem.GetName;

        private NumeralSystem numeralSystem;
        private int axValue = 0;
        private int bxValue = 0;
        private int cxValue = 0;
        private int dxValue = 0;

        public RegistryView(NumeralSystem numeralSystem)
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
