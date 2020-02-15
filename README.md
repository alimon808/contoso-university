# Contoso University [![Build Status](https://travis-ci.org/alimon808/contoso-university.svg?branch=master)](https://travis-ci.org/alimon808/contoso-university)
Contoso University is a place for learning AspNetCore and related technologies.  This demo application is an amalgamation of smaller demo applications found in tutorials at [AspNetCore docs](https://docs.microsoft.com/en-us/aspnet/core/).  The tutorials are great at demonstrating isolated concepts, but issues surfaces when applying these concepts/techniques in a larger context.  The purpose of this demo application is to apply concepts/techniques learned from those tutorial into a single domain (i.e. university).

### ContosoUniversity.Web
- Traditional Web App using MVC + Razor Pages
- [Demo](http://contoso-university-web.adrianlimon.com)
### ContosoUniversity.Api
- Traditional Rest Api
- [Demo](http://contoso-university-api.adrianlimon.com/)
- Generate JWT Token at http://contoso-university-web.adrianlimon.com/api/token to access secure api content.  Requires registering via Web App.
### Testing
- Unit Testing using [Moq](https://github.com/Moq/moq4/wiki/Quickstart) and [xUnit](https://xunit.github.io/docs/getting-started-dotnet-core)
- Integration Testing using TestHost and InMemoryDatabase
- UI Testing using Selenium
### Security
- using Identity 2.0
- Confirm Email using [SendGrid](sendgrid.com)
- Confirm Phone using [Twilio](https://www.twilio.com/sms/api)
- Two-Factor Authentication - [see tutorial](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/2fa)
- OAuth 2 - Enable Google & Facebook logins
- JWT (Json Web Token) - use to access secure API
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
