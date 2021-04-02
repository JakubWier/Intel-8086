using System;
using System.ComponentModel;

namespace Intel_8086.Registers
{
    class SegmentRegistersView : Observer, INotifyPropertyChanged
    {
        public string CS { get => cS; set { cS = value; OnPropertyChanged("BP"); } }
        public string SS { get => sS; set { sS = value; OnPropertyChanged("SP"); } }
        public string DS { get => dS; set { dS = value; OnPropertyChanged("BP"); } }
        public string ES { get => eS; set { eS = value; OnPropertyChanged("SP"); } }

        private string cS;
        private string sS;
        private string dS;
        private string eS;

        private NumeralConverter numeralConverter;

        public event PropertyChangedEventHandler PropertyChanged;

        public SegmentRegistersView(NumeralConverter numeralSystem)
        {
            numeralConverter = numeralSystem;
            CS = numeralConverter.IntToString(0);
            SS = numeralConverter.IntToString(0);
            DS = numeralConverter.IntToString(0);
            ES = numeralConverter.IntToString(0);
        }

        public void Update(object data)
        {
            ValueTuple<string, byte[]> updateData = (ValueTuple<string, byte[]>)data;
            int value = BitConverter.ToUInt16(updateData.Item2);

            switch (updateData.Item1)
            {
                case "CS":
                    CS = numeralConverter.IntToString(value);
                    break;
                case "SS":
                    SS = numeralConverter.IntToString(value);
                    break;
                case "DS":
                    DS = numeralConverter.IntToString(value);
                    break;
                case "ES":
                    ES = numeralConverter.IntToString(value);
                    break;
            }

        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
