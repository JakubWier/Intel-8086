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
using Intel_8086.Registers;
using Intel_8086.MemorySystem;
using Intel_8086.Console;

namespace Intel_8086
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, OutputController, Observer
    {
        GeneralPurposeRegisters generalPurposeRegisters;
        IndexRegisters indexRegisters;
        PointerRegisters pointerRegisters;
        SegmentRegisters segmentRegisters;

        GeneralPurposeRegistersView generalPurposeRegistersView;
        IndexRegistersView indexRegistersView;
        PointerRegistersView pointerRegistersView;
        SegmentRegistersView segmentRegistersView;
        MemoryView memoryView;

        CommandInterpreter commandInterpreter;
        public MainWindow()
        {
            InitializeComponent();

            MemoryModel.SetAddressBusLength = 20;
            MemoryModel.GetInstance().AddObserver(this);
            memoryView = new MemoryView();
            Go_Click(null, null);

            generalPurposeRegisters = new GeneralPurposeRegisters();
            indexRegisters = new IndexRegisters();
            pointerRegisters = new PointerRegisters();
            segmentRegisters = new SegmentRegisters();

            commandInterpreter = InitDefaultRegistryCommander();

            generalPurposeRegistersView = new GeneralPurposeRegistersView(new HexParser());
            indexRegistersView = new IndexRegistersView(new HexParser());
            pointerRegistersView = new PointerRegistersView(new HexParser());
            segmentRegistersView = new SegmentRegistersView(new HexParser());

            if (generalPurposeRegisters is Observable observableGeneralPurposeReg)
                observableGeneralPurposeReg.AddObserver(generalPurposeRegistersView);
            if (indexRegisters is Observable observableIndexReg)
                observableIndexReg.AddObserver(indexRegistersView);
            if (pointerRegisters is Observable observablePointersReg)
                observablePointersReg.AddObserver(pointerRegistersView);
            if (segmentRegisters is Observable observableSegmentsReg)
                observableSegmentsReg.AddObserver(segmentRegistersView);

            SetBlockDataContext();
        }

        private void Input_Enter(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Output.Text = "";
                commandInterpreter.InputCommand(Input.Text);
                Description.Text += Input.Text + "\n";
                Input.Text = "";
            }
        }

        public void ReplaceOutput(string line)
        {
            Output.Text = line;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            generalPurposeRegisters.SetBytesToRegistry("AX", 0);
            generalPurposeRegisters.SetBytesToRegistry("BX", 0);
            generalPurposeRegisters.SetBytesToRegistry("CX", 0);
            generalPurposeRegisters.SetBytesToRegistry("DX", 0);
            Output.Text = "General purpose registers cleared.";
        }
        private void Random_Click(object sender, RoutedEventArgs e)
        {
            Random registryValueRandomizer = new Random();

            generalPurposeRegisters.SetBytesToRegistry("AX", BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            generalPurposeRegisters.SetBytesToRegistry("BX", BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            generalPurposeRegisters.SetBytesToRegistry("CX", BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            generalPurposeRegisters.SetBytesToRegistry("DX", BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            Output.Text = "General purpose registers randomized.";
        }

        private CommandInterpreter InitDefaultRegistryCommander()
        {
            RegistryCommander commander = new RegistryCommander(this, generalPurposeRegisters, indexRegisters, pointerRegisters, segmentRegisters);

            PUSH push = new PUSH();
            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            commander.AddHandler(push);
            commander.AddHandler(xchg);
            commander.AddHandler(mov);
            commander.AddHandler(assignTo);

            return commander;
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            string input = MemoryInput.Text;
            if (ushort.TryParse(input.Substring(0, 4), System.Globalization.NumberStyles.HexNumber, null, out ushort address))
                if (ushort.TryParse(input.Substring(input.Length - 4, 4), System.Globalization.NumberStyles.HexNumber, null, out ushort offset))
                {
                    MemoryOutput.Text = memoryView.GetView(address, offset);
                    return;
                }
            Output.Text = "Unable to find correct memory location.\nInvalid address formatting.";
        }

        public void SetBlockDataContext()
        {
            BlockAX.DataContext = generalPurposeRegistersView;
            BlockBX.DataContext = generalPurposeRegistersView;
            BlockCX.DataContext = generalPurposeRegistersView;
            BlockDX.DataContext = generalPurposeRegistersView;

            BlockSI.DataContext = indexRegistersView;
            BlockDI.DataContext = indexRegistersView;

            BlockBP.DataContext = pointerRegistersView;
            BlockSP.DataContext = pointerRegistersView;

            BlockCS.DataContext = segmentRegistersView;
            BlockSS.DataContext = segmentRegistersView;
            BlockDS.DataContext = segmentRegistersView;
            BlockES.DataContext = segmentRegistersView;
        }

        public void Update(object update)
        {
            if (update is uint memoryAddress)
            {
                MemoryInput.Text = MemoryView.AddressInputToString(memoryAddress);
                Go_Click(null, null);
            }
        }
    }

}
