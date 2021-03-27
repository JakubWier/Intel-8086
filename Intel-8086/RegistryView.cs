using System;
using System.ComponentModel;
using Intel_8086.Registers;

namespace Intel_8086
{
    class RegistryView : Observer, INotifyPropertyChanged
    {
        public string AX { get => ax; set { ax = value; OnPropertyChanged("AX"); } }
        public string BX { get => bx; set { bx = value; OnPropertyChanged("BX"); } }
        public string CX { get => cx; set { cx = value; OnPropertyChanged("CX"); } }
        public string DX { get => dx; set { dx = value; OnPropertyChanged("DX"); } }

        private string ax;
        private string bx;
        private string cx;
        private string dx;

        private NumeralConverter numeralConverter;

        public event PropertyChangedEventHandler PropertyChanged;

        public RegistryView(NumeralConverter numeralSystem)
        {
            this.numeralConverter = numeralSystem;
            AX = numeralConverter.IntToString(0);
            BX = numeralConverter.IntToString(0);
            CX = numeralConverter.IntToString(0);
            DX = numeralConverter.IntToString(0);
        }

        public void Update(object data)
        {
            ValueTuple<GeneralPurposeRegistryType, byte[]> updateData = (ValueTuple<GeneralPurposeRegistryType , byte[]>) data;
            int value = BitConverter.ToUInt16(updateData.Item2);
            int registryIndex = ((int)updateData.Item1)%4;
            switch (registryIndex)
            {
                case 0:
                    AX = numeralConverter.IntToString(value);
                    break;
                case 1:
                    BX = numeralConverter.IntToString(value);
                    break;
                case 2:
                    CX = numeralConverter.IntToString(value);
                    break;
                case 3:
                    DX = numeralConverter.IntToString(value);
                    break;
            }
            
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
