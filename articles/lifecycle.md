---
uid: uid_lifecycle
---
# **Application Lifecycle**

Below is a (somewhat simplified) overview of program flow for an application using the yak2D framework.

Methods which the user defines (or in the case of Launcher.Run(), calls) are coloured in light pink.

The program loop is broadly split into two parts - Update steps and Render steps. 

The frequency of Update steps is defined by the [UpdatePeriod](xref:Yak2D.UpdatePeriod) Type declared in the [StartupConfig](xref:Yak2D.StartupConfid) returned by the Configure() method of the user's [IApplication](xref:Yak2D.IApplication) implementation.

The render steps are executed whenever all required Updates have been processed, and as quickly as the GPU is able too, subject to vertical sync waits if required. Once Render commands are passed to the GPU, the main execution thread is not blocked, allowing additional Update steps to be processed, if required, until the GPU becomes free again.

![](../images/lifecycle.png)