using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLDesign.eTagTech.SDK.Events
{

    /// <summary>
    /// 自定义特性 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventMethodAttribute : Attribute
    {
        public ThreadMode ThreadMode { get; set; } = ThreadMode.None;

    }

    public enum ThreadMode
    {
        None=0,
        MAIN = 1,
        ASYNC=2
    }

}
