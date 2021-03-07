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
        GeneralPurposeRegisters registers;
        RegistersView registersView;
        OutputLogger logger;
        public MainWindow()
        {
            Tests_Intel_8086.UTest.StartAllTests();

            InitializeComponent();
            registers = new GeneralPurposeRegisters();
            registersView = new RegistersView(registers);
            logger = new OutputLogger(Output);

            BlockAX.DataContext = registersView;
            BlockBX.DataContext = registersView;
            BlockCX.DataContext = registersView;
            BlockDX.DataContext = registersView;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void BlockAX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (registersView.NumeralSystem == NumeralSystem.Decimal)
                {
                    byte[] bytes = BitConverter.GetBytes(Convert.ToInt64(((TextBox)sender).Text));
                    registers.SetBytes(RegistryType.AX, bytes);
                    ((TextBox)sender).Text = registersView.GetAX;
                    logger.WriteLog(registersView.GetAX);
                }

            }
        }

        private void ButtonSET_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (registersView.NumeralSystem == NumeralSystem.Decimal)
                {
                    RegistryType registry = (RegistryType)SelectedRegistry.SelectedItem;
                    byte[] bytes = BitConverter.GetBytes(Convert.ToInt64(InputValue.Text));
                    registers.SetBytes(registry, bytes);
                    BlockAX.Text = registersView.GetAX;
                }
            } catch (FormatException ex)
            {

            }
        }
    }
}
