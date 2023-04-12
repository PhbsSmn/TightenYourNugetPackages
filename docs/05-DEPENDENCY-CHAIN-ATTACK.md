# Description
A dependency chain attack is when someone tries to abuse a flaw related to how the build environment fetches code dependencies.
The attack could target your dev, pipeline or production environment depending on what the attacker wants to achieve.

If for instance the attacker could target a package that has access to your DI configuration at startup, they could for instance add a handler that applies to all your contollers which then in turn could grab environment variables, body content, headers and querystrings.
So with the headers this would also mean that the JWT tokens are out in the open.

In this demo I played more nicely and just print that it tried to broadcast you message.

(REWORK THIS TO SHOW THE AWFULL one, it adds more weight to the severeness the original issue was too lightweigth to get things moving internally as well.)

## How to avoid being vulnerable
In this attack we were attacked by a repo that I've added to simulate the nuget.org environment.
The easiest way to protect yourself is to register a prefix on nuget.org. Once this is approved then no one besides you can add new package with that prefix. Just note that already existing packages aren't affected.
There is a good guide available on nuget describing how to register a prefix. Not all prefixes are allowed.

You can easy verify if a package has a reserved prefix or not by looking it up via nuget.org.
[NuGet Gallery | Serilog 2.12.0] https://www.nuget.org/packages/Serilog/2.12.0) only pay attention the not all packages are visible.

Using packages which are having a registered prefix makes those packages already safer if you trust the owner. Keep in mind that those packages in turn can also have dependencies that could also be a potential target for an attacker. So it is not because there's a registered prefix it is safe by definition.

When a package is being restored NuGet will request the package from all its repositories. Then the retrieved ones are merged to 1 flat file list.
This results that you don't know from which source the package is coming from.
So in my case I thought I was resolving my package from my private repository but in fact I was retrieving it from the public one. To resolve this we should use package source mapping. This way NuGet is instructed to resolve package from a certain repository. The most restrictive one will win.

	<?xml version="1.0" encoding="utf-8"?>
	<configuration>
		...
		<packageSourceMapping>
			<packageSource key="PublicPackageSource">
				<clear />
				<package pattern="*" />
			</packageSource>
			<packageSource key="PrivatePackageSource">
				<clear />
				<package pattern="*" />
				<package pattern="DigitalDiary.*" />
			</packageSource>
			<packageSource key="nuget">
				<clear />
				<package pattern="*" />
			</packageSource>
		</packageSourceMapping>
		...
	</configuration>

By introducing this source mapping we instruct NuGet to only retrieve package with the prefix DigitalDiary from the private repository.
Only pay attention that we now need to clear our local cache first otherwise it will still successfully resolve the 1.0.5 version.

And restart the solution, if the force restore is now requested it will resolve the one from our private repo ignoring the one from the public repo.

## What if I detect that I was using a faulty package in my application?
Make sure the erase all local caches where this solution has been build in the company. Most likely you have been breached, I don't speak from experience now but with my investigation I would suggest the following steps:
1. Raise awareness to the people in charge so that they can inform the stakeholders.
2. Clean your local cache and from all the devices where it was build.
3. Make a hotfix for all your environments where the appliaction is running for most of us this is a push your CI/CD server, also here if it was using a shared cache make sure to clean it.
4. Try to know more about the package that was corrupted maybe you find clues what was targeted. If this package came from a public source inform this source about report this package as well.
