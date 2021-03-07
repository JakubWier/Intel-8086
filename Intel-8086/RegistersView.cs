using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    class RegistersView
    {
        MainWindow mainWindow;
        public string NumeralSystem => numeralSystem;
        public string GetAX => axValue.ToString();

        private string numeralSystem = "Decimal";
        private int axValue = 1000;

        public RegistersView(GeneralPurposeRegisters registers, MainWindow mainWindow)
        {
            registers.RegistryChanged += RegistryChanged;
        }

        public void RegistryChanged(object sender, RegistryChangedEventArgs eventArgs)
        {

        }

    }
}
