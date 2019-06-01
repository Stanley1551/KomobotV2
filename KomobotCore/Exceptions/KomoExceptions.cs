using System;
using System.Collections.Generic;
using System.Text;

namespace KomobotCore.Exceptions
{
    public class TwitchStreamNotFoundException : Exception
    {
        public TwitchStreamNotFoundException() : base()
        {
        }

        public TwitchStreamNotFoundException(string msg) : base(msg)
        {

        }
    }
}
