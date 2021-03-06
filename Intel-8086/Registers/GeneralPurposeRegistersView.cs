﻿using System;
using System.ComponentModel;

namespace Intel_8086.Registers
{
    public class GeneralPurposeRegistersView : Observer, INotifyPropertyChanged
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

        public GeneralPurposeRegistersView(NumeralConverter numeralSystem)
        {
            numeralConverter = numeralSystem;
            AX = numeralConverter.IntToString(0);
            BX = numeralConverter.IntToString(0);
            CX = numeralConverter.IntToString(0);
            DX = numeralConverter.IntToString(0);
        }

        public void Update(object data)
        {
            ValueTuple<string, byte[]> updateData = (ValueTuple<string , byte[]>) data;
            int value = BitConverter.ToUInt16(updateData.Item2);

            switch (updateData.Item1)
            {
                case "AX":
                    AX = numeralConverter.IntToString(value);
                    break;
                case "BX":
                    BX = numeralConverter.IntToString(value);
                    break;
                case "CX":
                    CX = numeralConverter.IntToString(value);
                    break;
                case "DX":
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
