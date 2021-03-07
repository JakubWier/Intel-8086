using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    class RegistersView
    {
        public NumeralSystem NumeralSystem => numeralSystem;
        public string GetAX {
            get
            {
                string s = axValue.ToString("X");
                s = s.PadLeft(4, '0');
                return s;
            }
        }
        public string GetBX => bxValue.ToString();
        public string GetCX => cxValue.ToString();
        public string GetDX => dxValue.ToString();

        private NumeralSystem numeralSystem = NumeralSystem.Decimal;
        private int axValue = 0;
        private int bxValue = 0;
        private int cxValue = 0;
        private int dxValue = 0;

        public RegistersView(GeneralPurposeRegisters registers)
        {
            registers.RegistryChanged += RegistryChanged;
        }

        public void RegistryChanged(object sender, RegistryChangedEventArgs eventArgs)
        {
            axValue = BitConverter.ToUInt16(eventArgs.newBytes);
        }

    }
}
