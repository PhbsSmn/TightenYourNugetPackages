# Description
There are multiple ways how NuGet can resolve its dependencies.
By default I most commonly see major, minor, revision way which is the default.

**These ranges can be used as well**
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

All the notations will also fallback to oldest NuGet package it can find, except for the floating versions. The latter makes it it great for automated updates without lifting a finger.
I wouldn't advise using floating versions without these constraints being set:
- You should be in control when to float and when not otherwise you could have some strange situtations in your pipeline.
e.g.: a merge request is created in the pipeline with a package having version 1.0.0 and merged to your main or master branch when it was still 1.0.0 but the floating version is being set to version 1.*.
After a few weeks all the merge requests are being pushed to a staging or production environment and everything is being rebuild. Everything merges properly to the environment but when external people start working on it nothing works. I can assure you that it will take a while that you see that the specific package is no longer version 1.0.0 but it has floated to 1.1.0 for instance and that this one contains a bug for the method your using. This is no fairy tale, it happened to me already.

## Repeatable package restores using a lock file
We can stay in charge of which NuGet package that gets restored by introducing lock files.
There are a few reasons why NuGet would restore a different file
1. **Nuget.Config** mismatch, lets hope you don't encounter this as it will lead to an inconsistent set of package repositories.
2. **Intermediate versions**, you specify version 1.0.0 but it can only find version 1.1.0 
3. **Package deletion**, nuget.org doesn't allow this but other repositories don't always have this constraint.
4. **Floating versions**, like mentioned in the example you start with version 1.* which resolve to version 1.0.0 today but tomorrow an update happened on the repository and now the newest is 1.0.1.
5. **Package content mismatch**, the same package exists on 2 different repositories but they have different content. When not backed up by a lock file you get the quickest responder.

## Update the WebApi to use floating versions
    <Project Sdk="Microsoft.NET.Sdk">
		<ItemGroup>
			<PackageReference Include="DigitalDiary.MemoryRepository" Version="1.*" />
			<PackageReference Include="Swashbuckle.AspNetCore" Version="6.*" />
		</ItemGroup>
    </Project>

## Applying a lock file
Add this to the *MyDigitalDiary.WebApi*

    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
    	  ...
    	  <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
    	  ...
      </PropertyGroup>
    </Project>
When this is added to the property group it will create for each project a packages.lock.json file automatically for you. Don't update this file manually.
Pay attention that in visual studio when you right click and request a "Restore NuGet Packages" you will not get the latest version but the one that is defined inside the lock file if it is present.
Ok this is all nice and well but if your using floating versions you should feel that this is putting you back to the default versioning system. So why bother doing this effort of floating with locking them?
With the Command Task Runner Extension you can create an automated task resolving to the latest versions ignoring the current, I initially tried to find a solution with MSBuild but I couldn't find one.

	{
	  "commands": {
	    "UpdateLock": {
	      "fileName": "dotnet",
	      "workingDirectory": ".",
	      "arguments": "restore --force-evaluate"
	    },
	    "GitAcceptAllPackageLocks": {
	      "fileName": "git",
	      "workingDirectory": ".",
	      "arguments": "add */packages.lock.json"
	    }
	  }, "-vs-binding":{"ProjectOpened":["UpdateLock"]}
	}
The first command will enable you to restore nuget package ignore the locks, and with a floating version that is exactly what you want. When the packages are restored the lock files are updated automatically.

The second command will help you when your are working in a team to quickly resolve the merge conflicts on the lock files. This command will just quickly accept all the locks, it is then up to the developer to verify if the code is still working properly after the merge. If not just double click on the UpdateLock and most likely the lock files will be updated to the correct and latest versions if your using floating versions.

## Lock file in a CI/CD pipeline
When running a restore in a CI/CD pipeline make sure you add this.

    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
    	  ...
    	  <RestoreLockedMode Condition="'$(ContinuousIntegrationBuild)' == 'true'">true</RestoreLockedMode>
    	  ...
      </PropertyGroup>
    </Project>
Pay attention to the condition attribute, don't add this to your local development environment without it or prepare for failing nuget restores. Another thing you should keep in mind is your target runtime. Nowadays we can develop on Windows but run our code on Linux. If this is the case the property RuntimeIdentifiers also needs to be added

    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
    	  ...
    	  <RuntimeIdentifiers>linux-x64</RuntimeIdentifiers>
    	  ...
      </PropertyGroup>
    </Project>

## Summary
This already feels like a good implementation but there is still a big security concern here that should be taken care of.
If neglected sooner or later someone will try to abuse your code base for personal gain.