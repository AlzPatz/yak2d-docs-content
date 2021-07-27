---
uid: uid_tut_continued
---
# Getting Started - Continued (part 2)

## Aim
Expand the blank project created in the [first tutorial](xref:uid_tut_gettingstarted) to:

1. Add a texture to the project
2. Draw the texture to an off-screen [RenderTarget](xref:rendertargets)
3. Apply a built in graphical effect (using a [RenderStage](xref:uid_renderstages)) to the contents of the off-screen [RenderTarget](xref:rendertargets) and display this on the screen

## Prerequisites 
* Completion of [Getting Started (part 1)](xref:uid_tut_gettingstarted)

## Steps

### Choose a Texture
Choose the Texture you wish to use in the application. This tutorial uses the following [.png](https://github.com/AlzPatz/yak2d/blob/master/logo.png) which you can download. Note: **yak2D** supports .PNG, .BNP, .JPG, .GIF and .TGA.

### Define and create the Application's root Texture directory
The [StartupConfig](xref:Yak2D.StartupConfig) object returned from [Configure()](xref:Yak2D.IApplication.Configure) in the application's implementation of [IApplication](xref:Yak2D.IApplication) defines a root Texture directory. This directory is relative to the directory within which the application's `csproj` project file resides, and whose relative address is where the framework will look for files when Texture load calls are made. In `MyApplication.cs` from the [first tutorial](xref:uid_tut_gettingstarted) a helper method is used to create a [StartupConfig](xref:Yak2D.StartupConfig) object. The helper method defines the directory as `Textures`. We must create this directory, add our texture file to it and then define how textures in this folder are included in the project.

#### Windows
Using FileExplorer, navigate to the application's project directory, right click and select `New -> Folder`. Name that folder `Textures`

#### Linux and MacOS 
Either use the OS GUI to create a new folder or navigate to the project directory using the terminal and enter:
```bash
    mkdir Textures
```  
### Copy the desired Texture into the new directory
Use standard operating system copy functions for this.

### Modify the .csproj file to include Textures in the compiled project
Textures can be either embedded in the application binary as resources or included in the application output directory. For this tutorial we embed resources. To do this, modify the `.csproj` file and add the following to include all files within the `Textures` directory and sub-folders as embedded resources:

```
<ItemGroup>
    <EmbeddedResource Include="Textures\**" />
</ItemGroup>
```

For reference, if you wish to keep Textures as separate files, copied to the application's output directory, include the following:

```
<ItemGroup> 
    <Content Include="$(MSBuildThisFileDirectory)Textures\**">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        <Link>Textures\%(RecursiveDir)\%(FileName)%(Extension)</Link>
    </Content>
</ItemGroup>
```
### Load the Texture
Create a variable in `MyApplication.cs` to store a reference to an [ITexture](xref:Yak2D.ITexture):

```csharp
private ITexture _texture;
```
We load the [ITexture](xref:Yak2D.ITexture) in `MyApplication.cs`'s [CreateResources()](xref:Yak2D.IApplication.CreateResources) method. As a reminder, all [resource](xref:uid_glossary#Resource) creation should be completed within, or be triggered by, this method. 

Assuming you have named your Texture `logo.png`, modify [CreateResources()](xref:Yak2D.IApplication.CreateResources) to load the texture using the following:

```csharp
public bool CreateResources(IServices yak)
{
    _texture = yak.Surfaces.LoadTexture("logo", AssetSourceEnum.Embedded);

    return true;
}
```