# Description
Ok now the package must come from 1 source that we dictate, if you have a internal nuget source the it's advised to sign the packages as well.
This could be with a self signed certificate but it is better to have a proper signing certificate. When you are publishing a package to the internal nuget source it should be signed.
Normally the CI/CD pipeline should take care of this step and it should be the only allowed path to put new internal packages to your interna nuget source.

This way if someone is finding a loophole and puts a non signed package to your internal nuget source you can bounce this package.
With this in mind when someone would download a package from your internal nuget source and tampers it. Visual studio will detect this and throw a warning, rejecting the package as well.