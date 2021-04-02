using System;
using System.Text;
using Intel_8086.Registers;

namespace Intel_8086.Console
{
    class XCHG : CommandHandler
    {
        private StringBuilder outputLogBuilder;
        private RegistersController[] processedRegisters;

        public CommandHandler NextHandler { get; set; }

        public string HandleOperation(string[] args, params RegistersController[] registryControllers)
        {
            processedRegisters = registryControllers;

            if (IsCommandXCHG(args[0]))
                if (args.Length > 2)
                {
                    outputLogBuilder = new StringBuilder();

                    int argSeparatorPos = args[1].IndexOf(',');
                    if (argSeparatorPos == -1)
                        return "XCHG arguments must separated by comma.";
                    args[1] = args[1].Remove(argSeparatorPos, 1);

                    TryExchangeRegisters(args[1], args[2]);

                    return outputLogBuilder.ToString();
                }
                else
                {
                    return "Too few arguments to function XCHG.";
                }

            if (NextHandler != null)
                return NextHandler.HandleOperation(args, processedRegisters);
            else
                return "";
        }

        private bool IsCommandXCHG(string potentialXchgKeyword) => potentialXchgKeyword == "XCHG";

        private bool IsRegistryName(string potentialRegistryName, out RegistersController registryController)
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

        private bool TryExchangeRegisters(string firstRegistry, string secondRegistry)
        {
            if (!IsRegistryName(firstRegistry, out RegistersController firstController))
            {
                outputLogBuilder.Append($"{firstRegistry} is unknown registry name.");
                return false;
            }

            if (!IsRegistryName(secondRegistry, out RegistersController secondController))
            {
                outputLogBuilder.Append($"{secondRegistry} is unknown registry name.");
                return false;
            }

            ExchangeRegisters(firstController, secondController, firstRegistry, secondRegistry);
            outputLogBuilder.Append($"{firstRegistry} exchanged with {secondRegistry}.");
            return true;
        }

        private void ExchangeRegisters(RegistersController firstController, RegistersController secondController, string firstRegistry, string secondRegistry)
        {
            char registryPostfix;

            byte[] firstRegistryBytes = firstController.GetRegistry(firstRegistry);
            byte[] secondRegistryBytes = secondController.GetRegistry(secondRegistry);

            registryPostfix = firstRegistry[firstRegistry.Length - 1];
            int firstValue = RegistryBytesToInt16(firstRegistryBytes, registryPostfix);

            registryPostfix = secondRegistry[secondRegistry.Length - 1];
            int secondValue = RegistryBytesToInt16(secondRegistryBytes, registryPostfix);

            SwapInPlace(ref firstValue, ref secondValue);

            secondController.SetBytesToRegistry(firstRegistry, BitConverter.GetBytes((ushort)firstValue));
            firstController.SetBytesToRegistry(secondRegistry, BitConverter.GetBytes((ushort)secondValue));
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
            'H' => registryBytes[1],
            _ => BitConverter.ToUInt16(registryBytes)
        };

    }
}
