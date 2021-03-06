using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public delegate void RegistryChangedHandler(byte newValue);
    class GeneralPurposeRegisters
    {
        public event RegistryChangedHandler RegistryChanged;

        byte[] AX = new byte[2];
        byte[] BX = new byte[2];
        byte[] CX = new byte[2];
        byte[] DX = new byte[2];

        public byte[] GetAX => AX;
        public byte[] GetBX => BX;
        public byte[] GetCX => CX;
        public byte[] GetDX => DX;
    }
}
