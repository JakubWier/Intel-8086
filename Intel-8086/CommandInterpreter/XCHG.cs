using System;
using System.Text;

namespace Intel_8086.CommandInterpreter
{
    class XCHG : IProcedureHandling
    {
        IRegistryModel registryModel;
        StringBuilder outputLogBuilder;
        public XCHG(IProcedureHandling nextHandler, IRegistryModel registry)
        {
            NextHandler = nextHandler;
            registryModel = registry;
        }

        public IProcedureHandling NextHandler { get; set; }

        public string HandleOperation(string[] args)
        {
            if (args.Length > 1)
                if (IsCommandXCHG(args[0]))
                {
                    outputLogBuilder = new StringBuilder();
                    args[1] = args[1].Replace(',', ' ');
                    args[1] = args[1].Trim();
                    string[] xchgArguments = args[1].Split(' ');
                    if (IsRegistryName(xchgArguments[0]))
                    {
                        if (TryExchangeRegisters(xchgArguments, args))
                            return outputLogBuilder.ToString();
                        return "Invalid XCHG command arguments.";
                    }
                    return $"{xchgArguments[0]} is unknown registry name.";
                }

            if (NextHandler != null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool IsCommandXCHG(string potentialXchgKeyword)
        {
            return potentialXchgKeyword == "XCHG" ? true : false;
        }

        private bool IsRegistryName(string potentialRegistryName)
        {
            if (potentialRegistryName.Length == 2)
                foreach (string reg in Enum.GetNames(typeof(RegistryType)))
                {
                    if (potentialRegistryName == reg)
                        return true;
                }
            return false;
        }

        private bool TryExchangeRegisters(string[] movArguments, string[] commandBuffer)
        {
            for (int i = 1; i < movArguments.Length; i++)
            {
                if (IsRegistryName(movArguments[i]))//"xchg reg,reg2"
                {
                    string firstRegistry = movArguments[0];
                    string secondRegistry = movArguments[1];
                    ExchangeRegisters(firstRegistry, secondRegistry);
                    outputLogBuilder.Append($"{firstRegistry} exchanged with {secondRegistry}.");
                    return true;
                }
            }
            for (int i = 2; i < commandBuffer.Length; i++)
            {
                if (IsRegistryName(commandBuffer[i]))//"xchg reg, reg2"
                {
                    string firstRegistry = movArguments[0];
                    string secondRegistry = commandBuffer[i];
                    ExchangeRegisters(firstRegistry, secondRegistry);
                    outputLogBuilder.Append($"{firstRegistry} exchanged with {secondRegistry}."); 
                    return true;
                }
            }
            return false;
        }

        private void ExchangeRegisters(string firstRegistry, string secondRegistry)
        {
            RegistryType firstRegistryType = (RegistryType)Enum.Parse(typeof(RegistryType), firstRegistry);
            RegistryType secondRegistryType = (RegistryType)Enum.Parse(typeof(RegistryType), secondRegistry);
            char registryPostfix;

            byte[] firstRegistryBytes = registryModel.GetRegistry(firstRegistryType);
            byte[] secondRegistryBytes = registryModel.GetRegistry(secondRegistryType);

            registryPostfix = firstRegistry[firstRegistry.Length - 1];
            int firstValue = RegistryBytesToInt16(firstRegistryBytes, registryPostfix);

            registryPostfix = secondRegistry[secondRegistry.Length - 1];
            int secondValue = RegistryBytesToInt16(secondRegistryBytes, registryPostfix);

            SwapInPlace(ref firstValue, ref secondValue);

            registryModel.SetBytesToRegistry(firstRegistryType, BitConverter.GetBytes((ushort)firstValue));
            registryModel.SetBytesToRegistry(secondRegistryType, BitConverter.GetBytes((ushort)secondValue));
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
