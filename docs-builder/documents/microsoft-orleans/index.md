# Authorization functionality for Microsoft Orleans

The set of security packages for Microsoft Orleans 3.* provide the ability to use the same authorization functionality which is used in ASP.NET Core 
It allows to use Azure Active Directory or IdentityServer4 v4.0 with MS Orleans grains and any ASP.NET Core 3.* or 5.* application.

![Image 1](index1.jpg)

The image below shows a round trip to the protected grain.

![Image 2](index2.jpg)

[Get started now](#getting-started) [View it on GitHub](https://github.com/Async-Hub/AuthZI)

---

## Getting started
### Dependencies
There are two packages: *Orleans.Security.Cluster* and *Orleans.Security.ClusterClient*. The first is for a silo host project and the seccond for an Orleans cluster client.

### Quick start

Please [see sample](https://github.com/Async-Hub/AuthZI-Samples) solutions for more details.

---

### License

AuthZI is distributed by an MIT license.

### Contributing

Contributions are welcome, please contact [via Azure DevOps](https://dev.azure.com/async-hub/AuthZI/_workitems/recentlyupdated/), email <admin@asynchub.org>, or any other method with the owners of this repository.
