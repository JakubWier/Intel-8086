using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086.CommandInterpreter
{
    class MOV : IProcedureHandling
    {
        IRegistryModel registryModel;
        public MOV(IProcedureHandling nextHandler, IRegistryModel registry)
        {
            NextHandler = nextHandler;
            registryModel = registry;
        }

        public IProcedureHandling NextHandler { get; set; }

        public string HandleOperation(string[] args)
        {
            if (args.Length > 1)
                if (IsPrefixCommandMOV(args[0]))
                {
                    string[] movArguments = args[1].Split(',');
                    if (IsRegistryName(movArguments[0]))
                    {
                        if (IsValue(movArguments[1], out int value)) //mov reg,value
                        {
                            byte[] bytes = value > 255 ? BitConverter.GetBytes(Convert.ToUInt16(value)) : new[] { Convert.ToByte(value) } ;
                            string registryDestination = movArguments[0].ToUpper();
                            RegistryType registryType = (RegistryType)Enum.Parse(typeof(RegistryType), registryDestination);
                            registryModel.SetBytesToRegistry(registryType, bytes);
                            string valueHex = value.ToString("X");
                            return $"{(valueHex.Length <= 2 ? valueHex.PadLeft(2,'0') : valueHex.PadLeft(4,'0')) } moved into {registryDestination}.";
                        } 
                        else if (IsRegistryName(movArguments[1]))//mov reg,reg2
                        {
                            string registryDestination = movArguments[0].ToUpper();
                            string registrySource = movArguments[1].ToUpper();
                            RegistryType registryDestinationType = (RegistryType)Enum.Parse(typeof(RegistryType), registryDestination);
                            RegistryType registrySourceType = (RegistryType)Enum.Parse(typeof(RegistryType), registrySource);
                            byte[] bytes = registryModel.GetRegistry(registrySourceType);
                            if(registrySource.EndsWith('H'))
                                registryModel.SetBytesToRegistry(registryDestinationType, bytes[1]);
                            else if(registrySource.EndsWith('L'))
                                registryModel.SetBytesToRegistry(registryDestinationType, bytes[0]);
                            else
                                registryModel.SetBytesToRegistry(registryDestinationType, bytes);
                            return $"{registrySource} moved into {registryDestination}.";
                        }
                    }
                    return "Invalid MOV command";
                }

            if(NextHandler!=null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool IsValue(string arg, out int value)
        {
            if(int.TryParse(arg, out int result))
            {
                value = result;
                return true;
            }
            value = int.MinValue;
            return false;
        }

        private bool IsPrefixCommandMOV(string potentialMovKeyword)
        {
            if (potentialMovKeyword.ToLower() == "mov")
                return true;
            return false;
        }

        private bool IsRegistryName(string potentialRegistryName)
        {
            potentialRegistryName = potentialRegistryName.ToLower();
            if (potentialRegistryName.Length == 2)
                foreach (string reg in Enum.GetNames(typeof(RegistryType)))
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
                registryModel.SetBytesToRegistry((RegistryType)Enum.Parse(typeof(RegistryType), registryName), bytes);
                return $"{ (valueHex.Length > 2 ? valueHex.PadLeft(4, '0') : valueHex.PadLeft(2, '0'))} assigned into {registryName}.";
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
