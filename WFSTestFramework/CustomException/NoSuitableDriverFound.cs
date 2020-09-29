using System;

namespace WFSTestFramework.CustomException
{
    internal class NoSuitableDriverFound : Exception
    {
        public NoSuitableDriverFound(string msg) : base(msg)
        {
        }
    }
}