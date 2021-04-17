using System;
using Intel_8086.Registers;
using System.Text;
using Intel_8086.MemorySystem;

namespace Intel_8086.Console
{
    public class POP : CommandHandler
    {
        public CommandHandler NextHandler { get; set; }
        private RegistersController[] supportedRegistries;
        private StringBuilder outputLogBuilder;

        public POP(params RegistersController[] supportedRegistries) {
            this.supportedRegistries = supportedRegistries;
        }

        public string HandleOperation(string[] args)
        {
            if (IsCommandPush(args[0]))
            {
                if (args.Length < 2)
                    return "Popping to registry from stack requires two arguments.";
                outputLogBuilder = new StringBuilder();

                PopFromStack(args[1]);

                return outputLogBuilder.ToString();
            }

            if (NextHandler != null)
                return NextHandler.HandleOperation(args);
            else
                return "";
        }

        private bool IsCommandPush(string commandName) => commandName == "POP";

        private bool TryGetRegistry(string potentialRegistryName, out RegistersController controller)
        {
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

        private void PopFromStack(string destinatedRegistryName)
        {
            const int TO_20BIT_SHIFT = 4;

            if (destinatedRegistryName.EndsWith('H') || destinatedRegistryName.EndsWith('L'))
            {
                outputLogBuilder.AppendLine($"Cannot use command POP for half registers.");
                return;
            }

            if (!TryGetRegistry(destinatedRegistryName, out RegistersController destinatedContainer))
            {
                outputLogBuilder.AppendLine($"{destinatedRegistryName} is unsupported registry name.");
                return;
            }

            if (!TryGetRegistry("SP", out RegistersController stackPointerContainer))
            {
                outputLogBuilder.AppendLine("Command interpreter couldn't find SP registry.");
                return;
            }

            if (!TryGetRegistry("SS", out RegistersController stackSegmentContainer))
            {
                outputLogBuilder.AppendLine("Command interpreter couldn't find SS registry.");
                return;
            }

            MemoryModel memory = MemoryModel.GetInstance();
            UInt16 stackSegment = BitConverter.ToUInt16(stackSegmentContainer.GetRegistry("SS"));
            Int16 stackPointer = BitConverter.ToInt16(stackPointerContainer.GetRegistry("SP"));
            stackPointer -= 2;

            if(stackPointer < 0)
            {
                outputLogBuilder.AppendLine($"The stack is empty, cannot perform operation.");
                return;
            }

            stackPointerContainer.SetBytesToRegistry("SP", BitConverter.GetBytes(stackPointer));
            uint physicalAddress = (uint)(stackSegment << TO_20BIT_SHIFT) + (UInt16)stackPointer;
            byte[] wordToPop = memory.GetMemoryWord(physicalAddress);
            destinatedContainer.SetBytesToRegistry(destinatedRegistryName, wordToPop);
            //memory.SetMemoryWord(physicalAddress, 0);

            outputLogBuilder.AppendLine($"Value {BitConverter.ToUInt16(wordToPop).ToString("X")}h from physical address {physicalAddress.ToString("X")}h popped from stack to registry {destinatedRegistryName}.");
        }
    }
}
