using System.Diagnostics;
using static System.Diagnostics.Debug;
using System;
using Intel_8086;

namespace Tests_Intel_8086
{
    class GeneralPurposeRegistryCommandTest
    {
        public void StartAllTests()
        {
            TestSetFixedValueToRegistry();
            TestInvalidCommand();
        }

        public void TestInvalidCommand()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            GeneralRegistryCommand registryCommand = new GeneralRegistryCommand(registersMock, loggerMock);

            registryCommand.InputCommand("");
            Assert(loggerMock.outputResult == "");
            registryCommand.InputCommand(" ");
            Assert(loggerMock.outputResult == "Invalid command line.");
            registryCommand.InputCommand("][/]'/]['");
            Assert(loggerMock.outputResult == "Invalid command line.");
        }
        public void TestSetFixedValueToRegistry()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            GeneralRegistryCommand registryCommand = new GeneralRegistryCommand(registersMock, loggerMock);
            registryCommand.InputCommand("AL 1");
            Assert(registersMock.number == 1 && loggerMock.outputResult == "01 inserted into AL");
            registryCommand.InputCommand("AH F");
            Assert(registersMock.number == 15 && loggerMock.outputResult == "0F inserted into AH");
            registryCommand.InputCommand("AX 123");
            Assert(registersMock.number == 291 && loggerMock.outputResult == "0123 inserted into AX");
        }

        private class GeneralPurposeRegistersMock : IRegistryModel
        {
            public int number;
            public byte[] GetRegistry(RegistryType registryType)
            {
                return null;
            }

            public void SetBytesToRegistry(RegistryType registryType, params byte[] bytes)
            {
                if (bytes.Length == 1)
                    bytes = new byte[] { bytes[0], 0 };
                number = BitConverter.ToInt16(bytes);
            }
        }

        private class LoggerMock : IOutputController
        {
            public string outputResult = "";
            public void ReplaceOutput(string line)
            {
                outputResult = line;
            }
        }
    }
}
