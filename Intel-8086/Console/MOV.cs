using System;
using System.Text;
using Intel_8086.Registers;
using Intel_8086.Memory;

namespace Intel_8086.Console
{
    class MOV : CommandHandler
    {
        public CommandHandler NextHandler { get; set; }

        private StringBuilder outputLogBuilder;
        private RegistersController[] processedRegisters;
        //private string[] processedArgs;

        public string HandleOperation(string[] args, params RegistersController[] registryControllers)
        {
            processedRegisters = registryControllers;

            if (IsCommandMOV(args[0]))
                if (args.Length > 2)
                {
                    outputLogBuilder = new StringBuilder();

                    int argSeparatorPos = FindAndRemoveArgSeparator(args);
                    if (argSeparatorPos == -1)
                        return "MOV arguments must be separated by comma.";

                    if (ContainsAddressArgument(args, out string[] addressArgs))
                    {
                        if (TryParseAddressToValue(addressArgs, out int address))
                        {
                            if(argSeparatorPos == 1)
                                if(IsSupportedRegistryName(args[1], out RegistersController registryController))
                                {
                                    TrySetMemoryToRegistry(registryController, args[1], address);
                                }
                        }
                        return outputLogBuilder.ToString();
                    }

                    if (!IsSupportedRegistryName(args[1], out RegistersController destinatedController))
                        return $"{args[1]} is unknown registry name.";

                    if (TrySetRegistryToRegistry(destinatedController, args[1], args[2]))
                        return outputLogBuilder.ToString();

                    if (TrySetValueToRegistry(destinatedController, args[1], args[2]))
                        return outputLogBuilder.ToString();

                    return outputLogBuilder.ToString();
                }
                else
                {
                    return "Too few arguments to function MOV.";
                }

            if (NextHandler!=null)
                return NextHandler.HandleOperation(args, processedRegisters);
            else
                return "";
        }

        private bool IsCommandMOV(string potentialMovKeyword) => (potentialMovKeyword == "MOV");

        private int FindAndRemoveArgSeparator(string[] args)
        {
            int pos = 0;
            int index = -1;

            for(;pos < args.Length; pos++)
            {
                index = args[pos].IndexOf(',');
                if (index != -1)
                {
                    args[pos] = args[pos].Remove(index, 1);
                    break;
                }
            }
            if (pos == args.Length)
                return -1;

            return pos;
        }

        private void TrySetMemoryToRegistry(RegistersController registryController, string destinatedRegistry, int memoryAddress)
        {
            if(IsSupportedRegistryName("DS", out RegistersController segmentsController))
            {
                int dataSegment = BitConverter.ToInt16(segmentsController.GetRegistry("DS"));
                dataSegment = dataSegment << 4;
                int physicalAddress = dataSegment + memoryAddress;
                MemoryModel memory = MemoryModel.GetInstance();
                byte[] word = memory.GetMemoryWord(physicalAddress);
                SetValueToRegistry(registryController, destinatedRegistry, BitConverter.ToUInt16(word));
                outputLogBuilder.AppendLine(BitConverter.ToUInt16(word).ToString());
                outputLogBuilder.AppendLine(physicalAddress.ToString("X"));
            }
        }

        private bool TrySetValueToRegistry(RegistersController registryController, string destinatedRegistry, string potentialValue)
        {
            if (IsValue(potentialValue, out Int64 valueArg))
            {
                char regPostfix = destinatedRegistry[^1];
                CheckAndReduceOverflow(ref valueArg, regPostfix);
                SetValueToRegistry(registryController, destinatedRegistry, valueArg);
                string valueHex = valueArg.ToString("X");
                outputLogBuilder.Append($"{(valueHex.Length <= 2 ? valueHex.PadLeft(2, '0') : valueHex.PadLeft(4, '0')) } moved into {destinatedRegistry}.");
                return true;
            }
            return false;
        }

