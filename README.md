SimpleBroker is a broker-based implementation of the Observer pattern. 

If you want to subscribe:


    this.Subscribe<SomeClass>(x => {
        // Do something here.
    });
    
    // Or
    
    Broker.Subscribe<SomeClass>(this, x => {
        // Do something here.
    });


Unsubscribing is just as easy:


    this.Unsubscribe<SomeClass>();
    
    // Or
    
    Broker.Unsubscribe<SomeClass>(this);

 
To publish an object:

    var sc = new SomeClass();
    sc.Publish();
    
    // Or
    
    Broker.Publish(sc);


SimpleBroker also supports asynchronous publishing:


    var sc = new SomeClass();
    sc.PublishAsync();
    
    // Or
    
    Broker.PublishAsync(sc);
