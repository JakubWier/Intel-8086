using System;
using Intel_8086.Registers;
using System.Text;

namespace Intel_8086.Console
{
    class AssignToRegistry : ProcedureHandler
    {
        StringBuilder outputLog;
        public ProcedureHandler NextHandler { get; set; }
        RegistryOperator[] registryContainers;
        public AssignToRegistry(ProcedureHandler next, params RegistryOperator[] registers)
        {
            registryContainers = registers;
            NextHandler = next;
        }

        public string HandleOperation(string[] args)
        {
            if (IsCommandSetFixedToGeneralRegistry(args[0], out RegistryOperator registryContainer))
            {
                outputLog = new StringBuilder();
                return TrySetFixedToGeneralRegistry(registryContainer, args[0], args[1]);
            }

            if (NextHandler != null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool IsCommandSetFixedToGeneralRegistry(string potentialRegistryName, out RegistryOperator registryContainer)
        {
            registryContainer = null;

            if (potentialRegistryName.Length != 2)
                return false;

            foreach(RegistryOperator container in registryContainers)
                if (container.Contains(potentialRegistryName))
                {
                    registryContainer = container;
                    return true;
                }

            return false;
        }

        private string TrySetFixedToGeneralRegistry(RegistryOperator registryContainer, string registryName, string valueHex)
        {
            CutIfOverflow(ref valueHex);

            try
            {
                byte[] bytes = (valueHex.Length <= 2) ? new[] { Convert.ToByte(valueHex, 16) } : BitConverter.GetBytes(Convert.ToInt16(valueHex, 16));
                registryContainer.TrySetBytesToRegistry(registryName, bytes);
                outputLog.AppendLine($"{ (valueHex.Length > 2 ? valueHex.PadLeft(4, '0') : valueHex.PadLeft(2, '0')).ToUpper()}H assigned into {registryName}.");
                return outputLog.ToString();
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

        private void CutIfOverflow(ref string valueHex)
        {
            if (valueHex.Length > 4)
            {
                outputLog.AppendLine($"Value {valueHex}H is too big and will cause overflow.");
                valueHex = valueHex.Substring(valueHex.Length - 4, 4);
                outputLog.AppendLine($"Data loss due to conversion to 16bit type.");
            }
        }
    }
}