        private void SetValueToRegistry(RegistersController registryController, string destinatedRegName, Int64 value)
        {
            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) };
            registryController.SetBytesToRegistry(destinatedRegName, bytes);
        }


        private void CheckAndReduceOverflow(ref Int64 value, char registryPostfix = '0')
        {
            if (registryPostfix == 'L' || registryPostfix == 'H')
            {
                if (value > byte.MaxValue)
                {
                    value = BitConverter.GetBytes(value)[0]; //255;
                    outputLogBuilder.Append("Expected 8bit value.\nData loss due to conversion.\nMoving first byte of value.\n");
                }
            }
            else
            {
                if (value > ushort.MaxValue)
                {
                    byte[] convert = BitConverter.GetBytes(value);
                    value = convert[1]*256 + convert[0];
                    outputLogBuilder.Append("Expected 16bit value.\nData loss due to conversion.\nMoving first and second byte of value.\n");
                }
            }
        }

        private bool TrySetRegistryToRegistry(RegistersController destinatedController, string destinatedRegistry, string potentialSourcedRegistry)
        {
            if (IsSupportedRegistryName(potentialSourcedRegistry, out RegistersController sourcedController))//"mov reg,reg2"
            {
                SetRegistryToRegistry(destinatedController, sourcedController, destinatedRegistry, potentialSourcedRegistry);
                outputLogBuilder.Append($"{potentialSourcedRegistry} moved into {destinatedRegistry}.");
                return true;
            }
            outputLogBuilder.Append($"{potentialSourcedRegistry} is unknown registry name.");
            return false;
        }

        private void SetRegistryToRegistry(RegistersController destinatedController, RegistersController sourcedController, string destinatedRegistry, string sourcedRegistry)
        {
            byte[] bytes = sourcedController.GetRegistry(sourcedRegistry);

            if (sourcedRegistry.EndsWith('H'))
                destinatedController.SetBytesToRegistry(destinatedRegistry, bytes[1]);
            else if (sourcedRegistry.EndsWith('L'))
                destinatedController.SetBytesToRegistry(destinatedRegistry, bytes[0]);
            else
                destinatedController.SetBytesToRegistry(destinatedRegistry, bytes);
        }

        private bool IsValue(string arg, out Int64 value)
        {
            if (!arg.EndsWith("H"))
            {
                if (Int64.TryParse(arg, out Int64 result))
                {
                    outputLogBuilder.Clear();
                    outputLogBuilder.AppendLine($"Parsing value \"{arg}\" as decimal.");
                    value = result;
                    return true;
                }
            }
            else
            {
                if (Int64.TryParse(arg.Remove(arg.Length-1, 1), System.Globalization.NumberStyles.HexNumber, null, out Int64 resultFromHex))
                {
                    outputLogBuilder.Clear();
                    outputLogBuilder.AppendLine($"Parsing value \"{arg.Substring(0, arg.Length-1)}\" as hexadecimal.");
                    value = resultFromHex;
                    return true;
                }
            }

            value = int.MinValue;
            return false;
        }

        private bool IsSupportedRegistryName(string potentialRegistryName, out RegistersController registryController)
        {
            if (potentialRegistryName.Length == 2)
                foreach (RegistersController container in processedRegisters)
                {
                    if (container.Contains(potentialRegistryName))
                    {
                        registryController = container;
                        return true;
                    }
                }

            registryController = null;
            return false;
        }

        private bool TryParseAddressToValue(string []addressArgs, out int address)
        {
            //Adresowanie bazowe (BX lub BP)
            address = 0;
            foreach (string arg in addressArgs)
            {
                if (arg == "+")
                {
                    continue;
                }
                if (IsIndexRegister(arg) || IsBaseRegister(arg))
                {
                    if (IsSupportedRegistryName(arg, out RegistersController regController))
                    {
                        address += BitConverter.ToInt16(regController.GetRegistry(arg));
                    }
                } else if(IsValue(arg, out long value))
                {
                    CheckAndReduceOverflow(ref value);
                    address += (int)value;
                }
                else
                {
                    outputLogBuilder.AppendLine($"Cannot parse arguments as memory address.");
                    outputLogBuilder.AppendLine($"Argument \"{arg}\" is invalid.");
                    return false;
                }
            }
            outputLogBuilder.AppendLine($"Calculating memory address from arguments.");
            outputLogBuilder.AppendLine($"The effective address is {address.ToString("X")}h.");
            return true;
        }

        private bool IsIndexRegister(string potentialRegister) => (potentialRegister == "SI" || potentialRegister == "DI");
        
        private bool IsBaseRegister(string potentialRegister) => (potentialRegister == "BX" || potentialRegister == "BP");

        private bool ContainsAddressArgument(string[] args, out string[] addressArgs)
        {
            int startBracketPos = -1, endBracketPos = -1;
            int startIndex = 1, endIndex = 1;

            for (int iterator = 1; iterator < args.Length; iterator++)
            {
                if (startBracketPos == -1)
                {
                    startBracketPos = args[iterator].IndexOf('[');
                    startIndex = iterator;
                }
            }

            for (int iterator = startIndex; iterator < args.Length; iterator++)
            {
                if (endBracketPos == -1)
                {
                    endBracketPos = args[iterator].IndexOf(']');
                    endIndex = iterator;
                }
                else
                    break;
            }

            if (startBracketPos != -1 || endBracketPos != -1) //AND, OR?
            {
                addressArgs = new string[endIndex - startIndex + 1];
                args[startIndex] = args[startIndex].Remove(startBracketPos, 1);

                if (startIndex == endIndex)
                    args[endIndex] = args[endIndex].Remove(endBracketPos - 1, 1);
                else
                    args[endIndex] = args[endIndex].Remove(endBracketPos, 1);

                for (int it = startIndex, k = 0; it <= endIndex; it++, k++)
                {
                    addressArgs[k] = args[it];
                }
                return true;
            }
            addressArgs = null;
            return false;
        }
    }
}
