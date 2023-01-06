# My Digital Diary
This is a demo project used for demonstrating some functionality that is available with Nuget repositories. This isn't a bleeding edge demo. But for some reason I never saw these techniques in my projects or others and didn't knew of them until I followed the session of Claire Novotny at Techorama 2022.
The demo project is a digital diary where you can write some things to in memory. This was kept simple deliberately, as the implementation doesn't matter for this demo.
After this demo you should have seen some techniques to make development easier and more secure for you.
## Prerequisites
In order to be able to run this project you need Visual Studio 2022 with .net6 & .net7.
Both SDKs were only added to demonstrate that all these things support multi target frameworks.
### Tools
- [GitHub - madskristensen/CommandTaskRunner: A Visual Studio extension](https://github.com/madskristensen/CommandTaskRunner)
This extensions visual studio extension simplifies/automates certain actions a lot.
- [GitHub - NuGet/PackageSourceMapper: A home of the PackageSourceMapper tool that helps onboard users to PackageSourceMapping](https://github.com/NuGet/PackageSourceMapper)
A great tool to help you construct your package source mapping as you don't want to do this all by yourself for large projects.

## Project structure
Everything is created with relative paths, in order to start this project make sure that the following folders are present, if not create them or install the CommandTaskRunner extension then it will be done automatically for you at startup.
- MyDigitalDiary/Nuget/GlobalPackagesFolder, this folder is the local cache, for this demo it's better not to taint the local one on you machine and for the last part of the demoe it's actually important that this is a separate folder.
- MyDigitalDiary/Nuget/PrivatePackageSource, this represent the firms internal Nuget repository
- MyDigitalDiary/Nuget/PublicPackageSource, this represent a public repository like the public nuget.org repository, although that one is also in this project it wasn't my desire to publish any packages to that source. It's easier to manipulate things when they are local for demonstation purposes.

### .solution items
#### Directory.Build.Props (& Directory.Build.Targets)
In the root of the project and in the nuget src folder a **Directory.Build.props** & **Directory.Build.targets** was added these names are case sensitive when building on a linux environment. The purpose of these files is that whenever a project is added under the src of the Nuget I don't need to add these settings manually. This reduces a lot of copy/pasting issues and makes sure that the global settings are set just once and none are forgotten. If for some reason a setting needs to be overwritten you can still do this.

The difference between the props & targets is that some properties aren't known upfront. Like AssemblyName, is not known upfront because it can be altered by defining the AssemblyName element in the csproj file.

Note that the Directory.Build.props under the Nuget references to the one in the root. This is because once a project find a Directory.Build.props file it stops navigating up to the root of the system searching for one.
#### commands.json
In the root of the solution the command.json file is located. It contains some snippets that you could also type manually but with the integration of the command task runner the need for it is being reduced and some steps could be automated this way.

### Nuget
All the projects defined under this folder are autocreating a nuget package upon build. This is configured in the Directory.Build.props
#### DigitalDiary.Abstractions
Nothing special it holds the models and interfaces
#### DigitalDiary.MemoryRepository
This is a very simple repository, consuming the interface.

### WebApi
A very simple web API with a swagger being configured, this way we can keep the focus on the essence.

## Baseline
This will be the baseline for this demo.