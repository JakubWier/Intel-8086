using Microsoft.VisualStudio.TestTools.UnitTesting;
using Intel_8086;
using Intel_8086.Registers;
using Intel_8086.Console;
using System;
using Intel_8086.MemorySystem;

namespace Tests_Intel_8086
{
    [TestClass]
    public class CommandTest
    {

        [TestMethod]
        public void TestAssignToRegistry()
        {
            GeneralPurposeRegisters registersMock = new GeneralPurposeRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);
            
            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            registryCommand.InputCommand("AL 1");
            Assert.AreEqual(registersMock.GetRegistry("AL")[0], 1);
            Assert.AreEqual(loggerMock.outputResult, "01 assigned into AL.");

            registryCommand.InputCommand("AH F");
            Assert.AreEqual(registersMock.GetRegistry("AH")[0], 15);
            Assert.AreEqual(loggerMock.outputResult, "0F assigned into AH.");

            registryCommand.InputCommand("AX 123");
            byte[] reg = registersMock.GetRegistry("AX");
            Assert.AreEqual(reg[0], 35);
            Assert.AreEqual(reg[1], 1);
            Assert.AreEqual(loggerMock.outputResult, "0123 assigned into AX.");
        }

        [TestMethod]
        public void TestXCHG()
        {
            GeneralPurposeRegisters registersMock = new GeneralPurposeRegisters();
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
            Assert.AreEqual(first[0], 255);
            Assert.AreEqual(first[1], 0);
            Assert.AreEqual(second[0], 0);
            Assert.AreEqual(second[1], 1);
            Assert.AreEqual(loggerMock.outputResult, "AX exchanged with BX.");

            registryCommand.InputCommand("xchG al, cl");
            first = registersMock.GetRegistry("AX");
            second = registersMock.GetRegistry("CX");
            Assert.AreEqual(first[0], 0);
            Assert.AreEqual(first[1], 0);
            Assert.AreEqual(second[0], 255);
            Assert.AreEqual(second[1], 0);
            Assert.AreEqual(loggerMock.outputResult, "AL exchanged with CL.");

            registersMock.SetBytesToRegistry("DX", BitConverter.GetBytes(65535));
            registryCommand.InputCommand("XchG bh, dh");
            first = registersMock.GetRegistry("BX");
            second = registersMock.GetRegistry("DX");
            Assert.AreEqual(first[0], 0);
            Assert.AreEqual(first[1], 255);
            Assert.AreEqual(second[0], 255);
            Assert.AreEqual(second[1], 1);
            Assert.AreEqual(loggerMock.outputResult, "BH exchanged with DH.");

            registryCommand.InputCommand("xchg cx, dx");
            first = registersMock.GetRegistry("CX");
            second = registersMock.GetRegistry("DX");
            Assert.AreEqual(first[0], 255);
            Assert.AreEqual(first[1], 1);
            Assert.AreEqual(second[0], 255);
            Assert.AreEqual(second[1], 0);
            Assert.AreEqual(loggerMock.outputResult, "CX exchanged with DX.");

            registryCommand.InputCommand("xchG dl, ch");
            first = registersMock.GetRegistry("DX");
            second = registersMock.GetRegistry("CX");
            Assert.AreEqual(first[0], 1);
            Assert.AreEqual(first[1], 0);
            Assert.AreEqual(second[0], 255);
            Assert.AreEqual(second[1], 255);
            Assert.AreEqual(loggerMock.outputResult, "DL exchanged with CH.");
        }

