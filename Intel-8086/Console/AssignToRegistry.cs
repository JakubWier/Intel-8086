using System;
using Intel_8086.Registers;

namespace Intel_8086.Console
{
    class AssignToRegistry : RegistryCommandHandler
    {
        public RegistryCommandHandler NextHandler { get; set; }
        private RegistersController[] processedRegisters;

        public string HandleOperation(string[] args, params RegistersController[] registryControllers)
        {
            processedRegisters = registryControllers;

            if (IsCommandSetFixedToRegistry(args[0], out RegistersController controller))
            {
                return TrySetFixedToRegistry(controller, args[0], args[1]);
            }

            if (NextHandler != null)
                return NextHandler.HandleOperation(args, processedRegisters);
            else
                return "";
        }

        private bool IsCommandSetFixedToRegistry(string potentialRegistryName, out RegistersController controller)
        {
            if (potentialRegistryName.Length == 2)
                foreach (RegistersController container in processedRegisters)
                {
                    if (container.Contains(potentialRegistryName))
                    {
                        controller = container;
                        return true;
                    }
                }

            controller = null;
            return false;
        }

        private string TrySetFixedToRegistry(RegistersController controller, string registryName, string valueHex)
        {
            registryName = registryName.ToUpper();
            if (valueHex.Length > 4)
                valueHex = valueHex.Substring(valueHex.Length - 4, 4);

            try
            {
                byte[] bytes = (valueHex.Length <= 2) ? new[] { Convert.ToByte(valueHex, 16) } : BitConverter.GetBytes(Convert.ToInt16(valueHex, 16));
                controller.SetBytesToRegistry(registryName, bytes);
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
