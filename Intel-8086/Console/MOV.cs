using System;
using System.Text;
using Intel_8086.Registers;

namespace Intel_8086.Console
{
    class MOV : ProcedureHandler
    {
        RegistryOperator[] registryOperators;
        StringBuilder outputLogBuilder;
        public MOV(ProcedureHandler nextHandler, params RegistryOperator[] containers)
        {
            NextHandler = nextHandler;
            registryOperators = containers;
        }

        public ProcedureHandler NextHandler { get; set; }

        public string HandleOperation(string[] args)
        {
            if (IsCommandMOV(args[0]))
                if (args.Length > 2)
                {
                    outputLogBuilder = new StringBuilder();

                    int argSeparatorPos = args[1].IndexOf(',');
                    if (argSeparatorPos == -1)
                        return "MOV arguments must be separated by comma.";

                    args[1] = args[1].Remove(argSeparatorPos, 1);

                    if (!IsRegistryName(args[1], out RegistryOperator registryOperator))
                        return $"{args[1]} is unknown registry name.";

                    if (TrySetValueToRegistry(args[1], args[2]))
                        return outputLogBuilder.ToString();
                    if (TrySetRegistryToRegistry(args[1], args[2]))
                        return outputLogBuilder.ToString();
                    return outputLogBuilder.ToString();
                }
                else
                {
                    return "Too few arguments to function MOV.";
                }

            if (NextHandler!=null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool TrySetValueToRegistry(string destinatedRegistry, string potentialValue)
        {
            if (IsValue(potentialValue, out int valueArg))
            {
                char regPostfix = destinatedRegistry[destinatedRegistry.Length - 1];
                CheckAndReduceOverflow(ref valueArg, regPostfix);
                SetValueToRegistry(destinatedRegistry, valueArg);
                string valueHex = valueArg.ToString("X");
                outputLogBuilder.Append($"{(valueHex.Length <= 2 ? valueHex.PadLeft(2, '0') : valueHex.PadLeft(4, '0')) } moved into {destinatedRegistry}.");
                return true;
            }

            return false;
        }

        private void SetValueToRegistry(string destinatedRegName, int value)
        {
            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) };
            GeneralPurposeRegistryType registryType = (GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), destinatedRegName);
            //registryModel.SetBytesToRegistry(registryType, bytes);
        }

        /*private bool IsValueArgument(string argument, out int valueArg)
        {
            if (IsValue(argument, out int value)) //"mov reg,value"
            {
                valueArg = value;
                return true;
            }
            valueArg = int.MinValue;
            return false;
        }*/

        private void CheckAndReduceOverflow(ref int value, char registryPostfix)
        {
            if (registryPostfix == 'L')
            {
                if (value > byte.MaxValue)
                {
                    value = BitConverter.GetBytes(value)[0]; //255;
                    outputLogBuilder.Append("Expected 8bit value.\nData loss due to conversion.\nMoving first byte.\n");
                }
            } else if(registryPostfix == 'H')
            {
                if (value > byte.MaxValue)
                {
                    value = BitConverter.GetBytes(value)[1]; //255;
                    outputLogBuilder.Append("Expected 8bit value.\nData loss due to conversion.\nMoving first byte.\n");
                }
            }
            else
            {
                if (value > ushort.MaxValue)
                {
                    byte[] convert = BitConverter.GetBytes(value);
                    value = convert[1]*256 + convert[0];
                    outputLogBuilder.Append("Expected 16bit value.\nData loss due to conversion.\nMoving first two bytes.\n");
                }
            }
        }

        private bool TrySetRegistryToRegistry(string destinatedRegistry, string potentialSourcedRegistry)
        {
            if (IsRegistryName(potentialSourcedRegistry))//"mov reg,reg2"
            {
                //SetRegistryToRegistry(destinatedRegistry, potentialSourcedRegistry);
                outputLogBuilder.Append($"{potentialSourcedRegistry} moved into {destinatedRegistry}.");
                return true;
            }
            outputLogBuilder.Append($"{potentialSourcedRegistry} is unknown registry name.");
            return false;
        }

        /*private void SetRegistryToRegistry(string destinatedRegistry, string sourcedRegistry)
        {
            GeneralPurposeRegistryType destinatedRegistryType = (GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), destinatedRegistry);
            GeneralPurposeRegistryType sourcedRegistryType = (GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), sourcedRegistry);
            byte[] bytes = registryContainers.GetRegistry(sourcedRegistryType);

            if (sourcedRegistry.EndsWith('H'))
                registryContainers.SetBytesToRegistry(destinatedRegistryType, bytes[1]);
            else if (sourcedRegistry.EndsWith('L'))
                registryContainers.SetBytesToRegistry(destinatedRegistryType, bytes[0]);
            else
                registryContainers.SetBytesToRegistry(destinatedRegistryType, bytes);
        }*/

        private bool IsValue(string arg, out int value)
        {
            if (!arg.EndsWith("H"))
            {
                if (int.TryParse(arg, out int result))
                {
                    outputLogBuilder.Append("Parsing value from decimal.\n");
                    value = result;
                    return true;
                }
            }
            else
            {
                if (int.TryParse(arg.Remove(arg.Length-1, 1), System.Globalization.NumberStyles.HexNumber, null, out int resultFromHex))
                {
                    outputLogBuilder.Append("Parsing value from hexadecimal.\n");
                    value = resultFromHex;
                    return true;
                }
            }
            value = int.MinValue;
            return false;
        }

        private bool IsCommandMOV(string potentialMovKeyword)
        {
            return potentialMovKeyword == "MOV" ? true : false;
        }

        private bool IsRegistryName(string potentialRegistryName, out RegistryOperator registryOperator)
        {
            registryContainer = null;

            if (potentialRegistryName.Length != 2)
                return false;

            foreach (RegistryOperator container in registryOperators)
                if (container.Contains(potentialRegistryName))
                {
                    registryContainer = container;
                    return true;
                }

            return false;
        }
    }
}
