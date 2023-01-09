# Gum

Gum is a collection of tools focusing mainly on game development with the goal of getting rid of the boilerplate code.

# Trusted By
<p align="center">
  <img src="Resources/gleamgames.png?raw=true width="350" title="Gleam Games">
</p>

# Made With Gum

You can create a pull request or email me directly to display your game/company here emrefistikcioglu@gmail.com

<p align="center">
  <img src="Resources/everblast.png?raw=true width="350" height="350" title="Gleam Games">
</p>

[EverBlast - Blast and Match](https://play.google.com/store/apps/details?id=com.gleamgames.everblast&hl=en&gl=US)

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

public class Foo : IPoolable
{
    public event Action OnReturnToPoolRequested;

    public Reset()
    {
        //reset object's values   
    }

    public void Erase()
    {
        //delete the object
        GC.SuppressFinalize(this);
    }
}
```

Pool usage with Unity's MonoBehaviour

```CSharp
public class Foo : MonoBehaviour, IPoolable
{
    public event Action OnReturnToPoolRequested;

    public Reset()
    {
        gameObject.SetActive(true);  
    }
    
    public void ReturnToPool()
    {
        gameObject.SetActive(false);
        OnReturnToPoolRequested.Invoke();      
    }

    public void Erase()
    {
        if(this == null) //to avoid race conditions with Unity's object life-time
        {
            return;
        }
    
        Destroy(gameObject);
    }
}
```

PoolCollection<TKey, TValue> usage
```CSharp
PoolBuilder<Foo> poolBuilder = new PoolBuilder<Foo>();
//configuring the builder
poolBuilder
          .FromPoolableInstanceProvider(new FooInstanceProvider())
          .SetPoolType(PoolType.Stack);

PoolCollection<int, Foo> poolCollection = new PoolCollection<int, Foo>(poolBuilder);

int key = 0;

Foo foo = _poolCollection.Get(key); //gets an object from the pool with the specific key, creates one if the pool is empty
```

## Composer
<p>Composer allows you to work with data compositions instead of inheritance thus allowing you to create data agnostic systems and cleaner, decoupled code.</p>
<p>In order to work with the Composer, you have to use the aspect system.</p>

1. Go to ```Gum.Composer\Aspects```
2. Create a file with extension ".gum"
3. Start typing your aspect
4. Run the codegen using ```CompositionCodeGenerator.Run()```

Example aspect file
MyAspect.gum
```
aspect MyAspect 
{
    int MyInt;
    
    string MyAspect;
    
    double MyDouble;
    
    Transform MyTransform;
    
    Vector3 MyVector3;
}
```
You can use ANY object type while creating aspects.

```CSharp
private void Run()
{
    //you can call this method from anywhere (from unity editor or a console application)
    CompositionCodeGenerator.Run(); 
}
```

You can use the ```Gum.Composer\UserConfig.cs``` to configure the codegen.

```CSharp
public struct Foo : IComposable
{
    public int foo;
    
    public string bar;
    
    public Vector3 baz;

    public Composition GetComposition()
    {
        return Composition.Create(new IAspect[]
        {
            new FooAspect(foo),
            new BarAspect(bar),
            new BazAspect(baz)
        });
    }
}

public class Bar : IComposable
{
    public double Qux;
    
    public string bar;
    
    public Composition GetComposition()
    {
        return Composition.Create(new IAspect[]
        {
            new QuxAspect(foo),
            new BarAspect(bar),
        });
    }
}

//as you can see this method can use both Foo and and Bar to operate
public void UseAspects(IComposable composable)
{
    //always use the disposable pattern or manually dispose composition
    using (Composition composition = composable.GetComposition())
    {
        BarAspect barAspect = _composition.GetAspect<BarAspect>();				
    }
}
```

Other usages
```CSharp
composition
    .GetAspectFluent(out FooAspect fooAspect)
    .GetAspectFluent(out BarAspect barAspect);
	
BarAspect barAspect = (BarAspect)_composition[BarAspect.ASPECT_TYPE];

composition.AddAspect(new BarAspect());

composition.SetAspect(new BarAspect());

composition.RemoveAspect(BarAspect.ASPECT_TYPE);

foreach (IAspect aspect in composition)
{
    BarAspect barAspect = (BarAspect)aspect;
}
```
