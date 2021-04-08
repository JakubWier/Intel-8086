using System;
using System.Text;

namespace Intel_8086.Memory
{
    class MemoryView
    {
        public string GetView(UInt16 address, UInt16 lines = 8)
        {
            StringBuilder viewBuilder = new StringBuilder();
            byte cell;
            Memory memory = MemoryModel.GetInstance();
            for (UInt16 it = 0; it < lines; it++, address += 16)
            {
                viewBuilder.Append(address.ToString("X").PadLeft(4, '0') + " |");
                for(uint offset = 0; offset < 16; offset++)
                {
                    cell = memory.GetMemoryCell(address + offset);
                    viewBuilder.Append(" ");
                    viewBuilder.Append(cell.ToString("X").PadLeft(2, '0'));
                    viewBuilder.Append(" ");
                }
                viewBuilder.Append("\n");
            }
            return viewBuilder.ToString();
        }
    }
}
