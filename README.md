# My Digital Diary
This is a demo project used for demonstrating some functionality that is available with Nuget repositories. This isn't a bleeding edge demo. But for some reason I never saw these techniques in my projects or others and didn't knew of them until I followed the session of Claire Novotny at Techorama.
The demo project is a digital diary where you can write some things to in memory. This was done deliberately, to avoid any biased opinions on a certain implementation of naming on a topic someone is working on.

Some MSbuild tasks where added to simulate the CI/CD pipeline that you should use instead of this, for the demo's purpose this independence was actually needed.

## Prerequisites
In order to be able to run this project you need Visual Studio 2022 with .net6 & .net7.
Concerning .net7 it was only added to demonstrate that all these things support multi target frameworks. If you want to play with this preview go to Tools-> Options -> Preview Features -> And enable the checkbox "**Use previews of the .NET SDK (requires restart)**"
## Project structure
Everything is created with relative paths, in order to start this project make sure that the following folders are present, if not create them, and empty:
- MyDigitalDiary/Nuget/GlobalPackagesFolder, this folder is the local cache, for this demo it's better not to taint the local one on you machine and for the last part it's actually important that this is a separate folder.
- MyDigitalDiary/Nuget/PrivatePackageSource, this represent the firms internal Nuget repository
- MyDigitalDiary/Nuget/PublicPackageSource, this represent the public nuget.org repository, although that one is also in this project it wasn't my desire to publish any packages to that source.
### .solution items
The only important file here to mention is the Directory.Build.Props, both the client and nuget projects will consume this and extend their project with it.
### Nuget
#### DigitalDiary.Abstractions
Nothing special it holds the models and interfaces
#### DigitalDiary.MemoryRepository
This is a very simple repository, consuming the interface.
#### DigitalDiary.MemoryRepositoryInjected
This is a project that demonstrates a project written with ill intent that could inject itself into your chain if you don't pay attention.
Therefor that the elements *AssemblyName* & *RootNamespace* are changing the end result of this project.
### Client
The client project will be used for demonstrating results, the code is just like in the other files irrelevant.
## Demo
### Directory.Build (.props & .targets)
In the root of the project and in the nuget src folder a **Directory.Build.props** & **Directory.Build.targets** was added these names are case sensitive when building on a linux environment. The purpose of these files is that whenever a project is added under the src of the Nuget I don't need to add these settings manually. This reduces a lot of copy/pasting issues and makes sure that the global settings are set just once and none are forgotten. If for some reason a setting needs to be overwritten you can still do this.

This is done also in this project => "*DigitalDiary.MemoryRepositoryInjected*". In this project a deviation was required in the Target **CopyPackage**

The difference between the props & targets is that some properties aren't known upfront. Like AssemblyName, is not known upfront because it can be altered by defining the AssemblyName element in the csproj file.

Note that the Directory.Build.props under the Nuget references to the one in the root. This is because once a project find a Directory.Build.props file it stops navigating up to the root of the system searching for one.
### License
When creating a package you should always add a license to it as well so that the one consuming it knows what the rules are when they do use your package. This can be set in the project by right clicking on the project -> properties-> package -> license -> Choose SPDX License Expression and then the one that suits you in this case I chose MIT
### Reproducable builds
When this is configured it makes it easier to use your package not just as a black box, this can be configured in by adding the following packages:
- DotNet.ReproducibleBuilds
- Microsoft.SourceLink.GitLab, this is an addition to indicate that our source can be downloaded from that location, it also requires a SourceLinkGitLabHost to work properly.

The following properties in the PropertyGroup must be set as well:
- EmbedUntrackedSources
- PublishRepositoryUrl
- IncludeSymbols
- SymbolPackageFormat
- DebugType
- ContinuousIntegrationBuild, only needed for the demo otherwise this checkbox wouldn't been hit

When the DigitalDiary.MemoryRepository  is built the nuget packages will be added to the PrivatePackageSource path. Now the DigitalDiary.WinForm project should reference this package so that we can consume it.
### Dependency resolution
There are multiple ways that Nuget can resolve their dependencies. Currently I always saw the major, minor, revision way which is the default.

But these ranges can be used as well

