using System;
using System.Text;
using Intel_8086.Registers;

namespace Intel_8086.Console
{
    class MOV : ProcedureHandler
    {
        RegistryContainer registryModel;
        StringBuilder outputLogBuilder;
        public MOV(ProcedureHandler nextHandler, RegistryContainer registry)
        {
            NextHandler = nextHandler;
            registryModel = registry;
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
                        return "MOV arguments must separated by comma.";
                    args[1] = args[1].Remove(argSeparatorPos, 1);

                    if (!IsRegistryName(args[1]))
                        return $"{args[1]} is unknown registry name.";

                    if (TrySetValueToRegistry(args[1], args[2]))
                        return outputLogBuilder.ToString();

                    if (TrySetRegistryToRegistry(args[1], args[2]))
                        return outputLogBuilder.ToString();

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

        private bool TrySetValueToRegistry(string destinatedRegistry, string potentialValue)
        {
            if (IsValue(potentialValue, out Int64 valueArg))
            {
                char regPostfix = destinatedRegistry[destinatedRegistry.Length - 1];
                CheckAndReduceOverflow(ref valueArg, regPostfix);
                SetValueToRegistry(destinatedRegistry, valueArg);
                string valueHex = valueArg.ToString("X");
                outputLogBuilder.Append($"{(valueHex.Length <= 2 ? valueHex.PadLeft(2, '0') : valueHex.PadLeft(4, '0')) } moved into {destinatedRegistry}.");
                return true;
            }

            return false;
        }

        private void SetValueToRegistry(string destinatedRegName, Int64 value)
        {
            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) };
            GeneralPurposeRegistryType registryType = (GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), destinatedRegName);
            registryModel.SetBytesToRegistry(registryType, bytes);
        }


        private void CheckAndReduceOverflow(ref Int64 value, char registryPostfix)
        {
            if (registryPostfix == 'L' || registryPostfix == 'H')
            {
                if (value > byte.MaxValue)
                {
                    value = BitConverter.GetBytes(value)[0]; //255;
                    outputLogBuilder.Append("Expected 8bit value.\nData loss due to conversion.\n");
                }
            }
            else
            {
                if (value > ushort.MaxValue)
                {
                    byte[] convert = BitConverter.GetBytes(value);
                    value = convert[1]*256 + convert[0];
                    outputLogBuilder.Append("Expected 16bit value.\nData loss due to conversion.\n");
                }
            }
        }

        private bool TrySetRegistryToRegistry(string destinatedRegistry, string potentialSourcedRegistry)
        {
            if (IsRegistryName(potentialSourcedRegistry))//"mov reg,reg2"
            {
                SetRegistryToRegistry(destinatedRegistry, potentialSourcedRegistry);
                outputLogBuilder.Append($"{potentialSourcedRegistry} moved into {destinatedRegistry}.");
                return true;
            }
            outputLogBuilder.Append($"{potentialSourcedRegistry} is unknown registry name.");
            return false;
        }

        private void SetRegistryToRegistry(string destinatedRegistry, string sourcedRegistry)
        {
            GeneralPurposeRegistryType destinatedRegistryType = (GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), destinatedRegistry);
            GeneralPurposeRegistryType sourcedRegistryType = (GeneralPurposeRegistryType)Enum.Parse(typeof(GeneralPurposeRegistryType), sourcedRegistry);
            byte[] bytes = registryModel.GetRegistry(sourcedRegistryType);

            if (sourcedRegistry.EndsWith('H'))
                registryModel.SetBytesToRegistry(destinatedRegistryType, bytes[1]);
            else if (sourcedRegistry.EndsWith('L'))
                registryModel.SetBytesToRegistry(destinatedRegistryType, bytes[0]);
            else
                registryModel.SetBytesToRegistry(destinatedRegistryType, bytes);
        }

        private bool IsValue(string arg, out Int64 value)
        {
            if (!arg.StartsWith("0X"))
            {
                if (Int64.TryParse(arg, out Int64 result))
                {
                    outputLogBuilder.Append("Parsing value from decimal.\n");
                    value = result;
                    return true;
                }
            }
            else
            {
                if (Int64.TryParse(arg.Remove(0, 2), System.Globalization.NumberStyles.HexNumber, null, out Int64 resultFromHex))
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
    }
}
