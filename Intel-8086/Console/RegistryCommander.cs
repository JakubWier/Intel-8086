using Intel_8086.Registers;

namespace Intel_8086.Console
{
    class RegistryCommander : CommandInterpreter
    {
        private OutputController output;
        private CommandHandler commandHandler;
        private RegistersController[] supportedRegisters;

        public RegistryCommander(OutputController output, params RegistersController[] supportedRegisters) {
            this.output = output;
            this.supportedRegisters = supportedRegisters;
        }

        /// <summary>
        /// Converts command line into methods and executes them on registry model.
        /// </summary>
        /// <param name="line">Command line.</param>
        /// <returns>Returns method interpreted name.</returns>
        public void InputCommand(string line)
        {
            if (line.Length == 0)
                return;
            line = line.Trim();

            string[] commandBlockBuffer = line.Split(' ');

            for(int i=0; i<commandBlockBuffer.Length;i++)
                commandBlockBuffer[i] = commandBlockBuffer[i].ToUpper();

            string outputResult = commandHandler.HandleOperation(commandBlockBuffer, supportedRegisters);

            if (outputResult?.Length != 0)
                output.ReplaceOutput(outputResult);
            else
                output.ReplaceOutput("Invalid command line.");
        }

        public void AddHandler(CommandHandler handler)
        {
            handler.NextHandler = commandHandler;
            commandHandler = handler;
        }
    }
}
