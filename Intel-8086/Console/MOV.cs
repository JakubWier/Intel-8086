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

                    if (ContainsAddressArgument(args, out string[] addressArgs, out bool isRightOperand))
                    {
                        if (TryParseEffectiveAddressToValue(addressArgs, out int address))
                        {
                            if (isRightOperand)
                            {
                                if (IsSupportedRegistryName(args[1], out RegistersController destinatedRegistryContainer))
                                    TrySetMemoryToRegistry(destinatedRegistryContainer, args[1], address);
                            }
                            else
                            {
                                if (IsSupportedRegistryName(args[^1], out RegistersController sourcedRegistryContainer))
                                    TrySetRegistryToMemory(sourcedRegistryContainer, args[^1], address);
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

        private void TrySetMemoryToRegistry(RegistersController registryController, string destinatedRegistry, int effectiveAddress)
        {
            if(IsSupportedRegistryName("DS", out RegistersController segmentsController))
            {
                int dataSegment = BitConverter.ToUInt16(segmentsController.GetRegistry("DS"));
                dataSegment = dataSegment << 4;
                uint physicalAddress = (uint)(dataSegment + effectiveAddress);
                MemoryModel memory = MemoryModel.GetInstance();
                byte[] word = memory.GetMemoryWord(physicalAddress);
                int value = BitConverter.ToUInt16(word);
                CheckAndReduceOverflow(ref value, destinatedRegistry[^1]);
                SetValueToRegistry(registryController, destinatedRegistry, value);
                outputLogBuilder.AppendLine($"Value {value.ToString("X")}h assigned to registry {destinatedRegistry} from physical address {physicalAddress.ToString("X")}h.");
            }
        }

        private void TrySetRegistryToMemory(RegistersController registryController, string sourcedRegistry, int effectiveAddress)
        {
            if (IsSupportedRegistryName("DS", out RegistersController segmentsController))
            {
                int dataSegment = BitConverter.ToUInt16(segmentsController.GetRegistry("DS"));
                dataSegment = dataSegment << 4;
                uint physicalAddress = (uint)(dataSegment + effectiveAddress);
                MemoryModel memory = MemoryModel.GetInstance();
                ushort value = BitConverter.ToUInt16(registryController.GetRegistry(sourcedRegistry));
                if(sourcedRegistry[^1] != 'L' && sourcedRegistry[^1] != 'H')
                    memory.SetMemoryWord(physicalAddress, value);
                else
                    memory.SetMemoryCell(physicalAddress, BitConverter.GetBytes(value)[0]);
                outputLogBuilder.AppendLine($"Value {value.ToString("X")}h from registry {sourcedRegistry} assigned to physical address {physicalAddress.ToString("X")}h.");
            }
        }

        private bool TrySetValueToRegistry(RegistersController registryController, string destinatedRegistry, string potentialValue)
        {
            if (IsValue(potentialValue, out int valueArg))
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

        private void SetValueToRegistry(RegistersController registryController, string destinatedRegName, int value)
        {
            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) };
            registryController.SetBytesToRegistry(destinatedRegName, bytes);
        }


        private void CheckAndReduceOverflow(ref int value, char registryPostfix = '0')
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

        private bool IsValue(string arg, out int value)
        {
            if (!arg.EndsWith("H"))
            {
                if (int.TryParse(arg, out int result))
                {
                    outputLogBuilder.Clear();
                    outputLogBuilder.AppendLine($"Parsing value \"{arg}\" as decimal.");
                    value = result;
                    return true;
                }
            }
            else
            {
                if (int.TryParse(arg.Remove(arg.Length-1, 1), System.Globalization.NumberStyles.HexNumber, null, out int resultFromHex))
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

        private bool TryParseEffectiveAddressToValue(string []addressArgs, out int address)
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
                        byte[] bytes = regController.GetRegistry(arg);
                        address += BitConverter.ToUInt16(bytes);
                    }
                } else if(IsValue(arg, out int value))
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
            outputLogBuilder.AppendLine($"Converting arguments into effective address {address.ToString("X")}h.");
            return true;
        }

        private bool IsIndexRegister(string potentialRegister) => (potentialRegister == "SI" || potentialRegister == "DI");
        
        private bool IsBaseRegister(string potentialRegister) => (potentialRegister == "BX" || potentialRegister == "BP");

        private bool ContainsAddressArgument(string[] args, out string[] addressArgs, out bool isRightOperand)
        {
            int startBracketPos = -1, endBracketPos = -1;
            int startIndex = 1, endIndex = 1;
            isRightOperand = false;

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
                if (startIndex > 1)
                    isRightOperand = true;
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