        [TestMethod]
        public void TestGeneralRegistersMOV()
        {
            GeneralPurposeRegisters registersMock = new GeneralPurposeRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);

            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            byte[] reg;
            registryCommand.InputCommand("MoV AL, 16");
            reg = registersMock.GetRegistry("AL");
            Assert.AreEqual(reg[0], 16);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"16\" as decimal.\r\n10h moved into AL.");

            registryCommand.InputCommand("MOV Ch, 255");
            reg = registersMock.GetRegistry("CH");
            Assert.AreEqual(reg[0], 255);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"255\" as decimal.\r\nFFh moved into CH.");

            registryCommand.InputCommand("MOV bx, 65535");
            reg = registersMock.GetRegistry("BX");
            Assert.AreEqual(reg[0], 255);
            Assert.AreEqual(reg[1], 255);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"65535\" as decimal.\r\nFFFFh moved into BX.");

            registryCommand.InputCommand("MOV bx, 65536");
            reg = registersMock.GetRegistry("BX");
            Assert.AreEqual(reg[0], 0);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"65536\" as decimal.\r\nExpected 16bit value.\nData loss due to conversion.\nMoving first and second byte of value.\n00h moved into BX.");

            registryCommand.InputCommand("mOV Bx, AX");
            reg = registersMock.GetRegistry("BX");
            Assert.AreEqual(reg[0], 16);
            Assert.AreEqual(reg[1], 0);
            Assert.AreEqual(loggerMock.outputResult, "AX moved into BX.");

            registryCommand.InputCommand("MOV bL, ch");
            reg = registersMock.GetRegistry("BL");
            Assert.AreEqual(reg[0], 255);
            Assert.AreEqual(loggerMock.outputResult, "CH moved into BL.");

            registryCommand.InputCommand("mov DH, 1");
            reg = registersMock.GetRegistry("DH");
            Assert.AreEqual(reg[0], 1);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"1\" as decimal.\r\n01h moved into DH.");

            registryCommand.InputCommand("mov AH, DH");
            reg = registersMock.GetRegistry("AH");
            Assert.AreEqual(reg[0], 1);
            Assert.AreEqual(loggerMock.outputResult, "DH moved into AH.");

            registryCommand.InputCommand("mov AX, ff11H");
            reg = registersMock.GetRegistry("AX");
            Assert.AreEqual(reg[0], 17);
            Assert.AreEqual(reg[1], 255);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"FF11\" as hexadecimal.\r\nFF11h moved into AX.");
        }

        [TestMethod]
        public void TestXCHGMemoryRegistry()
        {
            MemoryModel.SetAddressBusLength = 20;
            MemoryModel memory = MemoryModel.GetInstance();
            GeneralPurposeRegisters generalRegistersMock = new GeneralPurposeRegisters();
            SegmentRegisters segmentRegistersMock = new SegmentRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, generalRegistersMock, new IndexRegisters(), new PointerRegisters(), segmentRegistersMock);

            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            registryCommand.InputCommand("MoV AL, FFh");
            registryCommand.InputCommand("MoV [SI + 0], AL");
            registryCommand.InputCommand("MoV AL, 1");
            registryCommand.InputCommand("XCHG [0], AL");
            byte bytetmp = memory.GetMemoryCell(0);
            Assert.AreEqual(bytetmp, 1);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"0\" as decimal.\r\nConverting arguments into effective address 0h.\r\nValue 1h from registry AL exchanged with FF at physical address 0h.\r\n");
            bytetmp = generalRegistersMock.GetRegistry("AL")[0];
            Assert.AreEqual(bytetmp, 255);


            segmentRegistersMock.SetBytesToRegistry("DS", 255);
            registryCommand.InputCommand("MoV CX, FF11h");
            registryCommand.InputCommand("MoV [SI + BP + FFh], CX");
            registryCommand.InputCommand("MoV CX, 2233h");
            registryCommand.InputCommand("XCHG CX, [100h]");
            byte[] word = generalRegistersMock.GetRegistry("CX");
            Assert.AreEqual(word[0], 255);
            Assert.AreEqual(word[1], 0);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"100\" as hexadecimal.\r\nConverting arguments into effective address 100h.\r\nValue 2233h from registry CX exchanged with FF at physical address 10F0h.\r\n");
            word = memory.GetMemoryWord(4336);
            Assert.AreEqual(word[0], 51);
            Assert.AreEqual(word[1], 34);

            MemoryModel.SetAddressBusLength = 0;
        }

        [TestMethod]
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
            Assert.AreEqual(bytetmp, 255);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"0\" as decimal.\r\nConverting arguments into effective address 0h.\r\nValue FFh from registry AL assigned to physical address 0h.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 255);
            registryCommand.InputCommand("MoV AH, AAh");
            registryCommand.InputCommand("MoV [FFh], AH");
            bytetmp = memory.GetMemoryCell(4335);
            Assert.AreEqual(bytetmp, 170);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"FF\" as hexadecimal.\r\nConverting arguments into effective address FFh.\r\nValue AAh from registry AH assigned to physical address 10EFh.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 15);
            registryCommand.InputCommand("MoV [0h], AX");
            byte[] word = memory.GetMemoryWord(240);
            Assert.AreEqual(word[0], 255);
            Assert.AreEqual(word[1], 170);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"0\" as hexadecimal.\r\nConverting arguments into effective address 0h.\r\nValue AAFFh from registry AX assigned to physical address F0h.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 0);
            registryCommand.InputCommand("MoV AX, [0h]");
            word = generalRegistersMock.GetRegistry("AX");
            Assert.AreEqual(word[0], 255);
            Assert.AreEqual(word[1], 0);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"0\" as hexadecimal.\r\nConverting arguments into effective address 0h.\r\nValue FFh assigned to registry AX from physical address 0h.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 255);
            registryCommand.InputCommand("MoV AX, [FEh]");
            word = generalRegistersMock.GetRegistry("AX");
            Assert.AreEqual(word[0], 0);
            Assert.AreEqual(word[1], 170);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"FE\" as hexadecimal.\r\nConverting arguments into effective address FEh.\r\nValue AA00h assigned to registry AX from physical address 10EEh.\r\n");

            segmentRegistersMock.SetBytesToRegistry("DS", 15);
            registryCommand.InputCommand("MoV AX, [0h]");
            word = generalRegistersMock.GetRegistry("AX");
            Assert.AreEqual(word[0], 255);
            Assert.AreEqual(word[1], 170);
            Assert.AreEqual(loggerMock.outputResult, "Parsing value \"0\" as hexadecimal.\r\nConverting arguments into effective address 0h.\r\nValue AAFFh assigned to registry AX from physical address F0h.\r\n");
            
            MemoryModel.SetAddressBusLength = 0;
        }

        [TestMethod]
        public void TestPUSH()
        {
            MemoryModel.SetAddressBusLength = 20;
            MemoryModel memory = MemoryModel.GetInstance();
            GeneralPurposeRegisters generalRegistersMock = new GeneralPurposeRegisters();
            SegmentRegisters segmentRegistersMock = new SegmentRegisters();
            PointerRegisters pointerRegistersMock = new PointerRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, generalRegistersMock, pointerRegistersMock, segmentRegistersMock);

            PUSH push = new PUSH();
            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(push);
            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            byte[] word;

            registryCommand.InputCommand("ax 1122");
            registryCommand.InputCommand("push ax");
            word = memory.GetMemoryWord(0);
            Assert.AreEqual(word[0], 34);
            Assert.AreEqual(word[1], 17);
            Assert.AreEqual("Value 1122h from registry AX pushed on stack with physical address 0h.\r\n", loggerMock.outputResult);

            word = pointerRegistersMock.GetRegistry("SP");
            Assert.AreEqual(word[0], 2);

            registryCommand.InputCommand("ax FFEE");
            registryCommand.InputCommand("bx AA99");
            registryCommand.InputCommand("SS FF");
            registryCommand.InputCommand("push ax");
            registryCommand.InputCommand("push bx");
            word = memory.GetMemoryWord(4083);
            Assert.AreEqual(word[0], 255);
            Assert.AreEqual(word[1], 153);
            Assert.AreEqual("Value AA99h from registry BX pushed on stack with physical address FF4h.\r\n", loggerMock.outputResult);

            word = pointerRegistersMock.GetRegistry("SP");
            Assert.AreEqual(word[0], 6);
        }

        [TestMethod]
        public void TestPOP()
        {
            MemoryModel.SetAddressBusLength = 20;
            MemoryModel memory = MemoryModel.GetInstance();
            GeneralPurposeRegisters generalRegistersMock = new GeneralPurposeRegisters();
            SegmentRegisters segmentRegistersMock = new SegmentRegisters();
            PointerRegisters pointerRegistersMock = new PointerRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, generalRegistersMock, pointerRegistersMock, segmentRegistersMock);

            POP pop = new POP();
            PUSH push = new PUSH();
            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(pop);
            registryCommand.AddHandler(push);
            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            byte[] word;

            registryCommand.InputCommand("ax 1122");
            registryCommand.InputCommand("push ax");
            registryCommand.InputCommand("pop cx");
            word = generalRegistersMock.GetRegistry("CX");
            Assert.AreEqual(34, word[0]);
            Assert.AreEqual(17, word[1]);
            Assert.AreEqual("Value 1122h from physical address 0h popped from stack to registry CX.\r\n", loggerMock.outputResult);

            word = pointerRegistersMock.GetRegistry("SP");
            Assert.AreEqual(0, word[0]);

            registryCommand.InputCommand("ax FFEE");
            registryCommand.InputCommand("bx AA99");
            registryCommand.InputCommand("SS FF");
            registryCommand.InputCommand("push ax");
            registryCommand.InputCommand("push bx");
            registryCommand.InputCommand("pop dx");
            word = generalRegistersMock.GetRegistry("DX");
            Assert.AreEqual(153, word[0]);
            Assert.AreEqual(170, word[1]);
            Assert.AreEqual("Value AA99h from physical address FF2h popped from stack to registry DX.\r\n", loggerMock.outputResult);

            word = pointerRegistersMock.GetRegistry("SP");
            Assert.AreEqual(2, word[0]);
        }

        [TestMethod]
        public void TestInvalidCommands()
        {
            GeneralPurposeRegisters registersMock = new GeneralPurposeRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);

            POP pop = new POP();
            PUSH push = new PUSH();
            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(pop);
            registryCommand.AddHandler(push);
            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            registryCommand.InputCommand("");
            Assert.AreEqual(loggerMock.outputResult, "");

            registryCommand.InputCommand(" ");
            Assert.AreEqual(loggerMock.outputResult, "Invalid command line.");

            registryCommand.InputCommand("][/]'/]['");
            Assert.AreEqual(loggerMock.outputResult, "Invalid command line.");

            registryCommand.InputCommand("mov ak, bh");
            Assert.AreEqual(loggerMock.outputResult, "AK is unsupported registry name.");

            registryCommand.InputCommand("mov ah, bk");
            Assert.AreEqual(loggerMock.outputResult, "BK is unsupported registry name.");

            registryCommand.InputCommand("mov ah bk");
            Assert.AreEqual(loggerMock.outputResult, "MOV arguments must be separated by comma.");

            registryCommand.InputCommand("xchg ak, bx");
            Assert.AreEqual(loggerMock.outputResult, "AK is unsupported registry name.");

            registryCommand.InputCommand("xchg ah, bk");
            Assert.AreEqual(loggerMock.outputResult, "BK is unsupported registry name.");

            registryCommand.InputCommand("xchg ah bk");
            Assert.AreEqual(loggerMock.outputResult, "XCHG arguments must be separated by comma.");

            registryCommand.InputCommand("mov ch,");
            Assert.AreEqual(loggerMock.outputResult, "Too few arguments to function MOV.");

            registryCommand.InputCommand("xchg dh");
            Assert.AreEqual(loggerMock.outputResult, "Too few arguments to function XCHG.");

            registryCommand.InputCommand("mov ax, [0");
            Assert.AreEqual(loggerMock.outputResult, "Argument is missing bracket.");

            registryCommand.InputCommand("mov [0, ax");
            Assert.AreEqual(loggerMock.outputResult, "Argument is missing bracket.");            
            
            registryCommand.InputCommand("xchg bl, [0");
            Assert.AreEqual(loggerMock.outputResult, "Argument is missing bracket.");

            registryCommand.InputCommand("mov [0, ch");
            Assert.AreEqual(loggerMock.outputResult, "Argument is missing bracket.");            
        }

        [TestMethod]
        private void TestInvalidPUSHCommand()
        {
            GeneralPurposeRegisters registersMock = new GeneralPurposeRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);

            PUSH push = new PUSH();
            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(push);
            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            registryCommand.InputCommand("push");
            Assert.AreEqual(loggerMock.outputResult, "Pushing registry on stack requires two arguments.");

            registryCommand.InputCommand("push ak");
            Assert.AreEqual(loggerMock.outputResult, "AK is unsupported registry name.\r\n");

            registryCommand.InputCommand("push ax");
            Assert.AreEqual(loggerMock.outputResult, "Command interpreter couldn't find SP registry.\r\n");

            registryCommand = new RegistryCommander(loggerMock, registersMock, new PointerRegisters());
            registryCommand.InputCommand("push ax");
            Assert.AreEqual(loggerMock.outputResult, "Command interpreter couldn't find SS registry.\r\n");
        }

        private void TestInvalidPOPCommand()
        {
            GeneralPurposeRegisters registersMock = new GeneralPurposeRegisters();
            LoggerMock loggerMock = new LoggerMock();
            RegistryCommander registryCommand = new RegistryCommander(loggerMock, registersMock);

            POP pop = new POP();
            PUSH push = new PUSH();
            XCHG xchg = new XCHG();
            MOV mov = new MOV();
            AssignToRegistry assignTo = new AssignToRegistry();

            registryCommand.AddHandler(pop);
            registryCommand.AddHandler(push);
            registryCommand.AddHandler(xchg);
            registryCommand.AddHandler(mov);
            registryCommand.AddHandler(assignTo);

            registryCommand.InputCommand("pop");
            Assert.AreEqual(loggerMock.outputResult, "Popping registry from stack requires two arguments.");

            registryCommand.InputCommand("pop ak");
            Assert.AreEqual(loggerMock.outputResult, "AK is unsupported registry name.\r\n");

            registryCommand.InputCommand("pop ax");
            Assert.AreEqual(loggerMock.outputResult, "Command interpreter couldn't find SP registry.\r\n");

            registryCommand = new RegistryCommander(loggerMock, registersMock, new PointerRegisters());
            registryCommand.InputCommand("pop ax");
            Assert.AreEqual(loggerMock.outputResult, "Command interpreter couldn't find SS registry.\r\n");
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
