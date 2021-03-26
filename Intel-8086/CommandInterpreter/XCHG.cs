using System;
using System.Text;
using Intel_8086.Registers;

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
                foreach (string reg in Enum.GetNames(typeof(GeneralPurposeRegistryType)))
                {
                    if (potentialRegistryName == reg)
                        return true;
                }

            return false;
        }

        private bool TryExchangeRegisters(string firstRegistry, string secondRegistry)
        {
            if (!IsRegistryName(firstRegistry))
            {
                outputLogBuilder.Append($"{firstRegistry} is unknown registry name.");
                return false;
            }

            if (!IsRegistryName(secondRegistry))
            {
                outputLogBuilder.Append($"{secondRegistry} is unknown registry name.");
                return false;
            }

            ExchangeRegisters(firstRegistry, secondRegistry);
            outputLogBuilder.Append($"{firstRegistry} exchanged with {secondRegistry}.");
            return true;
        }

        private void ExchangeRegisters(string firstRegistry, string secondRegistry)
        {
            GeneralPurposeRegistryType firstRegistryType = (GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), firstRegistry);
            GeneralPurposeRegistryType secondRegistryType = (GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), secondRegistry);
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
