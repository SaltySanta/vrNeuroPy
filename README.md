# vrNeuroPy

This Unity-VR framework as been created to simulate different kinds of learning scenarios within scenes created in Unity. This is done by adding a network API to Unity, making it possible for a client software to send commands to Unity as well as parallely receive sensory information, such as images, in real time. This can be seen as a budget alternative to a robot, which can also provide sensory information needed for various learning algorithms.
This documentation aims to provide a superficial overview over the architecture of this framework and hopefully give you the necessary information to use this software, or at least give you an idea where to look for the information you're looking for.
If something doesn't work out as expected, you can always do the following to find out:

1) Read the old documentation, which focuses more in detail on how to create a scene. It might be out of date, though.

2) Read the DEV-doc for information on the Unity network API (SimpleNetwork), the network message serializer used (Protobuf), a more detailed explanation of the ClientArchitecture and the way the client communicated with Unity.

3) Read the Unity documentation if you're writing Unity code.

4) Take a look at the existing scenarios and Unity scripts (but there is no guarantee for the single scenarios and their scripts to be fully functioning).

5) Look at the (mostly documented) source files. The documentation should give you a general idea which files are responsible for that you want to use/change.
