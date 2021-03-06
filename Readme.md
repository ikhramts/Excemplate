Excemplate
==========

Overview
--------

Excemplate is a simple language (and a processing system to go along with it) that allows one to create Excel document templates directly in Excel spreadsheets.

Using Excemplate, we could write in one of the cells of an Excel file

    |getEmployeeName(32)
	
where `getEmployeeName` is a function defined either in a specific .NET library or in an F# fsx script.  This function could load the employee name from the database, or cound to generate a value on the fly, or it might do some sophisticated processing that is limited only by capabilities of .NET platform.  

We can then evaluate this newly created template using a command line interface

    excemplate templateFile.xls -h Handler.dll -o prettyReportFile.xls
	
which will place a value returned by `getEmployeeName` function into the cell containing an Excemplate expression.

Alternatively, we can use a GUI (depending on whether it will be implemented), or incorporate Excemplate in a larger .NET system by using Excemplate.Core.dll:

    using Excemplate.Core
	
	//...
	
	ExcemplateProcessor.Process(@"C:\Path\To\File.xlsx");
	
Building Excemplate
-------------------

Excemplate solution targets Visual Studio 2012.  The solution depends on

* [PowerPack for F# 3.0](https://fsharppowerpack.codeplex.com/releases/view/99231) for parsing the Excemplate expressions.  F# PowerPack can be downloaded [here](https://fsharppowerpack.codeplex.com/releases/view/99231).  After installing it, add `C:\Program Files (x86)\FSharpPowerPack-4.0.0.0\bin` or equivalent path to system `PATH`.
* [NUnit](http://www.nunit.org/) for testing.

Once these two are installed, build the Excemplate solution.

Documentation
-------------

None at the moment.

To Do
-----

This section contains things that will be skipped for the first release, but may be implemented later.

**Build System:**

* Make fslex and fsyacc run only if the source files have changed.

**Script Processor:**

* Allow to not provide a function handler by default.
* Add quote escaping in string literals.
* Add ordered (i.e. not named) function arguments.
* Maybe emit some debug info, so that some exceptions don't get swallowed completely silently?
* Maybe modify the abstract syntax tree evaluator to be tail-recursive?  This is a very low priority task, I do not foresee anyone writing stack-blowing expressions in Excel cells.
* Add ability to handle arrays.
* Add ability to handle cell references.

**ExcelManager:**
* Make ExcelManager into an IDisposable, and make it dispose of Excel when the process crashes.
* Rename ExcelManager.Stop() to ExcelManager.Close();


