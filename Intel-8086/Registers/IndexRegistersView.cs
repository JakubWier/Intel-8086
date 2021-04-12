using System;
using System.ComponentModel;

namespace Intel_8086.Registers
{
    public class IndexRegistersView : Observer, INotifyPropertyChanged
    {
        public string SI { get => si; set { si = value; OnPropertyChanged("SI"); } }
        public string DI { get => di; set { di = value; OnPropertyChanged("DI"); } }

        private string si;
        private string di;

        private NumeralConverter numeralConverter;

        public event PropertyChangedEventHandler PropertyChanged;

        public IndexRegistersView(NumeralConverter numeralSystem)
        {
            numeralConverter = numeralSystem;
            SI = numeralConverter.IntToString(0);
            DI = numeralConverter.IntToString(0);
        }

        public void Update(object data)
        {
            ValueTuple<string, byte[]> updateData = (ValueTuple<string, byte[]>)data;
            int value = BitConverter.ToUInt16(updateData.Item2);

            switch (updateData.Item1)
            {
                case "SI":
                    SI = numeralConverter.IntToString(value);
                    break;
                case "DI":
                    DI = numeralConverter.IntToString(value);
                    break;
            }

        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
