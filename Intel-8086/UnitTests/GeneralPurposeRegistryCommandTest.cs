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
            TestAssignToRegistry();
            TestMOV();
            TestXCHG();
            TestInvalidCommands();
        }
        public void TestAssignToRegistry()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            GeneralRegistryCommand registryCommand = new GeneralRegistryCommand(registersMock, loggerMock);

            registryCommand.InputCommand("AL 1");
            Assert(registersMock.number == 1 && loggerMock.outputResult == "01 assigned into AL.");

            registryCommand.InputCommand("AH F");
            Assert(registersMock.number == 15 && loggerMock.outputResult == "0F assigned into AH.");

            registryCommand.InputCommand("AX 123");
            Assert(registersMock.number == 291 && loggerMock.outputResult == "0123 assigned into AX.");
        }

        public void TestXCHG()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            GeneralRegistryCommand registryCommand = new GeneralRegistryCommand(registersMock, loggerMock);

            registersMock.SetBytesToRegistry(RegistryType.AX, BitConverter.GetBytes(Convert.ToUInt16(256)));
            registersMock.SetBytesToRegistry(RegistryType.BX, BitConverter.GetBytes(Convert.ToUInt16(255)));
            registryCommand.InputCommand("XchG aX,Bx");
            Assert(registersMock.ax == 255 && registersMock.bx == 256 && loggerMock.outputResult == "AX exchanged with BX.");

            registryCommand.InputCommand("xchG al,cl");
            Assert(registersMock.GetRegistry(RegistryType.AX)[0] == 0 && registersMock.GetRegistry(RegistryType.CX)[0] == 255 && loggerMock.outputResult == "AL exchanged with CL.");

            registersMock.SetBytesToRegistry(RegistryType.DX, BitConverter.GetBytes(Convert.ToUInt16(65535)));
            registryCommand.InputCommand("XchG bh,dh");
            Assert(registersMock.GetRegistry(RegistryType.BX)[1] == 255 && registersMock.GetRegistry(RegistryType.DX)[1] == 1 && loggerMock.outputResult == "BH exchanged with DH.");

            registryCommand.InputCommand("xchg cx, dx");
            byte[] cx = registersMock.GetRegistry(RegistryType.CX);
            byte[] dx = registersMock.GetRegistry(RegistryType.DX);
            Assert(cx[1] == 1 && cx[0] == 255 && dx[1] == 1 && dx[0] == 0 && loggerMock.outputResult == "CX exchanged with DX.");

            registryCommand.InputCommand("xchG dl, ch");
            Assert(registersMock.GetRegistry(RegistryType.DX)[0] == 1 && registersMock.GetRegistry(RegistryType.CH)[1] == 0 && loggerMock.outputResult == "DL exchanged with CH.");

        }

        public void TestMOV()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            GeneralRegistryCommand registryCommand = new GeneralRegistryCommand(registersMock, loggerMock);

            registryCommand.InputCommand("MoV AL,16");
            Assert(registersMock.number == 16 && loggerMock.outputResult == "10 moved into AL.");

            registryCommand.InputCommand("MOV Ch,255");
            Assert(registersMock.number == 255 && loggerMock.outputResult == "FF moved into CH.");

            registryCommand.InputCommand("MOV bx,65535");
            Assert(registersMock.number == 65535 && loggerMock.outputResult == "FFFF moved into BX.");

            registryCommand.InputCommand("MOV bx, 65536");
            Assert(registersMock.number == 65535 && loggerMock.outputResult == "Input value was too big.\nAssigned max value.\nFFFF moved into BX.");

            registryCommand.InputCommand("mOV Bx,AX");
            Assert(registersMock.number == 16 && loggerMock.outputResult == "AX moved into BX.");

            registryCommand.InputCommand("MOV bL,cL");
            Assert(registersMock.number == 255 && loggerMock.outputResult == "CL moved into BL.");

            registryCommand.InputCommand("mov DH, 1");
            Assert(registersMock.number == 1 && loggerMock.outputResult == "01 moved into DH.");
            registryCommand.InputCommand("mov AH, DL");
            Assert(registersMock.number == 1 && loggerMock.outputResult == "DL moved into AH.");
        }

        public void TestInvalidCommands()
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

        private class GeneralPurposeRegistersMock : IRegistryModel
        {
            public int number;

            public ushort ax;
            public ushort bx;
            public ushort cx;
            public ushort dx;
            public byte[] GetRegistry(RegistryType registryType)
            {
                int regIndex = (int)registryType % 4;
                switch (regIndex)
                {
                    case 0:
                        return BitConverter.GetBytes(ax);
                    case 1:
                        return BitConverter.GetBytes(bx);
                    case 2:
                        return BitConverter.GetBytes(cx);
                    case 3:
                        return BitConverter.GetBytes(dx);
                    default:
                        return null;
                }
            }

            public void SetBytesToRegistry(RegistryType registryType, params byte[] bytes)
            {
                if (bytes.Length == 1)
                    bytes = new byte[] { bytes[0], 0 };
                int regIndex = (int)registryType % 4;
                switch (regIndex)
                {
                    case 0:
                        ax = BitConverter.ToUInt16(bytes);
                        break;
                    case 1:
                        bx = BitConverter.ToUInt16(bytes);
                        break;
                    case 2:
                        cx = BitConverter.ToUInt16(bytes);
                        break;
                    case 3:
                        dx = BitConverter.ToUInt16(bytes);

                        break;
                }
                number = BitConverter.ToUInt16(bytes);
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
