using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ESLDesign.eTagTech.SDK.Events
{
    public class EventBusBuilder
    {
        internal SynchronizationContext context;

        public EventBusBuilder Context(SynchronizationContext Context)
        {
            this.context = Context;
            return this;
        }




        internal EventBusBuilder() { }


        public EventBus build()
        {
            return new EventBus(this);
        }

    }
}
