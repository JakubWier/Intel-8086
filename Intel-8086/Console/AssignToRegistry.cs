using System;
using Intel_8086.Registers;
using System.Text;

namespace Intel_8086.Console
{
    class AssignToRegistry : ProcedureHandler
    {
        StringBuilder outputLog;
        public ProcedureHandler NextHandler { get; set; }
        Registry[] registries;
        public AssignToRegistry(ProcedureHandler next, params Registry[] supportedRegistries)
        {
            registries = supportedRegistries;
            NextHandler = next;
        }

        public string HandleOperation(string[] args)
        {
            if (IsRegistryExists(args[0], out Registry registry))
            {
                outputLog = new StringBuilder(); //369
                if (TryParseValue(args[1], out int value))
                    return SetFixedToRegistry(registry, (ushort)value);
            }

            if (NextHandler != null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool IsRegistryExists(string potentialRegistryName, out Registry registry)
        {
            registry = null;

            if (potentialRegistryName?.Length == 0)
                return false;

            foreach (Registry reg in registries)
                if (reg.IsRegistry(potentialRegistryName, out Registry foundRegistry))
                {
                    registry = foundRegistry;
                    return true;
                }

            return false;
        }

        private bool TryParseValue(string valueHex, out int value)
        {
            CutIfOverflow(ref valueHex);

            if (int.TryParse(valueHex, System.Globalization.NumberStyles.HexNumber, null, out int result))
            {
                value = result;
                return true;
            }

            outputLog.Append($"Cannot parse \"{valueHex}\" as hexadecimal.");
            value = int.MaxValue;

            return false;
        }

        private string SetFixedToRegistry(Registry registry, ushort value)
        {

            byte[] bytes = BitConverter.GetBytes(value);
            registry.TrySetBytes(bytes: bytes);
            string valueHex = value.ToString("X");
            outputLog.AppendLine($"{ (valueHex.Length > 2 ? valueHex.PadLeft(4, '0') : valueHex.PadLeft(2, '0')).ToUpper()}h assigned into {registry.Name}.");

            return outputLog.ToString();
        }

        private void CutIfOverflow(ref string valueHex)
        {
            if (valueHex.Length > 4)
            {
                outputLog.AppendLine($"Value {valueHex}h is too big and will cause overflow.");
                valueHex = valueHex.Substring(valueHex.Length - 4, 4);
                outputLog.AppendLine($"Data loss due to conversion to 16bit type.");
            }
        }
    }
}
