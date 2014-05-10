using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleBroker
{
    public static class Broker
    {
        static Broker()
        {
            Subscriptions = new List<Subscription>();
        }

        internal static List<Subscription> Subscriptions { get; set; }

        public static void Subscribe<T>(this object sender, Action<T> action)
        {
            if (IsSubscribed<T>(sender)) return;

            var subscription = new Subscription(sender, action, typeof(T));
            Subscriptions.Add(subscription);
        }

        public static void Unsubscribe<T>(this object sender)
        {
            var subscription = Subscriptions.FirstOrDefault(x => (x.Subscriber.Target == sender && x.Type == typeof(T)));
            if (subscription != null) Subscriptions.Remove(subscription);
        }

        public static void Publish<T>(this T sender)
        {
            Cleanup();

            var subscriptions = Subscriptions.Where(x => x.Type == sender.GetType());
            foreach (var subscription in subscriptions) ((Action<T>)subscription.Action)(sender);
        }

        public static void PublishAsync<T>(this T sender)
        {
            var thread = new Thread(() => { Publish(sender); });
            thread.Start();
        }

        public static bool IsSubscribed<T>(this object sender)
        {
            var subscription = Subscriptions.FirstOrDefault(x => (x.Subscriber.Target == sender && x.Type == typeof(T)));
            return subscription != null;
        }

        internal static void Cleanup()
        { 
            Subscriptions.RemoveAll(x => { return !x.Subscriber.IsAlive; });
        }
    }
}
