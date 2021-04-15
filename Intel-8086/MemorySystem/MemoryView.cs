using System;
using System.Text;
using Intel_8086;

namespace Intel_8086.MemorySystem
{
    class MemoryView
    {
        public string GetView(UInt16 segmentAddress, UInt16 offset, UInt16 lines = 8)
        {
            StringBuilder viewBuilder = new StringBuilder();
            byte cell;
            Memory memory = MemoryModel.GetInstance();
            uint addressIterator = (uint)((segmentAddress << 4) + offset);

            for (UInt16 it = 0; it < lines; it++, offset += 16)
            {
                viewBuilder.Append(segmentAddress.ToString("X").PadLeft(4, '0') + ":" + offset.ToString("X").PadLeft(4, '0') + " ");

                for (uint singleByte = 0; singleByte < 16; singleByte++, addressIterator++)
                {
                    cell = memory.GetMemoryCell(addressIterator);
                    viewBuilder.Append(" ");
                    viewBuilder.Append(cell.ToString("X").PadLeft(2, '0'));
                    viewBuilder.Append(" ");
                }
                viewBuilder.Append("\n");
            }
            return viewBuilder.ToString();
        }

        public static string AddressInputToString(uint physicalAddress)
        {
            string physical = physicalAddress.ToString("X").PadLeft(5, '0');
            string segmentMock = physical.Substring(physical.Length - 5, 4);
            string offsetMock = physical.Substring(physical.Length - 1, 1).PadLeft(4, '0');
            return segmentMock + ":" + offsetMock;
        }
    }
}
