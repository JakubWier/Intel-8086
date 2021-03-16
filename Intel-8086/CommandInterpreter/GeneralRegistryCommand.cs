using Intel_8086.CommandInterpreter;

namespace Intel_8086
{
    class GeneralRegistryCommand : ICommandInterpreter
    {
        private IOutputController output;
        private IProcedureHandling procedureHandling;

        public GeneralRegistryCommand(IRegistryModel registryModel, IOutputController output) {
            procedureHandling = new AssignToRegistry(null, registryModel);
            this.output = output;
        }

        /// <summary>
        /// Converts command line into methods and executes them on registry model.
        /// </summary>
        /// <param name="line">Command line.</param>
        /// <returns>Returns method interpreted name.</returns>
        public void InputCommand(string line)
        {
            string[] commandBlockBuffer = line.Split(' ');
            if (line.Length == 0)
                return;

            string outputResult = procedureHandling.HandleOperation(commandBlockBuffer);
            if (outputResult?.Length != 0)
                output.ReplaceOutput(outputResult);
            else
                output.ReplaceOutput("Invalid command line.");
        }
    }
}
