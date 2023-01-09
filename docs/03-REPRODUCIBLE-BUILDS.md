# Description
When using a NuGet package it could happened that an issue occurs inside the package.
When reproducible builds are configured you get a clue from where the source is being build upon. Making it easier to verify what is going on.
If your visual studio is properly configured you could even step into the nuget package code and put breakpoints when it hits a certain point in the external code.

# Applying reproducible builds
For the demo this will be applied in the Directory.Build.props of the nuget projects but I would advise of doing this in your CI/CD pipeline as there are differences between different providers like GitHub or GitLab.
Since the demo is hosted on GitHub this will now be configured via GitHub.

## Install the NuGet package
Choose a project and install the following packages
- DotNet.ReproducibleBuilds
- Microsoft.SourceLink.GitHub

These will already configure a lot of things for you when the package is being build. Since we have 2 NuGet package projects it is easier to move this configuration to the Directory.Build.props

For being able to see the impact of this difference the version numbers will be augmented in the projects
## Specify additional properties
In the Directory.Build.props you can optionally add the following properties
- **PublishRepositoryUrl**: Publish the repository URL in the built .nupkg
- **EmbedUntrackedSources**: Embed source files that are not tracked by the source control manager in the PDB
- **IncludeSymbols**:  Build symbol package (.snupkg) to distribute the PDB containing Source Link
- **SymbolPackageFormat**: The extension name of the symbol package
- **DebugType**: this defaults to portable nuget.org currently only supports this type and gives you the advantage that it works on all OS types.

The last 3 properties will prepare the package to be used by source link. This means that visual studio will then download the source code from the symbol server and your able to step through the code as if it was on your machine. It comes with a hit but once it's retrieved it is cached on you system.

This is not enabled by default.
Tools -> Options -> Debugging -> Symbols:
- This need to point to your symbol servers.
Tools -> Options -> Debugging -> General:
- Enable Just My Code (**unchecked**)
- Enable source server support (**checked**)
- Enable Source Link Support (**checked**)
	- Fall back to Git Credential Manager (**checked**)

(DEMO with build server)