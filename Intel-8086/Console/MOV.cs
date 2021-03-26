using System;
using System.Text;
using Intel_8086.Registers;

namespace Intel_8086.Console
{
    class MOV : ProcedureHandler
    {
        Registry[] registries;
        StringBuilder outputLogBuilder;
        public MOV(ProcedureHandler nextHandler, params Registry[] registries)
        {
            NextHandler = nextHandler;
            this.registries = registries;
        }

        public ProcedureHandler NextHandler { get; set; }

        public string HandleOperation(string[] args)
        {
            if (IsCommandMOV(args[0]))
                if (args.Length > 2)
                {
                    outputLogBuilder = new StringBuilder();

                    int argSeparatorPos = args[1].IndexOf(',');
                    if (argSeparatorPos == -1)
                        return "MOV arguments must be separated by comma.";

                    args[1] = args[1].Remove(argSeparatorPos, 1);

                    if (!IsRegistry(args[1], out Registry registry))
                        return $"{args[1]} is unknown registry name.";

                    if (IsValue(args[2], out int value))
                    {
                        SetValueToRegistry(registry, value);
                        return outputLogBuilder.ToString();
                    }

                    //if (TrySetRegistryToRegistry(args[1], args[2]))
                        //return outputLogBuilder.ToString();

                    return outputLogBuilder.ToString();
                }
                else
                {
                    return "Too few arguments to function MOV.";
                }

            if (NextHandler!=null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool TrySetValueToRegistry(Registry destinatedRegistry, string potentialValue)
        {
            if (IsValue(potentialValue, out int valueArg))
            {
                CheckAndReduceOverflow(ref valueArg, destinatedRegistry.Name[destinatedRegistry.Name.Length-1]);
                SetValueToRegistry(destinatedRegistry, valueArg);
                string valueHex = valueArg.ToString("X");
                outputLogBuilder.Append($"{(valueHex.Length <= 2 ? valueHex.PadLeft(2, '0') : valueHex.PadLeft(4, '0')) } moved into {destinatedRegistry}.");
                return true;
            }

            return false;
        }

        private void SetValueToRegistry(Registry registry, int value)
        {
            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) };
            registry.TrySetBytes(bytes: bytes);
        }

        private void CheckAndReduceOverflow(ref int value, char registryPostfix)
        {
            if (registryPostfix == 'L')
            {
                if (value > byte.MaxValue)
                {
                    value = BitConverter.GetBytes(value)[0]; //255;
                    outputLogBuilder.Append("Expected 8bit value.\nData loss due to conversion.\nMoving first byte.\n");
                }
            } else if(registryPostfix == 'H')
            {
                if (value > byte.MaxValue)
                {
                    value = BitConverter.GetBytes(value)[0]; //255;
                    outputLogBuilder.Append("Expected 8bit value.\nData loss due to conversion.\nMoving first byte.\n");
                }
            }
            else
            {
                if (value > ushort.MaxValue)
                {
                    byte[] convert = BitConverter.GetBytes(value);
                    value = convert[1]*256 + convert[0];
                    outputLogBuilder.Append("Expected 16bit value.\nData loss due to conversion.\nMoving first two bytes.\n");
                }
            }
        }

        private bool TrySetRegistryToRegistry(Registry destinatedRegistry, string potentialSourcedRegistry)
        {
            if (IsRegistry(potentialSourcedRegistry, out Registry sourcedRegistry))//"mov reg,reg2"
            {
                //SetRegistryToRegistry(destinatedRegistry, potentialSourcedRegistry);
                outputLogBuilder.Append($"{sourcedRegistry} moved into {destinatedRegistry.Name}.");
                return true;
            }
            outputLogBuilder.Append($"{sourcedRegistry} is unknown registry name.");
            return false;
        }

        private bool IsValue(string arg, out int value)
        {
            if (!arg.EndsWith("H"))
            {
                if (int.TryParse(arg, out int result))
                {
                    outputLogBuilder.Append("Parsing value from decimal.\n");
                    value = result;
                    return true;
                }
            }
            else
            {
                if (int.TryParse(arg.Remove(arg.Length-1, 1), System.Globalization.NumberStyles.HexNumber, null, out int resultFromHex))
                {
                    outputLogBuilder.Append("Parsing value from hexadecimal.\n");
                    value = resultFromHex;
                    return true;
                }
            }
            value = int.MinValue;
            return false;
        }

        private bool IsCommandMOV(string potentialMovKeyword)
        {
            return potentialMovKeyword == "MOV" ? true : false;
        }

        private bool IsRegistry(string potentialRegistryName, out Registry registry)
        {
            if (potentialRegistryName == null || potentialRegistryName.Length == 0)
            {
                registry = null;
                return false;
            }

            foreach (Registry reg in registries)
                if (reg.Equals(potentialRegistryName))
                {
                    registry = reg;
                    return true;
                }

            registry = null;
            return false;
        }
    }
}
