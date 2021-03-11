using System;
using System.ComponentModel;

namespace Intel_8086
{
    class RegistryView : IObserver, INotifyPropertyChanged
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
            AX = "xD";
        }

        public void Update(object data)
        {
            ValueTuple<RegistryType, byte[]> D = (ValueTuple<RegistryType , byte[]>) data;
            int value = BitConverter.ToUInt16(D.Item2);
            AX = numeralConverter.IntToString(value);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
