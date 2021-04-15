using System;
using System.Text;
using Intel_8086.Registers;
using Intel_8086.MemorySystem;

namespace Intel_8086.Console
{
    public class XCHG : CommandHandler
    {
        private StringBuilder outputLogBuilder;
        private RegistersController[] processedRegisters;

        public CommandHandler NextHandler { get; set; }

        public string HandleOperation(string[] args, params RegistersController[] registryControllers)
        {
            processedRegisters = registryControllers;

            if (IsCommandXCHG(args[0]))
            {
                if (args.Length < 3)
                    return "Too few arguments to function XCHG.";

                outputLogBuilder = new StringBuilder();

                if (CommandFormatter.FindAndRemoveArgSeparator(args) == -1)
                    return "XCHG arguments must be separated by comma.";

                TryDetermineOperation(args);

                return outputLogBuilder.ToString();
            }

            return Next(args);
        }

        private bool IsCommandXCHG(string potentialXchgKeyword) => potentialXchgKeyword == "XCHG";
        private bool IsIndexRegister(string potentialRegister) => (potentialRegister == "SI" || potentialRegister == "DI");
        private bool IsBaseRegister(string potentialRegister) => (potentialRegister == "BX" || potentialRegister == "BP");

        private string Next(string[] args)
        {
            if (NextHandler != null)
                return NextHandler.HandleOperation(args, processedRegisters);
            else
                return "";
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

        private void TryDetermineOperation(string[] args)
        {
            if (TryExecuteAddressArgument(args))
                return;

            if (TryExchangeRegisters(args[1], args[2]))
                return;
        }

        private bool TryExchangeRegisters(string firstRegistry, string secondRegistry)
        {
            if (!IsSupportedRegistryName(firstRegistry, out RegistersController firstController))
            {
                outputLogBuilder.Append($"{firstRegistry} is unsupported registry name.");
                return false;
            }

            if (!IsSupportedRegistryName(secondRegistry, out RegistersController secondController))
            {
                outputLogBuilder.Append($"{secondRegistry} is unsupported registry name.");
                return false;
            }

            char registryPostfix;

            byte[] firstRegistryBytes = firstController.GetRegistry(firstRegistry);
            byte[] secondRegistryBytes = secondController.GetRegistry(secondRegistry);

            registryPostfix = firstRegistry[firstRegistry.Length - 1];
            int firstValue = RegistryBytesToInt16(firstRegistryBytes, registryPostfix);

            registryPostfix = secondRegistry[secondRegistry.Length - 1];
            int secondValue = RegistryBytesToInt16(secondRegistryBytes, registryPostfix);

            SwapInPlace(ref firstValue, ref secondValue);

            secondController.SetBytesToRegistry(secondRegistry, BitConverter.GetBytes((ushort)secondValue));
            firstController.SetBytesToRegistry(firstRegistry, BitConverter.GetBytes((ushort)firstValue));

            outputLogBuilder.Append($"{firstRegistry} exchanged with {secondRegistry}.");
            return true;
        }

        private void SwapInPlace(ref int firstValue, ref int secondValue)
        {
            firstValue = firstValue + secondValue;
            secondValue = firstValue - secondValue;
            firstValue = firstValue - secondValue;
        }

        private int RegistryBytesToInt16(byte[] registryBytes, char registryName) => registryName switch
        {
            'L' => registryBytes[0],
            'H' => registryBytes[0],
            _ => BitConverter.ToUInt16(registryBytes)
        };

        private bool TryExecuteAddressArgument(string[] args)
        {
            if (ContainsAddressArgument(args, out string[] addressArgs, out bool isRightOperand))
            {
                if (TryParseEffectiveAddressToValue(addressArgs, out int address))
                {
                    Index indexRegName = isRightOperand ? 1 : ^1;
                    if (IsSupportedRegistryName(args[indexRegName], out RegistersController registryContainer))
                    {
                        TryExchangeRegistryMemory(registryContainer, args[indexRegName], address);
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

        private void TryExchangeRegistryMemory(RegistersController registryContainer, string registryName, int effectiveAddress)
        {
            if (IsSupportedRegistryName("DS", out RegistersController segmentsController))
            {
                const int TO_20BIT_SHIFT = 4;
                int dataSegment = BitConverter.ToUInt16(segmentsController.GetRegistry("DS"));
                dataSegment = dataSegment << TO_20BIT_SHIFT;
                uint physicalAddress = (uint)(dataSegment + effectiveAddress);

                MemoryModel memory = MemoryModel.GetInstance();
                int regValue = BitConverter.ToUInt16(registryContainer.GetRegistry(registryName));
                int memValue = BitConverter.ToUInt16(memory.GetMemoryWord(physicalAddress));

                SwapInPlace(ref regValue, ref memValue);

                if (registryName[^1] != 'L' && registryName[^1] != 'H')
                {
                    memory.SetMemoryWord(physicalAddress, BitConverter.GetBytes(memValue));
                    registryContainer.SetBytesToRegistry(registryName, BitConverter.GetBytes(regValue));
                }
                else
                {
                    memory.SetMemoryCell(physicalAddress, BitConverter.GetBytes(memValue)[0]);
                    registryContainer.SetBytesToRegistry(registryName, BitConverter.GetBytes(regValue)[0]);
                }

                outputLogBuilder.AppendLine($"Value {memValue.ToString("X")}h from registry {registryName} exchanged with {regValue.ToString("X")} at physical address {physicalAddress.ToString("X")}h.");
            }
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
            }
            else if (startBracketPos != -1 || endBracketPos != -1)
            {
                addressArgs = new string[0];
                return false;
            }
            addressArgs = null;
            return false;
        }

        private bool TryParseEffectiveAddressToValue(string[] addressArgs, out int address)
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
                }
                else if (CommandFormatter.IsValue(arg, out int value, out string parseLog))
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

    }
}
