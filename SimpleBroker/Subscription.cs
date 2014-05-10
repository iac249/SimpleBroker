using System;

namespace SimpleBroker
{
    internal class Subscription
    {
        public Subscription(object subscriber, object action, Type type)
        {
            Action = action;
            Type = type;
            Subscriber = new WeakReference(subscriber);
        }

        public object Action { get; set; }

        public Type Type { get; set; }

        public WeakReference Subscriber { get; set; }
    }
}
