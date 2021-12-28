# Gum

Gum is planned to be a collection of tools/APIs focusing mainly on game development.


## Pooling

This is a very basic implementation of the pooling system.
```CSharp
PoolBuilder<Foo> poolBuilder = new PoolBuilder<Foo>();
IPool<Foo> pool = _poolBuilder
                .SetPoolType(PoolType.Stack) //this is the default pool type
                .FromPoolableInstanceProvider(new FooInstanceProvider())
                .WithInitialSize(10) //this is 0 by default
                .Build();
```

Alternatively, you can use "FromMethod"
```CSharp
IPool<Foo> pool = _poolBuilder
                .SetPoolType(PoolType.Stack)
                .FromMethod(Create)
                .Build();
```

Pool usage
```CSharp
Foo foo = pool.Get(); //gets an object from the pool, if it doesn't exists it creates one

pool.Put(foo); //puts the object back in the pool
```

PoolCollection usage
```CSharp
PoolBuilder<Foo> poolBuilder = new PoolBuilder<Foo>();
//configuring the builder
poolBuilder
          .FromPoolableInstanceProvider(new FooInstanceProvider())
          .SetPoolType(PoolType.Stack);

PoolCollection<Foo> poolCollection = new PoolCollection<Foo>(poolBuilder);

int key = 0;

Foo foo = _poolCollection.Get(key); //gets an object from the pool with the specific key, creates one if the pool is empty

poolCollection.Put(key, foo); //puts the object back in the pool with the specific key
```
