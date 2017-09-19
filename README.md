# Contoso University
Contoso University is a place for learning AspNetCore and related technologies.  This demo application is an amalgamation of smaller demo applications found in tutorials at [AspNetCore docs](https://docs.microsoft.com/en-us/aspnet/core/).  The tutorials are great at demostrating isolated concepts, but issues surfaces when applying these concepts/techniques in a larger context.  The purpose of this demo application is to apply concepts/techniques learned from those tutorial into a single domain (i.e. university).

### Web App
### Rest Api
### Testing
- Unit Testing using [Moq](https://github.com/Moq/moq4/wiki/Quickstart) and [xUnit](https://xunit.github.io/docs/getting-started-dotnet-core)
- Integration Testing using TestHost and InMemoryDatabase
### Security (work in progress)
- using Identity 2.0
- Confirm Email using [SendGrid](sendgrid.com)
- Confirm Phone using [Twilio](https://www.twilio.com/sms/api)
- Two-Factor Authentication - [see tutorial](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/2fa)
### Technologies
- [ASP.NET Core 2.0](https://blogs.msdn.microsoft.com/webdev/2017/08/14/announcing-asp-net-core-2-0/)
- Asp.Net Core Mvc 2.0 / Razor 2.0
- Entity Framework Core 2.0 / Identity 2.0
- Moq
- xUnit
- Twilio
- SendGrid

### Design Patterns
- [Repository](https://social.technet.microsoft.com/wiki/contents/articles/36287.repository-pattern-in-asp-net-core.aspx)
- [Unit Of Work](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/advanced)
