# rent-a-car
Web App for Rent a Car firm

### [Working example](https://robi-web.ddns.net/)

## Technology

 - This web app was created in .NET MVC 5 and EF 6 using Code First migration.
 - Tool used for creating this app is Visual Studio 2015
 
## Start Project

To start prject after importing it into Visual Studio, run next commands in *Package Manager Console* (*View -> Other Windows -> Package Manager Console*):

`Update-Database -ConfigurationTypeName RentACar.Migrations.RcDbContext.Configuration`<br/>
`Update-Database -ConfigurationTypeName RentACar.Migrations.UserDbContext.Configuration`<br/>

## Database

EF6 supports multiple DB contexts so this app uses two. There can be one or two databases for one app depending on security needs. 
 - First one whose connection in called *LocalConnection* is used for storing rent a car data (vehicles, orders, car equipment etc.). Context for this is named *RcDbContext*
 - Second one whose connection in called *UsersConnection* is used for storing users data (registration info, empleyees, roles etc.)
  Context for this is named *UserDbContext*
 
 This project has two database contexts *RcDbContext* and *UserDbContext*. To create and migrate databases schemas, next commands must be executed in *Package Manager Console* (*View -> Other Windows -> Package Manager Console*)
 
 1. Enable migration for each DB context <br/>
`PM> Enable-Migrations -ContextTypeName RcDbContext -MigrationsDirectory Migrations\RcDbContext` <br/>
`PM> Enable-Migrations -ContextTypeName UserDbContext -MigrationsDirectory Migrations\UserDbContext` <br/>

2. Add migration for each DB context <br/>
`Add-Migration -ConfigurationTypeName RentACar.Migrations.RcDbContext.Configuration "<Schema Migration Name>"`<br/>
`Add-Migration -ConfigurationTypeName RentACar.Migrations.UserDbContext.Configuration "<Schema Migration Name>"`<br/>

3. Update database with schema changes for each DB context <br/>
`Update-Database -ConfigurationTypeName RentACar.Migrations.RcDbContext.Configuration`<br/>
`Update-Database -ConfigurationTypeName RentACar.Migrations.UserDbContext.Configuration`<br/>

In _Web.config_ between tags *\<connectionStrings>\</connectionStrings>* are two connection string for *Local DB*

`<connectionStrings>`<br/> 

    `<add name="LocalConnection" connectionString="Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\RentACar.mdf;Initial Catalog=RentACar;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" providerName="System.Data.SqlClient"/>`

    `<add name="UsersConnection" connectionString="Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\RentACar.mdf;Initial Catalog=RentACar;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" providerName="System.Data.SqlClient"/>`
 
`</connectionStrings>`

