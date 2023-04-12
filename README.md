## Demo

### Securing the dependency chain
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