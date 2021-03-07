using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    class RegistryChangedEventArgs : EventArgs
    {
        public readonly byte[] newBytes;
        public readonly RegistryType registry;

        public RegistryChangedEventArgs(RegistryType registry, byte[] newBytes)
        {
            this.newBytes = newBytes;
            this.registry = registry;
        }
    }
}
