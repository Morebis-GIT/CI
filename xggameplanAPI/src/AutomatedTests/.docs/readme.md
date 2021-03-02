# Note on the test project

The **AutomationTests** project is designed to run under .NET Framework 4.7.2 using the _dotnet test_ command. In order to have it do that it was created as a .NET Core project targeting _net472_. Look in the project file.

This will future proof the project as it can simply have further targets added as required. It runs successfully (so far) under

* the _dotnet test_ command
* NCrunch
* Visual Studio 2017 NUnit test runner

## SpecFlow configuration

<https://specflow.org/documentation/Generate-Tests-from-MsBuild/>

The SpecFlow code behind files (*.feature.cs) are generated on building.
