namespace Intel_8086.Registers
{
    interface Registry
    {
        byte this[int i]
        {
            get;
            set;
        }
        byte[] Bytes { get; set; }
        string Name { get; }
    }
}
