using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086.CommandInterpreter
{
    class MOV : IProcedureHandling
    {
        IRegistryModel registryModel;
        StringBuilder outputLogBuilder;
        public MOV(IProcedureHandling nextHandler, IRegistryModel registry)
        {
            NextHandler = nextHandler;
            registryModel = registry;
        }

        public IProcedureHandling NextHandler { get; set; }

        public string HandleOperation(string[] args)
        {
            outputLogBuilder = new StringBuilder();
            if (args.Length > 1)
                if (IsCommandMOV(args[0]))
                {
                    args[1] = args[1].Replace(',', ' ');
                    string[] movArguments = args[1].Split(' ');
                    if (IsRegistryName(movArguments[0]))
                    {
                        if (TrySetValueToRegistry(movArguments, args))
                            return outputLogBuilder.ToString();
                        if (TrySetRegistryToRegistry(movArguments, args))
                            return outputLogBuilder.ToString();
                        return "Invalid MOV command arguments.";
                    }
                    return $"{movArguments[0]} is unknown registry name.";
                }

            if (NextHandler!=null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool TrySetValueToRegistry(string[] movArguments, string[] commandBuffer)
        {
            string destinatedRegistryName = movArguments[0];

            if (TryFindValueArgument(movArguments, out int valueArg))
            {
                char regPostfix = destinatedRegistryName[destinatedRegistryName.Length - 1];
                CheckAndReduceOverflow(ref valueArg, regPostfix);
                SetValueToRegistry(movArguments[0], valueArg);
                string valueHex = valueArg.ToString("X");
                outputLogBuilder.Append($"{(valueHex.Length <= 2 ? valueHex.PadLeft(2, '0') : valueHex.PadLeft(4, '0')) } moved into {destinatedRegistryName}.");
                return true;
            }

            if (TryFindValueArgument(commandBuffer, out int valueArgBuffer))
            {
                char regPostfix = destinatedRegistryName[destinatedRegistryName.Length - 1];
                CheckAndReduceOverflow(ref valueArgBuffer, regPostfix);
                SetValueToRegistry(movArguments[0], valueArgBuffer);
                string valueHex = valueArgBuffer.ToString("X");
                outputLogBuilder.Append($"{(valueHex.Length <= 2 ? valueHex.PadLeft(2, '0') : valueHex.PadLeft(4, '0')) } moved into {destinatedRegistryName}.");
                return true;
            }
            return false;
        }

        private void SetValueToRegistry(string destinatedRegName, int value)
        {
            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) };
            RegistryType registryType = (RegistryType)Enum.Parse(typeof(RegistryType), destinatedRegName);
            registryModel.SetBytesToRegistry(registryType, bytes);
        }

        private bool TryFindValueArgument(string[] argumentBuffer, out int valueArg)
        {
            for (int i = 1; i < argumentBuffer.Length; i++)
            {
                if (IsValue(argumentBuffer[i], out int value)) //"mov reg,value"
                {
                    valueArg = value;
                    return true;
                }
            }
            valueArg = int.MinValue;
            return false;
        }

        private void CheckAndReduceOverflow(ref int value, char registryPostfix)
        {
            if (registryPostfix == 'L' || registryPostfix == 'H')
            {
                if (value > 255)
                {
                    value = 255;
                    outputLogBuilder.Append("Input value was too big.\nAssigned max value.\n");
                }
            }
            else
            {
                if (value > 65535)
                {
                    value = 65535;
                    outputLogBuilder.Append("Input value was too big.\nAssigned max value.\n");
                }
            }
        }

        private bool TrySetRegistryToRegistry(string[] movArguments, string[] commandBuffer)
        {
            for (int i = 1; i < movArguments.Length; i++)
            {
                if (IsRegistryName(movArguments[i]))//"mov reg,reg2"
                {
                    string destinatedRegistry = movArguments[0];
                    string sourcedRegistry = movArguments[1];
                    SetRegistryToRegistry(destinatedRegistry, sourcedRegistry);
                    outputLogBuilder.Append($"{sourcedRegistry} moved into {destinatedRegistry}.");
                    return true;
                }
            }
            for (int i = 1; i < commandBuffer.Length; i++)
            {
                if (IsRegistryName(commandBuffer[i]))//"mov reg, reg2"
                {
                    string destinatedRegistry = movArguments[0];
                    string sourcedRegistry = commandBuffer[i];
                    SetRegistryToRegistry(destinatedRegistry, sourcedRegistry);
                    outputLogBuilder.Append($"{sourcedRegistry} moved into {destinatedRegistry}.");
                    return true;
                }
            }
            return false;
        }

        private void SetRegistryToRegistry(string destinatedRegistry, string sourcedRegistry)
        {
            RegistryType destinatedRegistryType = (RegistryType)Enum.Parse(typeof(RegistryType), destinatedRegistry);
            RegistryType sourcedRegistryType = (RegistryType)Enum.Parse(typeof(RegistryType), sourcedRegistry);
            byte[] bytes = registryModel.GetRegistry(sourcedRegistryType);

            if (sourcedRegistry.EndsWith('H'))
                registryModel.SetBytesToRegistry(destinatedRegistryType, bytes[1]);
            else if (sourcedRegistry.EndsWith('L'))
                registryModel.SetBytesToRegistry(destinatedRegistryType, bytes[0]);
            else
                registryModel.SetBytesToRegistry(destinatedRegistryType, bytes);
        }

        private bool IsValue(string arg, out int value)
        {
            if(int.TryParse(arg, out int result))
            {
                value = result;
                return true;
            }
            value = int.MinValue;
            return false;
        }

        private bool IsCommandMOV(string potentialMovKeyword)
        {
            if (potentialMovKeyword.ToLower() == "mov")
                return true;
            return false;
        }

        private bool IsRegistryName(string potentialRegistryName)
        {
            potentialRegistryName = potentialRegistryName.ToLower();
            if (potentialRegistryName.Length == 2)
                foreach (string reg in Enum.GetNames(typeof(RegistryType)))
                {
                    if (potentialRegistryName == reg.ToLower())
                        return true;
                }
            return false;
        }
    }
}
