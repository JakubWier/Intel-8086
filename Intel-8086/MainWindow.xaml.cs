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
        Registry[] registries;
        CommandInterpreter commandInterpreter;
        RegistryView registersView;
        public MainWindow()
        {
            //Tests_Intel_8086.UTest.StartAllTests();

            InitializeComponent();

            registries = InitializeGeneralPurposeRegisters();

            MemoryModel memory = new MemoryModel(20);
            registersView = new RegistryView(new HexParser());
            commandInterpreter = new RegistersCommander(this, registries);

            foreach (Observable registry in registries)
                registry.AddObserver(registersView);

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

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            /*registry.SetBytesToRegistry(GeneralPurposeRegistryType.AX, 0);
            registry.SetBytesToRegistry(GeneralPurposeRegistryType.BX, 0);
            registry.SetBytesToRegistry(GeneralPurposeRegistryType.CX, 0);
            registry.SetBytesToRegistry(GeneralPurposeRegistryType.DX, 0);*/
            Output.Text = "Registers cleared.";
        }
        private void Random_Click(object sender, RoutedEventArgs e)
        {
            Random registryValueRandomizer = new Random();

            /*registry.SetBytesToRegistry(GeneralPurposeRegistryType.AX, BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            registry.SetBytesToRegistry(GeneralPurposeRegistryType.BX, BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            registry.SetBytesToRegistry(GeneralPurposeRegistryType.CX, BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));
            registry.SetBytesToRegistry(GeneralPurposeRegistryType.DX, BitConverter.GetBytes(registryValueRandomizer.Next(0, 65536)));*/
            Output.Text = "Registers randomized.";
        }

        private Registry[] InitializeGeneralPurposeRegisters()
        {
            return new[] {
                new DoubleRegistry("AX", new CommonRegistry("AL", 1), new CommonRegistry("AH", 1)),
                new DoubleRegistry("BX", new CommonRegistry("BL", 1), new CommonRegistry("BH", 1)),
                new DoubleRegistry("CX", new CommonRegistry("CL", 1), new CommonRegistry("CH", 1)),
                new DoubleRegistry("DX", new CommonRegistry("DL", 1), new CommonRegistry("DH", 1))
            };
        }
    }

}
