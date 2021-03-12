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
    public partial class MainWindow : Window, IOutputController
    {
        IRegistryModel registry;
        ICommandInterpreter commandInterpreter;
        RegistryView registersView;
        public MainWindow()
        {
            Tests_Intel_8086.UTest.StartAllTests();

            InitializeComponent();
            registry = new GeneralPurposeRegisters();
            commandInterpreter = new GeneralRegistryCommand(registry, this);
            registersView = new RegistryView(new HexParser());
            if (registry is IObservable observable)
                observable.AddObserver(registersView);
            BlockAX.DataContext = registersView;
            BlockBX.DataContext = registersView;
            BlockCX.DataContext = registersView;
            BlockDX.DataContext = registersView;
            Description.Text = "AX FF11";
        }

        private void Input_Enter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Output.Text = "";
                commandInterpreter.InputCommand(Input.Text);
                Input.Text = "";
            }
        }

        public void ReplaceOutput(string line)
        {
            Output.Text = line;
        }
    }

}
