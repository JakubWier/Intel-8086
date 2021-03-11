using System;
using System.ComponentModel;

namespace Intel_8086
{
    class RegistryView : IObserver, INotifyPropertyChanged
    {
        public string AX { get; set; }
        public string BX { get; set; }
        public string CX { get; set; }
        public string DX { get; set; }

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
