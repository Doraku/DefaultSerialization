[![NuGet](https://buildstats.info/nuget/DefaultSerialization)](https://www.nuget.org/packages/DefaultSerialization)
[![Coverage Status](https://coveralls.io/repos/github/Doraku/DefaultSerialization/badge.svg?branch=master)](https://coveralls.io/github/Doraku/DefaultSerialization?branch=master)
![continuous integration status](https://github.com/doraku/DefaultSerialization/workflows/continuous%20integration/badge.svg)

- [API documentation](./documentation/api/DefaultSerialization.md 'API documentation')
<a/>

- [Requirements](#requirements)
- [Versioning](#versioning)
  - [Serialization](#serialization)
    - [TextSerializer](#textserializer)
    - [BinarySerializer](#binaryserializer)
- [Dependencies](#dependencies)

# Requirements
DefaultSerialization heavily uses features from C#7.0 and Span from the System.Memory package, compatible from .NETStandard 1.1.  
For development, a C#9.0 compatible environment, net framework 4.8, net6.0 are required to build and run all tests (it is possible to disable some targets in the test project if needed).  
It is possible to use DefaultSerialization in Unity (check [FAQ](./documentation/FAQ.md#unity)).

# Versioning
This is the current strategy used to version DefaultSerialization: v0.major.minor
- 0: DefaultSerialization is still in heavy development and although a lot of care is given not to break the current API, it can still happen
- major: incremented when there is a breaking change (reset minor number)
- minor: incremented when there is a new feature or a bug fix

## Serialization
DefaultSerialization supports serialization to save and load a world state. Two implementations are provided which are equal in functionality and it is possible to create a custom serialization engine using the framework of your choice by implementing a set of interfaces:

- `ISerializer` is the base interface
- `IComponentTypeReader` is used to get the settings of the serialized world in case a component max capacity has been set for a specific type different from the world's max capacity
- `IComponentReader` is used to get all the components of an entity

The provided implementations for `TextSerializer` and `BinarySerializer` are highly permissive and will serialize all fields and properties even if they are private or read-only, and do not require any attributes to work.  
This was the goal from the start as graphic and framework libraries do not always have well decorated types which would be used as components.  
Although the lowest target is netstandard1.1, please be aware that the capability of both implementations to handle types with no default constructors may not work if the version of your .NET platform is too low. Other known limitations are:
- no multi-dimensional array support
- no cyclic object graph support (all objects are copied, thus creating an infinite loop)
- not compatible with Xamarin.iOS, AOT platforms (uses the `System.Reflection.Emit` namespace)


```csharp
ISerializer serializer = new TextSerializer(); // or BinarySerializer

using (Stream stream = File.Create(filePath))
{
    serializer.Serialize(stream, world);
}

using (Stream stream = File.OpenRead(filePath))
{
    World worldCopy = serializer.Deserialize(stream);
}
```

Each implementation has its own serialization context which can be used to transform a given type to something else or just change the value during serialization or deserialization.
```csharp
using BinarySerializationContext context = new BinarySerializationContext()
    .Marshal<string, string>(_ => null) // set every string as null during serialization
    .Marshal<NonSerializableData, SerializableData>(d => new SerializableData(d)) // transform non serializable data to a serializable type during serialization
    .Unmarshal<SerializableData, NonSerializableData>(d => Load(d)); // reload non serializable data from serializable data during deserialization

BinarySerializer serializer = new BinarySerializer(context);
```

### TextSerializer
The purpose of this serializer is to provide a readable save format which can be edited by hand.
```
// declare the maximum number of entities in the World, this must be before any Entity or ComponentMaxCapacity line
WorldMaxCapacity 42

// this line is used to define an alias for a type used as a component inside the world and must be declared before being used
ComponentType Int32 System.Int32, System.Private.CoreLib

// this line is used to set the max capacity for the given type, in case it is different from the world max capacity
ComponentMaxCapacity Int32 13

// this creates a new entity with the id "Foo"
Entity Foo

// this line sets the component of the type with the alias Int32 on the previously created Entity to the value 13
Component Int32 13

// let's say we have the type defined as such already declared with the alias Test
// struct Test
// {
//     int Hello
//     int World
// }
ComponentType Test MyNamespace.Text, MyLib

// composite objects are set like this
Component Test {
	Hello 42
	// this line is ignored since the type does not have a member with the name Wow
	// also the World member will have its default value since not present
	Wow 43
}

// this creates a new entity with no id
Entity

// this sets the component of the type with the alias Test of the previously created Entity as the same as the one of the Entity with the id Foo
ComponentSameAs Test Foo
```
### BinarySerializer
This serializer is optimized for speed and file size.

# Dependencies
CI, tests and code quality rely on these awesome projects:
- [Coverlet](https://github.com/coverlet-coverage/coverlet)
- [NFluent](https://github.com/tpierrain/NFluent)
- [NSubstitute](https://github.com/nsubstitute/NSubstitute)
- [Roslynator](https://github.com/JosefPihrt/Roslynator)
- [XUnit](https://github.com/xunit/xunit)
