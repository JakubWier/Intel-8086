﻿using System;
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
using Intel_8086.Memory;
using Intel_8086.Console;

namespace Intel_8086
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, OutputController
    {
        RegistersController generalPurposeRegisters;
        RegistersController indexRegisters;
        RegistersController pointerRegisters;
        CommandInterpreter commandInterpreter;
        MemoryModel memory;
        GeneralPurposeRegistersView generalPurposeRegistersView;
        IndexRegistersView indexRegistersView;
        PointerRegistersView pointerRegistersView;
        public MainWindow()
        {
            Tests_Intel_8086.UTest.StartAllTests();
            InitializeComponent();

            memory = new MemoryModel(20);
            generalPurposeRegisters = new GeneralPurposeRegisters();
            indexRegisters = new IndexRegisters();
            pointerRegisters = new PointerRegisters();

            commandInterpreter = InitDefaultRegistryCommander(generalPurposeRegisters, indexRegisters);

            generalPurposeRegistersView = new GeneralPurposeRegistersView(new HexParser());
            indexRegistersView = new IndexRegistersView(new HexParser());
            pointerRegistersView = new PointerRegistersView(new HexParser());

            if (generalPurposeRegisters is Observable observableGeneralPurposeReg)
                observableGeneralPurposeReg.AddObserver(generalPurposeRegistersView);
            if (indexRegisters is Observable observableIndexReg)
                observableIndexReg.AddObserver(indexRegistersView);
            if (pointerRegisters is Observable observablePointersReg)
                observablePointersReg.AddObserver(pointerRegistersView);

            BlockAX.DataContext = generalPurposeRegistersView;
            BlockBX.DataContext = generalPurposeRegistersView;
            BlockCX.DataContext = generalPurposeRegistersView;
            BlockDX.DataContext = generalPurposeRegistersView;

            BlockSI.DataContext = indexRegistersView;
            BlockDI.DataContext = indexRegistersView;

            BlockBP.DataContext = pointerRegistersView;
            BlockSP.DataContext = pointerRegistersView;

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

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            generalPurposeRegisters.SetBytesToRegistry("AX", 0);
            generalPurposeRegisters.SetBytesToRegistry("BX", 0);
            generalPurposeRegisters.SetBytesToRegistry("CX", 0);
            generalPurposeRegisters.SetBytesToRegistry("DX", 0);
            Output.Text = "Registers cleared.";
        }
        private void Random_Click(object sender, RoutedEventArgs e)
        {
            Random registryValueRandomizer = new Random();

            generalPurposeRegisters.SetBytesToRegistry("AX", BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            generalPurposeRegisters.SetBytesToRegistry("BX", BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            generalPurposeRegisters.SetBytesToRegistry("CX", BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            generalPurposeRegisters.SetBytesToRegistry("DX", BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            Output.Text = "Registers randomized.";
        }

        private CommandInterpreter InitDefaultRegistryCommander(params RegistersController[] registries)
        {
            RegistryCommander commander = new RegistryCommander(this, registries);

            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            commander.AddHandler(xchg);
            commander.AddHandler(mov);
            commander.AddHandler(assignTo);

            return commander;
        }
    }

}
