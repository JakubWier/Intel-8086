using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    class GeneralRegistryCommand : ICommandInterpreter
    {
        private IRegistryModel registryModel;
        private IOutputController output;

        public GeneralRegistryCommand(IRegistryModel registryModel, IOutputController output) { 
            this.registryModel = registryModel;
            this.output = output;
        }

        /// <summary>
        /// Converts command line into methods and executes them on registry model.
        /// </summary>
        /// <param name="line">Command line.</param>
        /// <returns>Returns method interpreted name.</returns>
        public void InputCommand(string line)
        {
            string[] commandBlockBuffer = line.Split(' ');
            if (line.Length == 0)
                return;
            if (IsCommandSetFixedToRegistry(commandBlockBuffer[0]))
            {
                TryParseSetFixedToRegistry(commandBlockBuffer[0], commandBlockBuffer[1]);
                return;
            }
            output.ReplaceOutput("Invalid command line.");
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
                byte[] bytes = (valueHex.Length <= 2) ? new []{ Convert.ToByte(valueHex, 16) } : BitConverter.GetBytes(Convert.ToInt16(valueHex, 16));
                registryModel.SetBytesToRegistry((RegistryType)Enum.Parse(typeof(RegistryType), registryName), bytes);
                output.ReplaceOutput($"{ (valueHex.Length>2 ? valueHex.PadLeft(4,'0') : valueHex.PadLeft(2, '0') )} assigned into {registryName}.");
            }
            catch (FormatException)
            {
                output.ReplaceOutput($"Cannot parse \"{valueHex}\" as hexadecimal.");
            }
            catch (ArgumentException arg)
            {
                output.ReplaceOutput(arg.Message);
            }


        }
    }
}
