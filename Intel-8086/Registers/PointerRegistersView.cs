using System;
using System.ComponentModel;

namespace Intel_8086.Registers
{
    public class PointerRegistersView : Observer, INotifyPropertyChanged
    {
        public string BP { get => bP; set { bP = value; OnPropertyChanged("BP"); } }
        public string SP { get => sP; set { sP = value; OnPropertyChanged("SP"); } }

        private string bP;
        private string sP;

        private NumeralConverter numeralConverter;

        public event PropertyChangedEventHandler PropertyChanged;

        public PointerRegistersView(NumeralConverter numeralSystem)
        {
            numeralConverter = numeralSystem;
            BP = numeralConverter.IntToString(0);
            SP = numeralConverter.IntToString(0);
        }

        public void Update(object data)
        {
            ValueTuple<string, byte[]> updateData = (ValueTuple<string, byte[]>)data;
            int value = BitConverter.ToUInt16(updateData.Item2);

            switch (updateData.Item1)
            {
                case "BP":
                    BP = numeralConverter.IntToString(value);
                    break;
                case "SP":
                    SP = numeralConverter.IntToString(value);
                    break;
            }

        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
