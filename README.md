# StarterKit

## The problem
This small repository will be implementing a some of the common feature which every dot net solution generally needs to bootstap the solution. Basic idea is to have common aspects covered within this starter kit. It initially got inspired from the release notes of .NET 5.0 which is scheduled to be released in Nov 2020. It includes swagger as default so i thought maybe why not have few more common features including as well while we expand our learniing horizons.


### Functional requirements
We are asked to develop a StarterKit which would be able to provide the following with minimal line of codes in new solution:
1. Swagger implementation
2. Middleware logger which includes basic info about the request and also implement correlationid which can be traced in distributed multi level micro service architecture.
3. ADFS integration where user can set the ADFS URL and Client Id and it should automatically authenticate and retreive claims/tokens.
4. Publish StarterKit and StarterKit.Logging to nuget feed. Can be manually at the moment and later implement GitHub Actions for automated publishing (CI/CD). (In-Progress, not completed yet)

#### Delivery includes
1. Swagger will be automatically added as soon as we add the StarterKit library/package. Simply start new asp.net core web api/application solution. Navigate to Program.cs and go to method CreateHostBuilder() and add .UseStarterKit() at the end of brackets/braces. See Demo.Management.StarterKit.Sample.Program.cs for reference. This should add swagger. To Add ADFS, Please go to launchSettings.json and set ADFS__Url and ADFS__ClientId 

 "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ADFS__Url": "https://syst-fs1.mydomain.com/adfs",
        "ADFS__ClientId": "xx1233-1234-5678-8901-123456789012"
      }


2. Logging features out of the box. it would be able to log on console. Simply start new asp.net core web api/application solution. Navigate to Startup.cs and go to method ConfigureServices() and add

	var logger = new LoggerConfiguration()
			.WriteTo.Console(new ElasticsearchJsonFormatter())
			.Enrich.FromLogContext()
			.CreateLogger();
			collection.AddLoggingMiddleware(logger);

#### Pre-Requisite:
1. Visual Studio 2019 with common packages installed (eg: .Net Framework / .Net Core 3.1 etc.) and nuget feed should be available.

#### Solution consists of following projects
1. Demo.Management.StarterKit : .Net Core 3.1 Library project responsible to implement Swagger and ADFS.
2. Demo.Management.Starterkit.Logging : .Net Core 3.1 Library project responsible to implement middleware logger and logs some metrics. its using Serilog.
3. Demo.Management.Starterkit.* : Supporting projects for above two and also covers some unit test and integration test.

#### How to build:

1. Clone the project using Visual Studio 2019.
2. Go to menu -> Build -> Build Solution.
3. Go to menu -> Test -> Run All Tests
4. Hit F5 (Run) or go to menu -> Debug -> Start Debugging
  4.1 Please ensure your startup solution is Demo.Management.StarterKit.Sample.
  4.2 If it does not run successfully then it could be due to the fact your ADFS credentials are causing some issue.

#### Design Process
1. Keeping projects as small and independent as possible.
2. Keeping reusable code to be developed using class library so later on we can push them into artifactory as nuget packages and consume directly from nuget packages instead of referencing in project dependency manually.
3. Using Industry best practices to keep the seperation of concerns.
