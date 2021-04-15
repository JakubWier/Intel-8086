using Intel_8086.Registers;

namespace Intel_8086.Console
{
    public class RegistryCommander : CommandInterpreter
    {
        private OutputController output;
        private CommandHandler commandHandler;

        public RegistryCommander(OutputController output) {
            this.output = output;
            commandHandler = new NULL();
        }

        /// <summary>
        /// Converts command line into methods and executes them on registry model.
        /// </summary>
        /// <param name="line">Command line.</param>
        /// <returns>Returns method interpreted name.</returns>
        public void InputCommand(string line)
        {
            line = line.Trim();
            if (line.Length == 0)
                return;

            string[] commandBlockBuffer = line.Split(' ');

            for(int i=0; i<commandBlockBuffer.Length;i++)
                commandBlockBuffer[i] = commandBlockBuffer[i].ToUpper();

            string outputResult = commandHandler.HandleOperation(commandBlockBuffer);

            output.ReplaceOutput(outputResult);
        }

        public void AddHandler(CommandHandler handler)
        {
            handler.NextHandler = commandHandler;
            commandHandler = handler;
        }
    }
}