| Notation | Applied rule | Description |
|--|--|--|
| 1.0 | x ≥ 1.0 | Minimum version, inclusive|
|(1.0,)|x > 1.0|Minimum version, exclusive|
|[1.0]|x == 1.0|Exact version match|
|(,1.0]|x ≤ 1.0|Maximum version, inclusive|
|(,1.0)|x < 1.0|Maximum version, exclusive|
|[1.0,2.0]|1.0 ≤ x ≤ 2.0|Exact range, inclusive|
|(1.0,2.0)|1.0 < x < 2.0|Exact range, exclusive|
|[1.0,2.0)|1.0 ≤ x < 2.0|Mixed inclusive minimum and exclusive maximum version|
|(1.0)|invalid|invalid|
|1.*|1.0 ≤ x < 2.0| Floating version, uses the latest version, this differentiates it from the other as they always use the lowest one.

***(Make sure the injected project is build now)***

With this new knowledge the WinForm can be updated using the 1.* notation, this way when a new nuget package is added the project is automatically updated as well.

When this is applied the package version goes immediately to version 1.0.1 instead of 1.0.0. Let's try this again maybe the bug is resolved.
This isn't what we've expected to happen to begin with and now for this app it would mean that all your private memoires are out on the street for everyone to read.
### Securing the dependency chain
#### Create a lock file
Remove the 1.0.1 DigitalDiary.MemoryRepository from the global repo, and make sure the reference is set to 1.0.0 and correct it afterwards again to the floating version 1.*

In the csproj file add in the PropertyGroup the following element, also make sure to set the 1.0.0 reference in place again **before saving** this

    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
When this is set to true a packages.lock.json file will be created. When the injected packages is made available again the repository isn't added to the project although the 1.* floating version was applied to the references. However this will not help when the user manually updates the version in the package then the 1.0.1 version will still be applied.

We'll remove the lock file and set the creation of it back to false.
#### Package source mapping
Another approach is giving hints to the nuget manager to indicate where it should obtain it's resources. In the nuget.config an extra element can be used called **PackageSourceMapping**.
Now the rules are changing as with the prepared configuration we're not taking items anymore from the public or nuget repository that has a *DigitialDiary.* prefix, the most restrictive namespace dictates where the nuget package must come from.
One remark to be added if the repo is already in the globalpackages folder then this rule will be ignored. If we now try to apply the 1.0.1 version again it will be actively refused by visual studio.
#### Package signing
It could even go more stricter but with the same restriction if the package is known in the globalpackages folder the signature is ignored.
For the demo all the packages in the private repo are being signed with a self signed certificate (this will not work in nuget) but for a private repo it should be ok.
To enable this in the nuget.config uncomment the signatureValidationMode and the trustedSigners elements.

Via powershell create a self signed certificate that can be used for signing packages:

    New-SelfSignedCertificate -Subject "CN=Local Dev Cooper,O=For local dev purposes, OU=Use for testing purposes ONLY" `
                              -FriendlyName "LocalDevCooper" `
                              -Type CodeSigning `
                              -KeyUsage DigitalSignature `
                              -KeyLength 2048 `
                              -KeyAlgorithm RSA `
                              -HashAlgorithm SHA256 `
                              -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider" `
                              -CertStoreLocation "Cert:\CurrentUser\My"

To retrieve the fingerprints this call should do it:

    dotnet nuget verify --all digitaldiary.abstractions.1.0.0.nupkg

This will result in something like this:

> Verifying DigitalDiary.Abstractions.1.0.0
> 
> Signature type: Author   Subject Name: CN=**Local Dev Cooper**, O=For
> local dev purposes, OU=Use for testing purposes ONLY   SHA256 hash:
> **3B62049B6B17D2B2CC08F3165BB4158F4BD870BD3C6E504C5FFDB3641CF5508C**  
> Valid from: 05-06-2022 22:46:00 to 05-06-2023 23:06:00

The only downside on this approach that it affects all the packages so we can't turn this option on only for our own repo but in order to keep everything working from scratch the .NET Foundation, Microsoft Corporation & nuget.org also needed to be resolved.
But also know the injected repo isn't allowed anymore even with the source mapping being removed.
But also if the nuget package found it's way into our repo it will now be refused actively.
And if a newer package is being added then it will allowed automatically if floating versions are used.