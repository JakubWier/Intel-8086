﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Intel_8086
{
    public interface ICommandObserver
    {
        public void SetBytesToRegistry(RegistryType registryType, string valueH);
    }
}
