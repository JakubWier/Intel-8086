using System;
/*using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using System.Windows;
using System.Windows.Controls;
/*using System.Windows.Data;
using System.Windows.Documents;*/
using System.Windows.Input;
/*using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;*/

namespace Intel_8086
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICommandObserver
    {
        GeneralPurposeRegisters registers;
        RegistersView registersView;
        InputInterpreter inputInterpreter;
        public MainWindow()
        {
            Tests_Intel_8086.UTest.StartAllTests();

            InitializeComponent();
            BlockAX.DataContext = registersView;
            registers = new GeneralPurposeRegisters();
            registersView = new RegistersView(new HexParser());
            inputInterpreter = new InputInterpreter(this);
            Description.Text = "AX FF11";

            registers.RegistryChanged += registersView.RegistryChanged;
        }

        public void SetBytesToRegistry(RegistryType registryType, string valueHex)
        {
            byte[] bytes = BitConverter.GetBytes(int.Parse(valueHex, System.Globalization.NumberStyles.HexNumber));
            registers.SetBytes(registryType, bytes[0], bytes[1]);
            WriteToRegistryUI(registryType);
        }

        private void WriteToRegistryUI(RegistryType registryType)
        {
            switch (registryType)
            {
                case RegistryType.AX:
                case RegistryType.AL:
                case RegistryType.AH:
                    BlockAX.Text = registersView.GetFormattedValue(RegistryType.AX);
                    break;
                case RegistryType.BX:
                case RegistryType.BL:
                case RegistryType.BH:
                    BlockBX.Text = registersView.GetFormattedValue(RegistryType.BX);
                    break;
                case RegistryType.CX:
                case RegistryType.CL:
                case RegistryType.CH:
                    BlockCX.Text = registersView.GetFormattedValue(RegistryType.CX);
                    break;
                case RegistryType.DX:
                case RegistryType.DL:
                case RegistryType.DH:
                    BlockDX.Text = registersView.GetFormattedValue(RegistryType.DX);
                    break;
            }
        }

        private void Input_Enter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                inputInterpreter.CommandLine(Input.Text);
                Input.Text = "";
            }
        }
    }

}
