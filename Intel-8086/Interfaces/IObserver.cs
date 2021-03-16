using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface IObserver
    {
        void Update(object update);
    }
}
