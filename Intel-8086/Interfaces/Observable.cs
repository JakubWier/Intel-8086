using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface Observable
    {
        public void UpdateObservers(object data);
        public void AddObserver(Observer observer);
        public void RemoveObserver(Observer observer);
    }
}
