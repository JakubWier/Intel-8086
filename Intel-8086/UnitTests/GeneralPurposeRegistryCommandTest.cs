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

            byte[] first;
            byte[] second;

            registersMock.SetBytesToRegistry(GeneralPurposeRegistryType.AX, BitConverter.GetBytes(Convert.ToUInt16(256)));
            registersMock.SetBytesToRegistry(GeneralPurposeRegistryType.BX, BitConverter.GetBytes(Convert.ToUInt16(255)));
            registryCommand.InputCommand("XchG aX, Bx");
            first = registersMock.GetRegistry(GeneralPurposeRegistryType.AX);
            second = registersMock.GetRegistry(GeneralPurposeRegistryType.BX);
            Assert(first[0] == 255 && first[1] == 0 && second[0] == 0 && second[1] == 1);
            Assert(loggerMock.outputResult == "AX exchanged with BX.");

            registryCommand.InputCommand("xchG al, cl");
            first = registersMock.GetRegistry(GeneralPurposeRegistryType.AX);
            second = registersMock.GetRegistry(GeneralPurposeRegistryType.CX);
            Assert(first[0] == 0 && first[1] == 0 && second[0] == 255 && second[1] == 0);
            Assert(loggerMock.outputResult == "AL exchanged with CL.");

            registersMock.SetBytesToRegistry(GeneralPurposeRegistryType.DX, BitConverter.GetBytes(Convert.ToUInt16(65535)));
            registryCommand.InputCommand("XchG bh, dh");
            first = registersMock.GetRegistry(GeneralPurposeRegistryType.BX);
            second = registersMock.GetRegistry(GeneralPurposeRegistryType.DX);
            Assert(first[0] == 0 && first[1] == 255 && second[0] == 255 && second[1] == 1);
            Assert(loggerMock.outputResult == "BH exchanged with DH.");

            registryCommand.InputCommand("xchg cx, dx");
            first = registersMock.GetRegistry(GeneralPurposeRegistryType.CX);
            second = registersMock.GetRegistry(GeneralPurposeRegistryType.DX);
            Assert(first[0] == 255 && first[1] == 1 && second[0] == 255 && second[1] == 0);
            Assert(loggerMock.outputResult == "CX exchanged with DX.");

            registryCommand.InputCommand("xchG dl, ch");
            first = registersMock.GetRegistry(GeneralPurposeRegistryType.DX);
            second = registersMock.GetRegistry(GeneralPurposeRegistryType.CX);
            Assert(first[0] == 1 && first[1] == 0 && second[0] == 255 && second[1] == 255);
            Assert(loggerMock.outputResult == "DL exchanged with CH.");
        }

        public void TestMOV()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            GeneralRegistryCommand registryCommand = new GeneralRegistryCommand(registersMock, loggerMock);

            registryCommand.InputCommand("MoV AL, 16");
            Assert(registersMock.number == 16 && loggerMock.outputResult == "Parsing value from decimal.\n10 moved into AL.");

            registryCommand.InputCommand("MOV Ch, 255");
            Assert(registersMock.number == 255 && loggerMock.outputResult == "Parsing value from decimal.\nFF moved into CH.");

            registryCommand.InputCommand("MOV bx, 65535");
            Assert(registersMock.number == 65535 && loggerMock.outputResult == "Parsing value from decimal.\nFFFF moved into BX.");

            registryCommand.InputCommand("MOV bx, 65536");
            Assert(registersMock.number == 0 && loggerMock.outputResult == "Parsing value from decimal.\nExpected 16bit value.\nData loss due to conversion.\nMoving first two bytes.\n00 moved into BX.");

            registryCommand.InputCommand("mOV Bx, AX");
            Assert(registersMock.number == 16 && loggerMock.outputResult == "AX moved into BX.");

            registryCommand.InputCommand("MOV bL, cL");
            Assert(registersMock.number == 255 && loggerMock.outputResult == "CL moved into BL.");

            registryCommand.InputCommand("mov DH, 1");
            Assert(registersMock.number == 1 && loggerMock.outputResult == "Parsing value from decimal.\n01 moved into DH.");
            registryCommand.InputCommand("mov AH, DL");
            Assert(registersMock.number == 1 && loggerMock.outputResult == "DL moved into AH.");

            registryCommand.InputCommand("mov AX, ff11h");
            Assert(registersMock.number == 65297 && loggerMock.outputResult == "Parsing value from hexadecimal.\nFF11 moved into AX.");
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

            registryCommand.InputCommand("mov ak, bh");
            Assert(loggerMock.outputResult == "AK is unknown registry name.");

            registryCommand.InputCommand("mov ah, bk");
            Assert(loggerMock.outputResult == "BK is unknown registry name.");

            registryCommand.InputCommand("mov ah bk");
            Assert(loggerMock.outputResult == "MOV arguments must separated by comma.");

            registryCommand.InputCommand("xchg ak, bx");
            Assert(loggerMock.outputResult == "AK is unknown registry name.");

            registryCommand.InputCommand("xchg ah, bk");
            Assert(loggerMock.outputResult == "BK is unknown registry name.");

            registryCommand.InputCommand("xchg ah bk");
            Assert(loggerMock.outputResult == "XCHG arguments must separated by comma.");

            registryCommand.InputCommand("mov ch,");
            Assert(loggerMock.outputResult == "Too few arguments to function MOV.");

            registryCommand.InputCommand("xchg dh");
            Assert(loggerMock.outputResult == "Too few arguments to function XCHG.");

        }

        private class GeneralPurposeRegistersMock : IRegistryModel
        {
            public int number;

            public ushort ax;
            public ushort bx;
            public ushort cx;
            public ushort dx;
            public byte[] GetRegistry(GeneralPurposeRegistryType registryType)
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

            public void SetBytesToRegistry(GeneralPurposeRegistryType registryType, params byte[] bytes)
            {
                if (bytes.Length == 1)
                    bytes = new byte[] { bytes[0], 0 };
                else
                {
                    if (registryType.ToString().EndsWith('L'))
                    {
                        bytes = new byte[] { bytes[0], GetRegistry(registryType)[1] };
                    } else if (registryType.ToString().EndsWith('H'))
                    {
                        bytes = new byte[] { GetRegistry(registryType)[0], bytes[0] };
                    }
                }
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
