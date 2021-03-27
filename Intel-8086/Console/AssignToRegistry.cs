using System;
using Intel_8086.Registers;

namespace Intel_8086.Console
{
    class AssignToRegistry : ProcedureHandler
    {
        public ProcedureHandler NextHandler { get; set; }
        RegistryContainer registryModel;
        public AssignToRegistry(ProcedureHandler next, RegistryContainer registry)
        {
            registryModel = registry;
            NextHandler = next;
        }

        public string HandleOperation(string[] args)
        {
            if (IsCommandSetFixedToRegistry(args[0]))
            {
                return TryParseSetFixedToRegistry(args[0], args[1]);
            }

            if (NextHandler != null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool IsCommandSetFixedToRegistry(string potentialRegistryName)
        {
            potentialRegistryName = potentialRegistryName.ToLower();
            if (potentialRegistryName.Length == 2)
                foreach (string reg in Enum.GetNames(typeof(GeneralPurposeRegistryType)))
                {
                    if (potentialRegistryName == reg.ToLower())
                        return true;
                }
            return false;
        }

        private string TryParseSetFixedToRegistry(string registryName, string valueHex)
        {
            registryName = registryName.ToUpper();
            if (valueHex.Length > 4)
                valueHex = valueHex.Substring(valueHex.Length - 4, 4);
            try
            {
                byte[] bytes = (valueHex.Length <= 2) ? new[] { Convert.ToByte(valueHex, 16) } : BitConverter.GetBytes(Convert.ToInt16(valueHex, 16));
                registryModel.SetBytesToRegistry((GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), registryName), bytes);
                return $"{ (valueHex.Length > 2 ? valueHex.PadLeft(4, '0') : valueHex.PadLeft(2, '0')).ToUpper()} assigned into {registryName}.";
            }
            catch (FormatException)
            {
                return $"Cannot parse \"{valueHex}\" as hexadecimal.";
            }
            catch (ArgumentException arg)
            {
                return arg.Message;
            }


        }
    }
}
