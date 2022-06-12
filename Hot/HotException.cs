using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hot;

public class HotException : Exception
{
    public HotException(string msg): base(msg)
    {

    }

    public HotException(string msg, Exception cause): base(msg, cause)
    {

    }
}
