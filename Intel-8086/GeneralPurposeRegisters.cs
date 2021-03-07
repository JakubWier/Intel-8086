using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public delegate void RegistryChangedHandler(byte newValue);
    class GeneralPurposeRegisters
    {
        public event RegistryChangedHandler RegistryChanged;

        public GeneralPurposeRegisters()
        {
            registryBlock = new byte[4][];
            registryBlock[0] = new byte[2];
            registryBlock[1] = new byte[2];
            registryBlock[2] = new byte[2];
            registryBlock[3] = new byte[2];
        }

        byte[][] registryBlock;

        public byte[] GetAX => registryBlock[0];
        public byte GetAH => registryBlock[0][1];
        public byte GetAL => registryBlock[0][0];
        public byte[] GetBX => registryBlock[1];
        public byte GetBH => registryBlock[1][1];
        public byte GetBL => registryBlock[1][0];
        public byte[] GetCX => registryBlock[2];
        public byte GetCH => registryBlock[2][1];
        public byte GetCL => registryBlock[2][0];
        public byte[] GetDX => registryBlock[3];
        public byte GetDH => registryBlock[3][1];
        public byte GetDL => registryBlock[3][0];

        /// <summary>
        /// Sets passed bytes to selected register.
        /// Function intentionally simulates data loss if parameter "bytes" is too wide for 16bit registry or it's 8bit half. 
        /// </summary>
        public void SetBytes(RegistryType registryType, params byte[] bytes)
        {
            if (bytes == null || registryType.Equals(null))
                return;
            int registryIndex = (int)registryType;
            if (registryType <= RegistryType.DX)
            {
                registryBlock[registryIndex][0] = bytes[0];
                if(bytes.Length>1)
                    registryBlock[registryIndex][1] = bytes[1];
                else
                    registryBlock[registryIndex][1] = 0;
                return;
            }

            if (registryType >= RegistryType.AH && registryType <= RegistryType.DH)
            {
                registryIndex -= 4;
                SetHighByte(registryIndex, bytes[0]);
            }
            else
            {
                registryIndex -= 8;
                SetLowByte(registryIndex, bytes[0]);
            }
            return;
        }

        private void SetHighByte(int registryIndex, byte singleByte)
        {
            registryBlock[registryIndex][1] = singleByte;
        }
        private void SetLowByte(int registryIndex, byte singleByte)
        {
            registryBlock[registryIndex][0] = singleByte;
        }
    }
}
