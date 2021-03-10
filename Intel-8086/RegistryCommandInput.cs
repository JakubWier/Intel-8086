using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    class RegistryCommandInput : ICommandController
    {
        private IRegistry observer;
        private IOutputController output;

        public RegistryCommandInput(IRegistry observer, IOutputController output) { 
            this.observer = observer;
            this.output = output;
        }

        public void InputCommand(string line)
        {
            string[] commandBlockBuffer = line.Split(' ');
            if (IsCommandSetFixedToRegistry(commandBlockBuffer[0]))
                TryParseSetFixedToRegistry(commandBlockBuffer[0], commandBlockBuffer[1]);
        }

        private bool IsCommandSetFixedToRegistry(string potentialRegistryName)
        {
            if (potentialRegistryName.Length == 2)
                foreach (string reg in Enum.GetNames(typeof(RegistryType)))
                {
                    if (potentialRegistryName == reg)
                        return true;
                }
            return false;
        }

        private void TryParseSetFixedToRegistry(string registryName, string valueHex)
        {
            if (valueHex.Length > 4)
                valueHex = valueHex.Substring(valueHex.Length - 4, 4);
            try
            {
                byte[] bytes = BitConverter.GetBytes(int.Parse(valueHex, System.Globalization.NumberStyles.HexNumber));
                observer.SetBytesToRegistry((RegistryType)Enum.Parse(typeof(RegistryType), registryName), bytes);
            } catch(FormatException)
            {
                output.ReplaceOutput($"Cannot parse {valueHex} as hexadecimal");
            }
            catch (ArgumentException arg)
            {
                output.ReplaceOutput(arg.Message);
            }

        }
    }
}
