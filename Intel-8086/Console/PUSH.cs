using System;
using Intel_8086.Registers;
using System.Text;
using Intel_8086.MemorySystem;

namespace Intel_8086.Console
{
    public class PUSH : CommandHandler
    {
        public CommandHandler NextHandler { get; set; }
        private RegistersController[] supportedRegistries;
        private StringBuilder outputLogBuilder;

        public string HandleOperation(string[] args, params RegistersController[] supportedRegistries)
        {
            this.supportedRegistries = supportedRegistries;

            if (IsCommandPush(args[0]))
            {
                if (args.Length < 2)
                    return "Pushing registry on stack requires two arguments.";
                outputLogBuilder = new StringBuilder();
                PushOnStack(args[1]);
                return outputLogBuilder.ToString();
            }

            if (NextHandler != null)
                return NextHandler.HandleOperation(args, supportedRegistries);
            else
                return "";
        }

        private bool IsCommandPush(string commandName) => commandName == "PUSH";

        private bool TryGetRegistry(string potentialRegistryName, out RegistersController controller)
        {
            if (potentialRegistryName.Length == 2)
                foreach (RegistersController container in supportedRegistries)
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

        private void PushOnStack(string registryName)
        {
            const int TO_20BIT_SHIFT = 4;

            if (!TryGetRegistry(registryName, out RegistersController sourceContainer))
            {
                outputLogBuilder.AppendLine($"{registryName} is unsupported registry name.");
                return;
            }

            if (!TryGetRegistry("SP", out RegistersController stackPointerContainer))
            {
                outputLogBuilder.AppendLine("Command interpreter couldn't find SP registry.");
                return;
            }

            if(!TryGetRegistry("SS", out RegistersController stackSegmentContainer))
            {
                outputLogBuilder.AppendLine("Command interpreter couldn't find SS registry.");
                return;
            }

            MemoryModel memory = MemoryModel.GetInstance();
            UInt16 stackSegment = BitConverter.ToUInt16(stackSegmentContainer.GetRegistry("SS"));
            UInt16 stackPointer = BitConverter.ToUInt16(stackPointerContainer.GetRegistry("SP"));
            uint physicalAddress = (uint)(stackSegment << TO_20BIT_SHIFT) + stackPointer;
            byte[] wordToPush = sourceContainer.GetRegistry(registryName);
            memory.SetMemoryWord(physicalAddress, wordToPush);

            stackPointer += 2;
            stackPointerContainer.SetBytesToRegistry("SP", BitConverter.GetBytes(stackPointer));

            outputLogBuilder.AppendLine($"Value {BitConverter.ToUInt16(wordToPush).ToString("X")}h from registry {registryName} pushed on stack with physical address {physicalAddress.ToString("X")}h.");
        }
    }
}
