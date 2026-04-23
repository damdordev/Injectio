# Injectio

Injectio is a lightweight, fast, and simple Dependency Injection (DI) framework. It provides basic inversion of control (IoC) functionality designed for scenarios where full-fledged DI containers may be too heavy or complex.

# Table of Contents
- [Injectio](#injectio)
- [Features](#features)
- [Core concepts](#core-concepts)
  - [IDependencyContainer](#idependencycontainer)
  - [InjectAttribute](#injectattribute)
  - [DependencyResolver](#dependencyresolver)
- [Simple usage](#simple-usage)
  - [Create DependencyContainer](#create-dependencycontainer)
  - [Put dependency in container](#put-dependency-in-container)
  - [Create class with dependency attribute](#create-class-with-dependency-attribute)
  - [Resolve dependencies](#resolve-dependencies)
  - [Use interfaces instead of class](#use-interfaces-instead-of-class)
- [Injection types](#injection-types)
  - [Field](#field)
  - [Property](#property)
  - [Method](#method)
- [Advance usage](#advance-usage)
  - [DependencyContainerPack](#dependencycontainerpack)
  - [Custom IDependencyContainer](#custom-idependencycontainer)
  - [Resolving dependency directly from container](#resolving-dependency-directly-from-container)
  - [Only reference class support](#only-reference-class-support)
  - [Multithreading not supported due to performance](#multithreading-not-supported-due-to-performance)
  - [What happens when container doesn't have a reference](#what-happens-when-container-doesnt-have-a-reference)

# Features

* Lightweight and minimal API
* Support for Field, Property, and Method injection
* Extensible container abstraction (`IDependencyContainer`)
* Container packs for combining multiple containers (`DependencyContainerPack`)
* Fast reflection-based resolution

# Core concepts

## IDependencyContainer
An abstraction representing a source of dependencies. You can register dependencies here, and the framework will pull from it to satisfy injections.

## InjectAttribute
An attribute used to mark fields, properties, or methods that need dependencies injected.

## DependencyResolver
The core engine that scans a target object for members marked with `[Inject]` and resolves their values using a given `IDependencyContainer`.

# Simple usage

## Create DependencyContainer
```csharp
var container = new DependencyContainer();
```

## Put dependency in container
```csharp
var myService = new MyService();
container.Register(myService);
```

## Create class with dependency attribute
```csharp
public class MyConsumer
{
    [Inject]
    private MyService myService;
    
    public void DoWork()
    {
        myService.Execute();
    }
}
```

## Resolve dependencies
```csharp
var consumer = new MyConsumer();
DependencyResolver.Resolve(consumer, container);
// consumer.myService is now populated!
```

## Use interfaces instead of class
It is generally good practice to program against interfaces.
```csharp
public interface IMyService { void Execute(); }
public class MyService : IMyService { public void Execute() { } }

// Registration
container.Register<IMyService>(new MyService());

// Injection
public class MyConsumer
{
    [Inject]
    private IMyService myService;
}
```

# Injection types

## Field
```csharp
public class Example
{
    [Inject]
    private ILogger logger;
}
```

## Property
```csharp
public class Example
{
    [Inject]
    public ILogger Logger { get; private set; }
}
```

## Method
Method injection is useful when you need to run some initialization logic immediately after dependencies are provided.
```csharp
public class Example
{
    private ILogger logger;
    
    [Inject]
    public void Construct(ILogger logger)
    {
        this.logger = logger;
        this.logger.Log("Dependencies injected via method!");
    }
}
```

# Advance usage

## DependencyContainerPack
If you have multiple containers (e.g., global and local contexts), you can combine them:
```csharp
var globalContainer = new DependencyContainer();
var localContainer = new DependencyContainer();

var pack = new DependencyContainerPack(localContainer, globalContainer);

// Resolves by checking localContainer first, then globalContainer
DependencyResolver.Resolve(target, pack);
```

Container added last has the highest priority.

## Custom IDependencyContainer
You can implement `IDependencyContainer` to create your own specialized containers, such as resolving dependencies dynamically from a third-party framework or using an object pool.
```csharp
public class MyCustomContainer : IDependencyContainer
{
    public object Get(Type type)
    {
        // Custom logic here
        return null;
    }
}
```

## Resolving dependency directly from container
You don't always have to use `[Inject]`. You can retrieve dependencies directly:
```csharp
var service = container.Get<IMyService>();
// or
var serviceObj = container.Get(typeof(IMyService));
```

## Only reference class support
Value types (structs) cannot be the target of dependency injection because they are passed by value. The framework only supports injecting into reference types (classes). Also, the dependencies themselves should typically be reference types.

## Multithreading not supported due to performance
To ensure maximum performance on a single thread (like the main loop of a game engine), thread-safety locks have been omitted from `DependencyContainer`. Do not modify or read from the container across multiple threads simultaneously.

## What happens when container doesn't have a reference
If a dependency cannot be found in the container, it will return `null`. The `DependencyResolver` will then inject `null` into the target field, property, or method parameter. It will not throw an exception automatically, so be sure to check for nulls or ensure all required dependencies are registered.
