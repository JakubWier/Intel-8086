using Intel_8086.Registers;

namespace Intel_8086.Console
{
    class RegistersCommander : CommandInterpreter
    {
        private OutputController output;
        private ProcedureHandler procedureHandling;

        public RegistersCommander(OutputController output, params Registry[] registries) {
            XCHG xchg = new XCHG(null, registries);
            MOV mov = new MOV(xchg, registries);
            AssignToRegistry assignTo = new AssignToRegistry(mov, registries);
            procedureHandling = assignTo;
            this.output = output;
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

            string outputResult = procedureHandling.HandleOperation(commandBlockBuffer);

            if (outputResult?.Length != 0)
                output.ReplaceOutput(outputResult);
            else
                output.ReplaceOutput("Invalid command line.");
        }
    }
}
