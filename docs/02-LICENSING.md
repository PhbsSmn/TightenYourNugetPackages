# Description
When creating a package you should add a license to it as well.
The person that consumes your nuget package knows then what the rules are when they do use your package. Using a package without a license could be compared a bit to a blanc contract where you have signed but do not know the rules to play by.
For internal usage however you could opt not to add a license as these are not intended to be shared in the first place.
https://learn.microsoft.com/en-us/nuget/reference/nuspec#license

# Applying a license
Nuget.org only accepts license expressions that are apporved by the Opense Source Initiative or the Free Software Foundation.
A custom license file could be used as well if the list does not suit your needs.

For the demo we will apply an MIT license via a license expression.
This can be set in the project by right clicking on the project -> properties-> package -> license -> Choose SPDX License Expression and then type MIT.

When clicking on the question mark you could find the supported list.

This is now only applied for a single project. So we could repeat this for the other project but.
We could also add it to the Directory.Build.props where it will be applied then automatically for both and if another project is added it will configured immediately as well.