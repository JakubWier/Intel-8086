using System;
using System.Text;
using Intel_8086.Registers;

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

                    int argSeparatorPos = args[1].IndexOf(',');
                    if (argSeparatorPos == -1)
                        return "MOV arguments must separated by comma.";
                    args[1] = args[1].Remove(argSeparatorPos, 1);

                    if (!IsRegistryName(args[1], out RegistersController destinatedController))
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

        private bool TrySetValueToRegistry(RegistersController registryController, string destinatedRegistry, string potentialValue)
        {
            if (IsValue(potentialValue, out Int64 valueArg))
            {
                char regPostfix = destinatedRegistry[destinatedRegistry.Length - 1];
                CheckAndReduceOverflow(ref valueArg, regPostfix);
                SetValueToRegistry(registryController, destinatedRegistry, valueArg);
                string valueHex = valueArg.ToString("X");
                outputLogBuilder.Append($"{(valueHex.Length <= 2 ? valueHex.PadLeft(2, '0') : valueHex.PadLeft(4, '0')) } moved into {destinatedRegistry}.");
                return true;
            }
            return false;
        }

        private void SetValueToRegistry(RegistersController registryController, string destinatedRegName, Int64 value)
        {
            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) };
            registryController.SetBytesToRegistry(destinatedRegName, bytes);
        }


        private void CheckAndReduceOverflow(ref Int64 value, char registryPostfix)
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
            if (IsRegistryName(potentialSourcedRegistry, out RegistersController sourcedController))//"mov reg,reg2"
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

        private bool IsValue(string arg, out Int64 value)
        {
            if (!arg.EndsWith("H"))
            {
                if (Int64.TryParse(arg, out Int64 result))
                {
                    outputLogBuilder.Clear();
                    outputLogBuilder.Append("Parsing value from decimal.\n");
                    value = result;
                    return true;
                }
            }
            else
            {
                if (Int64.TryParse(arg.Remove(arg.Length-1, 1), System.Globalization.NumberStyles.HexNumber, null, out Int64 resultFromHex))
                {
                    outputLogBuilder.Clear();
                    outputLogBuilder.Append("Parsing value from hexadecimal.\n");
                    value = resultFromHex;
                    return true;
                }
            }

            value = int.MinValue;
            return false;
        }

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
    }
}
