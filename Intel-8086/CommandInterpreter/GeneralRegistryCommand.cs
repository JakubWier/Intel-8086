using Intel_8086.CommandInterpreter;

namespace Intel_8086
{
    class GeneralRegistryCommand : ICommandInterpreter
    {
        private IOutputController output;
        private IProcedureHandling procedureHandling;

        public GeneralRegistryCommand(IRegistryModel registryModel, IOutputController output) {
            MOV mov = new MOV(null, registryModel);
            AssignToRegistry assignTo = new AssignToRegistry(mov, registryModel);
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
