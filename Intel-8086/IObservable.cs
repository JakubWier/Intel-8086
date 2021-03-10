using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface IObservable
    {
        public void UpdateObservers(object data);
        void AddObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
    }
}
