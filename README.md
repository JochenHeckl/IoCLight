# IoCLight
IoCLight is a minimal IoC Container for the [Unity](https://unity.com/) Game Engine. IoCLight is designed to work with il2cpp.

Find the [Quick Start Guide](https://github.com/JochenHeckl/IoCLight/blob/master/Documentation~/GettingStarted.md) here;

Motivation:
I found Inversion of Control Containers to be a very simple to apply, yet effective, way to keep code structured and clean.
I'm used to tools like Autofac, Ninject, Unity (The IoC Container) and alike, and I used them heavily in the past.

So why would I go write my own?

Well I tried some of the ones mentioned above in Unity projects, but was never really pleased with how they fit into Unity.
It was only recently when I hit a hard wall porting a game to Android. The project used Autofac. It failed at runtime due to il2cpp not being able to deal correctly with C# Expressions.

After digging into the problem for a while and finding no applicable solution or alternative for quite a while I finally replaced Autofac with [{a}adic](https://assetstore.unity.com/packages/tools/adic-dependency-injection-container-32157).
which I can highly recommend if you need something that *works with Android using il2cpp as backend!*

When I started looking into building custom packages for the Unity Package Manager, I figured rolling a very simple IoC Container as a custom package just for science might be a nice little piece of work to learn about building custom packages. And here we go.

Feedback and contributions are welcome!

Have fun using IoCLight!


