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
    public partial class MainWindow : Window
    {
        IRegistry registry;
        ICommandController inputInterpreter;
        RegistryView registersView;
        public MainWindow()
        {
            Tests_Intel_8086.UTest.StartAllTests();

            InitializeComponent();
            BlockAX.DataContext = registersView;
            registry = new GeneralPurposeRegisters();
            inputInterpreter = new RegistryCommandInput(registry);
            registersView = new RegistryView(new HexParser());
            Description.Text = "AX FF11";

            registry.RegistryChanged += registersView.RegistryChanged;
        }

        public void SetBytesToRegistry(RegistryType registryType, string valueHex)
        {
            byte[] bytes = BitConverter.GetBytes(int.Parse(valueHex, System.Globalization.NumberStyles.HexNumber));
            registry.SetBytes(registryType, bytes[0], bytes[1]);
        }

        private void Input_Enter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                inputInterpreter.InputCommand(Input.Text);
                Input.Text = "";
            }
        }

        public void Error(string log)
        {
            Output.Text = log;
        }
    }

}
