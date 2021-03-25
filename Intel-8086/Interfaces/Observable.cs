using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface Observable
    {
        public void UpdateObservers(object data);
        void AddObserver(Observer observer);
        void RemoveObserver(Observer observer);
    }
}
