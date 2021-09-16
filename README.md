# C#/.NET Core with dotnet command
## How to create custom C#/.NET programing task under .NET Core framework with dotnet tool.

**TIP**: Please make sure to read [Getting started with programming tasks](https://help.devskiller.com/creating-tasks-and-tests/getting-started-with-programming-tasks) first.

You can start with our sample project that can be found on GitHub:
[Open sample project](https://github.com/Devskiller/devskiller-sample-dotnetcore)
[Download sample project](https://github.com/Devskiller/devskiller-sample-dotnetcore/archive/master.zip)

## Automatic assessment
It is possible to automatically assess a solution posted by the candidate.
Automatic assessment is based on Tests results and Code Quality measurements. 

All unit tests that are executed during the build will be detected by the Devskiller platform. 

There are two kinds of unit tests:

1. **Candidate tests** - unit tests that are visible for the candidate during the test. These should be used to do only basic verification and are designed to help the candidate understand the requirements. Candidate tests **WILL NOT** be used to calculate the final score.
2. **Verification tests** - unit tests that are hidden from the candidate during the assessment. Files containing verification tests will be added to the project after the candidate finishes the test and will be executed during verification phase. Verification test results **WILL** be used to calculate the final score.
After the candidate finishes the test, our platform builds the project posted by the candidate and executes the verification tests and static code analysis.

## Technical details for .NET Core support
To create automatic assessment, you'll need compilable **.NET Core solution** along with working unit tests. Any language of .NET platform can be used **(C#, F#, VisualBasic)**, though this article focus on c# only. Currently Devskiller platform supports .NET Core version: *1.0-2.2.1*
`Dotnet command` will be used to build, restore packages and test your code. You can use any unit-testing framework like **NUnit, XUnit or MSTest**. 
Please refer to [msdn](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test) for details about using testing frameworks.
Don't forget to add reference to `Microsoft.NET.Test.Sdk` in your test projects.


## Preparing solution for automatic tests
To prepare your solution for automatic assessment you should follow those 3 steps:

### 1. Prepare separate project in your solution for verification tests.
This project should reside in separate folder in the repository. The folder structure could look like this:

```
CalculatorTask
│   .gitignore
│   README.md
│   devskiller.json
│   CalculatorSample.sln   
│   
└───CalculatorSample
│      CalculatorSample.csproj
│      Calculator.cs
│   
└───CalculatorSample.Tests
│      CalculatorSample.Tests.csproj
│      Tests.cs
│   
└───CalculatorSample.VerifyTests
       CalculatorSample.sln
       CalculatorSample.VerifyTests.csproj
       VerifyTests.cs	
```

The **CalculatorTask\CalculatorSample.VerifyTests** folder from example above, contains the *.csproj and code file for verification tests that will be invisible for candidate. Please note there is also additional *.sln file in this folder - we will get back to it later.

### 2. Prepare devskiller.json file - Devskiller project descriptor. 
Programming task can be configured with the Devskiller project descriptor file. Just create a `devskiller.json` file and place it in the root directory of your project. Here is an example project descriptor:
```
{
  "readOnlyFiles" : [ "CalculatorSample.sln" ],
  "verification" : {
    "testNamePatterns" : [".*VerifyTests.*"],
    "pathPatterns" : ["CalculatorSample.VerifyTests/**"],
    "overwrite" : {
	 "CalculatorSample.VerifyTests/CalculatorSample.sln" : "CalculatorSample.sln"
    }
  }
}
```
You can find more details about devskiller.json descriptor in our [documentation](https://help.devskiller.com/creating-tasks-and-tests/using-custom-programming-tasks/programming-task-project-descriptor)

In example above, by setting `readOnlyFiles` field with a solution file, we make sure candidate won't be able to edit it. **It's important during phase of verification tests execution, don't forget to add it!**
- `testNamePatterns` - an array of RegEx patterns which should match all the test names of verification tests. Test names should contain: `[namespace_name].[Class_name].[method_name]` . In our sample project, all verification tests are inside VerifyTests  class, so the following pattern will be sufficient:
```
"testNamePatterns"  : [".*VerifyTests.*"]
```
- `pathPatterns` - an array of GLOB patterns which should match all the files containing verification tests. All the files that match defined patterns will be deleted from candidates projects and will be added to the projects during the verification phase. 
```
"pathPatterns" : ["CalculatorSample.VerifyTests/**"]
```

Because files with verification tests will be deleted from candidates projects, you need to make sure, that during final solution build, Devskiller platform will be aware of them.
To make that happen, you must point which solution file should be overwritten - you want solution from **CalculatorTask\CalculatorSample.VerifyTests** folder to overwrite the **root** solution file
```
"CalculatorSample.VerifyTests/CalculatorSample.sln" : "CalculatorSample.sln"
```
So the last thing is to prepare proper solution files:


### 3. Preparing two solution files.

You need *two* solution files. 
One in root folder, this is the solution file that will be used by the candidate. It should have project structure that the candidate should see, so there should be no verification test project there:

`CalculatorTask\CalculatorSample.sln` solution structure:
```
Solution 'CalculatorSample'
│   
└───CalculatorSample
│   
└───CalculatorSample.Tests
```

Second one is the 'temporary' solution file residing in verification tests folder. **During final testing it will override the root solution file** and thanks to that, test platform will be aware of existence of `CalculatorSample.VerifyTests.csproj` in solution

`CalculatorSample.VerifyTests\CalculatorSample.sln` solution structure:
```
Solution 'CalculatorSample'
│   
└───CalculatorSample
│   
└───CalculatorSample.Tests
│   
└───CalculatorSample.VerifyTests
```

Easiest way to have those two *.sln files is to prepare solution with verification tests, copy it to verification tests folder, than go back to root solution, remove the verification tests project and save it.


## Hints

1. Remember that you aren't bounded to unit tests. You can do some integration tests, in example .net core mvc apps are easy to instantiate within single test scope, try to use that.
2. Make sure, each test is self-runnable and independent of external components like: native system libraries, external databases, etc.
3. Avoid using external libraries in your source. You will never know on what operating system your code will be executed. If you need some external libraries please reference them as NuGet packages. This will make sure, you're code will behave on Devskiller platform in the same way it behaves on your environment.
4. When needed and applicable, consider usage of in-memory database engine emulation when aplicable. It's fast to run and easy to use.
5. Don't forget that you can utilize .NET framework references in .NET Core projects if needed - do it with caution though (maybe only in verification tests? - its up to you), you want to test candidate against .NET Core... don't you? Otherwise consider using [MSBuild](https://help.devskiller.com/creating-tasks-and-tests/using-custom-programming-tasks/cnet-with-msbuild)
6. Try to make test names clear and as short as possible. Long names will be harder to read for recruiters and can be confusing. Some test-parameter-injection methods tend to generate complex text output when executing tests. Make sure to check, if the test output looks good.
7. Remember to describe clearly the task instructions in `README.md` file.
8. When leaving gaps in code to be filled by candidate, consider throwing `NotImplementedException`, rather than in ex. returning `null`, `false`, `0`, etc. in your methods. As those returned values, dependent on logic, could be some edge cases, besides, the unit tests execution will instantly fail even before checking assertions, so the candidadte will be 100% sure where he is expected to make code changes.
10. You want candidate to start working on the task as soon as possible, not to struggle with configuration. Project delivered for candidate should be compilable and working without any configuration needed. It should only fail tests on execution.
