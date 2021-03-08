using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    class InputInterpreter
    {
        private ICommandObserver observer;

        public InputInterpreter(ICommandObserver observer) { this.observer = observer; }
        public void CommandLine(string line)
        {
            string[] commandBlocks = line.Split(' ');
            if (IsCommandSetValue(commandBlocks[0]))
            {
                if (commandBlocks[1].Length > 4)
                    commandBlocks[1] = commandBlocks[1].Substring(0, 4);
                observer.SetBytesToRegistry((RegistryType)Enum.Parse(typeof(RegistryType),commandBlocks[0]), commandBlocks[1]);
            }
        }

        private bool IsCommandSetValue(string command)
        {
            if (command.Length == 2)
                foreach (string reg in Enum.GetNames(typeof(RegistryType)))
                {
                    if (command == reg)
                        return true;
                }
            return false;
        }
    }
}
