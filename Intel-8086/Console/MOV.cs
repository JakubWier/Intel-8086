using System;
using System.Text;
using Intel_8086.Registers;
using Intel_8086.MemorySystem;

namespace Intel_8086.Console
{
    public class MOV : CommandHandler
    {
        public CommandHandler NextHandler { get; set; }

        private StringBuilder outputLogBuilder;
        private RegistersController[] processedRegisters;
        //private string[] processedArgs;

        public string HandleOperation(string[] args, params RegistersController[] registryControllers)
        {
            processedRegisters = registryControllers;

            if (IsCommandMOV(args[0]))
            {
                if (args.Length < 3)
                    return "Too few arguments to function MOV.";

                outputLogBuilder = new StringBuilder();

                if (CommandFormatter.FindAndRemoveArgSeparator(args) == -1)
                    return "MOV arguments must be separated by comma.";

                SelectOperation(args);
                return outputLogBuilder.ToString();
            }

            return Next(args);
        }

        private bool IsCommandMOV(string potentialMovKeyword) => (potentialMovKeyword == "MOV");
        private bool IsIndexRegister(string potentialRegister) => (potentialRegister == "SI" || potentialRegister == "DI");
        private bool IsBaseRegister(string potentialRegister) => (potentialRegister == "BX" || potentialRegister == "BP");

        private string Next(string[] args)
        {
            if (NextHandler != null)
                return NextHandler.HandleOperation(args, processedRegisters);
            else
                return "";
        }

        private void SelectOperation(string[] args)
        {
            if (TryExecuteAddressArgument(args))
                return;

            if (TrySetRegistryToRegistry(args[1], args[2]))
                return;

            if (TrySetValueToRegistry(args[1], args[2]))
                return;
        }

        private bool TryExecuteAddressArgument(string[] args)
        {
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
                outputLogBuilder.ToString();
                return true;
            }
            else if (addressArgs != null)
            {
                outputLogBuilder.Append("Argument is missing bracket.");
                return true;
            }
            return false;
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
                SetValueToRegistry(registryController, destinatedRegistry, value);
                outputLogBuilder.AppendLine($"Value {value.ToString("X")}h assigned to registry {destinatedRegistry} from physical address {physicalAddress.ToString("X")}h.");
            }
        }

        private void TrySetRegistryToMemory(RegistersController registryController, string sourcedRegistry, int effectiveAddress)
        {
            if (IsSupportedRegistryName("DS", out RegistersController segmentsController))
            {
                const int TO_20BIT_SHIFT = 4;
                int dataSegment = BitConverter.ToUInt16(segmentsController.GetRegistry("DS"));
                dataSegment = dataSegment << TO_20BIT_SHIFT;
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

        private bool TrySetValueToRegistry(string destinatedRegistry, string potentialValue)
        {
            if (!IsSupportedRegistryName(destinatedRegistry, out RegistersController destinatedController))
            {
                outputLogBuilder.Clear();
                outputLogBuilder.Append($"{destinatedRegistry} is unsupported registry name.");
                return false;
            }

            if (CommandFormatter.IsValue(potentialValue, out int valueArg, out string parseLog))
            {
                outputLogBuilder.Clear();
                CommandFormatter.CheckAndReduceOverflow(ref valueArg, out string overflowLog, destinatedRegistry[^1]);
                SetValueToRegistry(destinatedController, destinatedRegistry, valueArg);
                string valueHex = valueArg.ToString("X");

                outputLogBuilder.Append(parseLog);
                outputLogBuilder.Append(overflowLog);
                outputLogBuilder.Append($"{(valueHex.Length <= 2 ? valueHex.PadLeft(2, '0') : valueHex.PadLeft(4, '0')) }h moved into {destinatedRegistry}.");
                return true;
            }
            return false;
        }

        private void SetValueToRegistry(RegistersController registryController, string destinatedRegName, int value)
        {
            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) };
            registryController.SetBytesToRegistry(destinatedRegName, bytes);
        }

        private bool TrySetRegistryToRegistry(string destinatedRegistry, string sourcedRegistry)
        {
            if (!IsSupportedRegistryName(destinatedRegistry, out RegistersController destinatedController))
            {
                outputLogBuilder.Clear();
                outputLogBuilder.Append($"{destinatedRegistry} is unsupported registry name.");
                return false;
            }

            if (IsSupportedRegistryName(sourcedRegistry, out RegistersController sourcedController))//"mov reg,reg2"
            {
                SetRegistryToRegistry(destinatedController, sourcedController, destinatedRegistry, sourcedRegistry);
                outputLogBuilder.Append($"{sourcedRegistry} moved into {destinatedRegistry}.");
                return true;
            }
            outputLogBuilder.Append($"{sourcedRegistry} is unsupported registry name.");
            return false;
        }

        private void SetRegistryToRegistry(RegistersController destinatedController, RegistersController sourcedController, string destinatedRegistry, string sourcedRegistry)
        {
            byte[] bytes = sourcedController.GetRegistry(sourcedRegistry);

            if (sourcedRegistry.EndsWith('H') || sourcedRegistry.EndsWith('L'))
                destinatedController.SetBytesToRegistry(destinatedRegistry, bytes[0]);
            else
                destinatedController.SetBytesToRegistry(destinatedRegistry, bytes);
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
                } else if(CommandFormatter.IsValue(arg, out int value, out string parseLog))
                {
                    outputLogBuilder.Append(parseLog);
                    CommandFormatter.CheckAndReduceOverflow(ref value, out string log);
                    outputLogBuilder.Append(log);
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

            if (startBracketPos != -1 && endBracketPos != -1) //AND, OR?
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
            } else if(startBracketPos != -1 || endBracketPos != -1)
            {
                addressArgs = new string[0];
                return false;
            }
            addressArgs = null;
            return false;
        }
    }
}
