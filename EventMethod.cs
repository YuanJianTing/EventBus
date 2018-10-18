using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT4SignalRServer.Events
{

    /// <summary>
    /// 自定义特性 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventMethod : Attribute
    {

    }
}
