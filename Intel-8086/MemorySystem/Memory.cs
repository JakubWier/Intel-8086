﻿namespace Intel_8086.MemorySystem
{
    public interface Memory
    {
        public byte GetMemoryCell(uint address);
        public void SetMemoryCell(uint address, byte value);
    }
}
