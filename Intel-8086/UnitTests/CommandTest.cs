using Intel_8086.Registers;
using Intel_8086.Console;
using static System.Diagnostics.Debug;
using System;
using Intel_8086;
using Intel_8086.Memory;

namespace Tests_Intel_8086
{
    class CommandTest
    {
        public void StartAllTests()
        {
            TestAssignToRegistry();
            TestGeneralRegistersMOV();
            TestXCHG();
            TestInvalidCommands();
            TestFromToRegistryMemory();
        }
        public void TestAssignToRegistry()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);
            
            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

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
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);

            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            byte[] first;
            byte[] second;

            registersMock.SetBytesToRegistry("AX", BitConverter.GetBytes(Convert.ToUInt16(256)));
            registersMock.SetBytesToRegistry("BX", BitConverter.GetBytes(Convert.ToUInt16(255)));
            registryCommand.InputCommand("XchG aX, Bx");
            first = registersMock.GetRegistry("AX");
            second = registersMock.GetRegistry("BX");
            Assert(first[0] == 255 && first[1] == 0 && second[0] == 0 && second[1] == 1);
            Assert(loggerMock.outputResult == "AX exchanged with BX.");

            registryCommand.InputCommand("xchG al, cl");
            first = registersMock.GetRegistry("AX");
            second = registersMock.GetRegistry("CX");
            Assert(first[0] == 0 && first[1] == 0 && second[0] == 255 && second[1] == 0);
            Assert(loggerMock.outputResult == "AL exchanged with CL.");

            registersMock.SetBytesToRegistry("DX", BitConverter.GetBytes(Convert.ToUInt16(65535)));
            registryCommand.InputCommand("XchG bh, dh");
            first = registersMock.GetRegistry("BX");
            second = registersMock.GetRegistry("DX");
            Assert(first[0] == 0 && first[1] == 255 && second[0] == 255 && second[1] == 1);
            Assert(loggerMock.outputResult == "BH exchanged with DH.");

            registryCommand.InputCommand("xchg cx, dx");
            first = registersMock.GetRegistry("CX");
            second = registersMock.GetRegistry("DX");
            Assert(first[0] == 255 && first[1] == 1 && second[0] == 255 && second[1] == 0);
            Assert(loggerMock.outputResult == "CX exchanged with DX.");

            registryCommand.InputCommand("xchG dl, ch");
            first = registersMock.GetRegistry("DX");
            second = registersMock.GetRegistry("CX");
            Assert(first[0] == 1 && first[1] == 0 && second[0] == 255 && second[1] == 255);
            Assert(loggerMock.outputResult == "DL exchanged with CH.");
        }

        public void TestGeneralRegistersMOV()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);

            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            registryCommand.InputCommand("MoV AL, 16");
            Assert(registersMock.number == 16 && loggerMock.outputResult == "Parsing value \"16\" as decimal.\r\n10 moved into AL.");

            registryCommand.InputCommand("MOV Ch, 255");
            Assert(registersMock.number == 255 && loggerMock.outputResult == "Parsing value \"255\" as decimal.\r\nFF moved into CH.");

            registryCommand.InputCommand("MOV bx, 65535");
            Assert(registersMock.number == 65535 && loggerMock.outputResult == "Parsing value \"65535\" as decimal.\r\nFFFF moved into BX.");

            registryCommand.InputCommand("MOV bx, 65536");
            Assert(registersMock.number == 0 && loggerMock.outputResult == "Parsing value \"65536\" as decimal.\r\nExpected 16bit value.\nData loss due to conversion.\nMoving first and second byte of value.\n00 moved into BX.");

            registryCommand.InputCommand("mOV Bx, AX");
            Assert(registersMock.number == 16 && loggerMock.outputResult == "AX moved into BX.");

            registryCommand.InputCommand("MOV bL, cL");
            Assert(registersMock.number == 255 && loggerMock.outputResult == "CL moved into BL.");

            registryCommand.InputCommand("mov DH, 1");
            Assert(registersMock.number == 1 && loggerMock.outputResult == "Parsing value \"1\" as decimal.\r\n01 moved into DH.");
            registryCommand.InputCommand("mov AH, DL");
            Assert(registersMock.number == 1 && loggerMock.outputResult == "DL moved into AH.");

            registryCommand.InputCommand("mov AX, ff11H");
            Assert(registersMock.number == 65297 && loggerMock.outputResult == "Parsing value \"FF11\" as hexadecimal.\r\nFF11 moved into AX.");
        }

        public void TestFromToRegistryMemory()
        {
            MemoryModel.SetAddressBusLength = 20;
            MemoryModel memory = MemoryModel.GetInstance();
            GeneralPurposeRegisters generalRegistersMock = new GeneralPurposeRegisters();
            SegmentRegisters segmentRegistersMock = new SegmentRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, generalRegistersMock, segmentRegistersMock);

            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            registryCommand.InputCommand("MoV AL, 255");
            registryCommand.InputCommand("MoV [0], AL");
            byte bytetmp = memory.GetMemoryCell(0);
            Assert(bytetmp == 255 && loggerMock.outputResult == "Parsing value \"0\" as decimal.\r\nConverting arguments into effective address 0h.\r\nValue FFh from registry AL assigned to physical address 0h.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 255);
            registryCommand.InputCommand("MoV AH, AAh");
            registryCommand.InputCommand("MoV [FFh], AH");
            bytetmp = memory.GetMemoryCell(4335);
            Assert(bytetmp == 170 && loggerMock.outputResult == "Parsing value \"FF\" as hexadecimal.\r\nConverting arguments into effective address FFh.\r\nValue AAh from registry AH assigned to physical address 10EFh.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 15);
            registryCommand.InputCommand("MoV [0h], AX");
            byte[] word = memory.GetMemoryWord(240);
            Assert(word[0] == 255 && word[1] == 170 && loggerMock.outputResult == "Parsing value \"0\" as hexadecimal.\r\nConverting arguments into effective address 0h.\r\nValue AAFFh from registry AX assigned to physical address F0h.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 0);
            registryCommand.InputCommand("MoV AX, [0h]");
            word = generalRegistersMock.GetRegistry("AX");
            Assert(word[0] == 255 && word[1] == 0 && loggerMock.outputResult == "Parsing value \"0\" as hexadecimal.\r\nConverting arguments into effective address 0h.\r\nValue FFh assigned to registry AX from physical address 0h.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 255);
            registryCommand.InputCommand("MoV AX, [FEh]");
            word = generalRegistersMock.GetRegistry("AX");
            Assert(word[0] == 0 && word[1] == 170 && loggerMock.outputResult == "Parsing value \"FE\" as hexadecimal.\r\nConverting arguments into effective address FEh.\r\nValue AA00h assigned to registry AX from physical address 10EEh.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 15);
            registryCommand.InputCommand("MoV AX, [0h]");
            word = generalRegistersMock.GetRegistry("AX");
            Assert(word[0] == 255 && word[1] == 170 && loggerMock.outputResult == "Parsing value \"0\" as hexadecimal.\r\nConverting arguments into effective address 0h.\r\nValue AAFFh assigned to registry AX from physical address F0h.\r\n");
            
            MemoryModel.SetAddressBusLength = 0;
        }

        public void TestInvalidCommands()
        {
            GeneralPurposeRegistersMock registersMock = new GeneralPurposeRegistersMock();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);

            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

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
            Assert(loggerMock.outputResult == "MOV arguments must be separated by comma.");

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

            registryCommand.InputCommand("mov ax, [0");
            Assert(loggerMock.outputResult == "Argument is missing bracket.");

            registryCommand.InputCommand("mov [0, ax");
            Assert(loggerMock.outputResult == "Argument is missing bracket.");
        }

        private class GeneralPurposeRegistersMock : RegistersController
        {
            public int number;

            public ushort ax;
            public ushort bx;
            public ushort cx;
            public ushort dx;

            public bool Contains(string registryName)
            {
                if (ToRegistryIndex(registryName) != -1)
                    return true;
                return false;
            }

            public byte[] GetRegistry(string registryName)
            {
                int regIndex = ToRegistryIndex(registryName);
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

            public void SetBytesToRegistry(string registryName, params byte[] bytes)
            {
                if (bytes.Length == 1)
                    bytes = new byte[] { bytes[0], 0 };
                else
                {
                    if (registryName.ToString().EndsWith('L'))
                    {
                        bytes = new byte[] { bytes[0], GetRegistry(registryName)[1] };
                    } else if (registryName.ToString().EndsWith('H'))
                    {
                        bytes = new byte[] { GetRegistry(registryName)[0], bytes[0] };
                    }
                }
                int regIndex = ToRegistryIndex(registryName);
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

            private int ToRegistryIndex(string registryName) => registryName switch
            {
                "AX" => 0,
                "AH" => 0,
                "AL" => 0,
                "BX" => 1,
                "BH" => 1,
                "BL" => 1,
                "CX" => 2,
                "CH" => 2,
                "CL" => 2,
                "DX" => 3,
                "DH" => 3,
                "DL" => 3,
                _ => -1
            };
        }

        private class LoggerMock : OutputController
        {
            public string outputResult = "";
            public void ReplaceOutput(string line)
            {
                outputResult = line;
            }
        }
    }
}
