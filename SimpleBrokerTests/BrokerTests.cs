using System;
using System.Threading;
using NUnit.Framework;
using SimpleBroker;

namespace SimpleBrokerTests
{
    [TestFixture]
    public class BrokerTests
    {
        private AutoResetEvent threadEvent;

        [SetUp]
        public void TestSetup()
        {
            Broker.Subscriptions.Clear();
            threadEvent = new AutoResetEvent(false);
        }

        [Test]
        public void Broker_CanCleanupSubscriptions()
        {
            var o = new object();
            o.Subscribe<TestClass>(null);

            Assert.IsNotEmpty(Broker.Subscriptions);
            o = null;

            GC.Collect();
            Broker.Cleanup();

            Assert.IsEmpty(Broker.Subscriptions);
        }

        [Test]
        public void Object_CanSubscribe()
        {
            var o = new object();
            o.Subscribe<TestClass>(tc => { });
            Assert.IsTrue(Broker.Subscriptions.Count == 1);
        }

        [Test]
        public void Broker_CanSubscribeObject()
        {
            var o = new object();
            Broker.Subscribe<TestClass>(o, tc => { });
            Assert.IsTrue(Broker.Subscriptions.Count == 1);
        }

        [Test]
        public void IsSubscribed_ReportsCorrectly()
        {
            var o = new object();
            Assert.IsFalse(o.IsSubscribed<TestClass>());

            o.Subscribe<TestClass>(null);
            Assert.IsTrue(o.IsSubscribed<TestClass>());
        }

        [Test]
        public void SubscribingTwice_FailsSilently()
        {
            var o = new object();
            o.Subscribe<TestClass>(tc => { });
            o.Subscribe<TestClass>(tc => { });

            Assert.IsTrue(Broker.Subscriptions.Count == 1);
        }

        [Test]
        public void UnsubscribingWhenNotSubscribed_FailsSilently()
        {
            var o = new object();
            o.Unsubscribe<TestClass>();

            Assert.IsEmpty(Broker.Subscriptions);
        }

        [Test]
        public void Object_CanUnsubscribe()
        {
            var o = new object();
            o.Subscribe<TestClass>(null);
            Assert.IsTrue(Broker.Subscriptions.Count == 1);

            o.Unsubscribe<TestClass>();
            Assert.IsEmpty(Broker.Subscriptions);
        }

        [Test]
        public void Publish_ReachesSubscribers()
        {
            var count = 0;
            var o = new object();
            var s = "foo";
            var t = new TestClass();

            o.Subscribe<TestClass>(tc => { count++; });
            s.Subscribe<TestClass>(tc => { count++; });
            count.Subscribe<TestClass>(tc => { count++; });

            t.Publish();

            Assert.AreEqual(3, count);
        }

        [Test]
        public void PublishAsync_ReachesSubscriber()
        {
            var count = 0;
            var t = new TestClass();
            var o = new object();

            o.Subscribe<TestClass>(tc =>
            {
                count++;
                threadEvent.Set();
            });
            
            t.PublishAsync();
            threadEvent.WaitOne();

            Assert.IsTrue(count == 1);
        }
    }

    class TestClass { }
}
