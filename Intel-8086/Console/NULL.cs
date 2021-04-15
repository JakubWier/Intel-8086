using System;

namespace Intel_8086.Console
{
    class NULL : CommandHandler
    {
        public CommandHandler NextHandler { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string HandleOperation(string[] args)
        {
            return "Unsupported command line";
        }
    }
}
