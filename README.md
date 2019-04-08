# .NET Core Instrumentation Middleware 

[![Build status](https://ci.appveyor.com/api/projects/status/uspy2okfos556c9r?svg=true)](https://ci.appveyor.com/project/w0lya/dotnetinstrumentation)

A Middleware that collects request info for .NET Core apps and a monitoring app that displays the gathered request data in real-time.
The gathering and real time display part is handled by [Monitoring App](https://github.com/w0lya/instrumentation-monitoring).
Here you can find the sample web application that has this custom Middleware registered in it, Middleware itself and some tests for it.

## Features of the Middleware
For each web request for the application the Middleware added to, it collects:
- Request processing time
- Request body size
- Execution time
- Request URL

The Logger component that Middleware utilizes internally, posts the gathered data to a pre-configured endpoint (Monitoring app).
As the app is running, it is saving the data into the DB, displaying all the available data, and also displaying some stats: Min, Max and Average response sizes. 

## Prerequisites
1. [.NET Core](https://dotnet.microsoft.com/download)
2. Visual Studio 2015 or higher, or Visual Studio Code with [solution explorer extension](https://marketplace.visualstudio.com/items?itemName=fernandoescolar.vscode-solution-explorer)


## Running locally
1. Clone the repository
2. Open the containing folder in Visual Studio (Code).
3. Restore the packages for each project in the solution (you may use ```dotnet restore``` or UI in Visual Studio). For convenience, you may look up the commands in _appveyor.yml_ file.
4. Launch the app locally on IIS Express.

## Testing
As the app is running, also spin up the [Monitoring app](https://github.com/w0lya/instrumentation-monitoring) to see the updates there.
On the home page of the app, click top navigation links: Home, About, Contact. As you click them, you will see that new entries are appearing in the Monitoring app 

## Tech Stack
.NET Core, ASP.NET Core MVC

## Future improvements
1. Exception handling and logging.
2. More integration and unit tests.
3. Deployment and testing: spin up the apps in containers.
4. Make the app safe against DDOS attacks, e.g. by implementing another piece of Middleware.
5. Add support for multiple applications.
6. Add handling of cases with failing requests (log the required data, e.g. status code, errors, and separate reporting view and stats for these)
7. Test on linux and add notes as needed.

The rest of ideas can be found in the  [Monitoring app] readme.

## Additional Concerns
Concurrency: the Middleware should be safe as it doesn't have shared mutable state in it.
The Monitoring app uses sockets, and sockets queue up

 
