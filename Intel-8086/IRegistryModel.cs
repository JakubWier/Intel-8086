using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface IRegistryModel
    {
        byte[] GetRegistry(RegistryType registryType);
        public void SetBytesToRegistry(RegistryType registryType, params byte[] bytes);
    }
}
