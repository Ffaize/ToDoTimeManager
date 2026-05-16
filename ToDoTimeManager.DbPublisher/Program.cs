using System.Diagnostics;
using Microsoft.SqlServer.Dac;

// DbPublisher lives at: solution/ToDoTimeManager.DbPublisher/bin/Debug/net10.0/
// Four levels up → solution root
var solutionRoot = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

var dbProjectDir  = Path.Combine(solutionRoot, "ToDoTimeManager.DataBase");
var sqlprojPath   = Path.Combine(dbProjectDir, "ToDoTimeManager.DataBase.sqlproj");
var publishXmlPath = Path.Combine(dbProjectDir, "ToDoTimeManager.DataBase.publish.xml");

foreach (var required in new[] { sqlprojPath, publishXmlPath })
{
    if (!File.Exists(required))
    {
        Console.Error.WriteLine($"Required file not found: {required}");
        return 1;
    }
}

// --- Find MSBuild via vswhere ---
var vswhere = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
    "Microsoft Visual Studio", "Installer", "vswhere.exe");

if (!File.Exists(vswhere))
{
    Console.Error.WriteLine("vswhere.exe not found — Visual Studio is required to build SSDT projects.");
    return 1;
}

Console.WriteLine("Locating MSBuild...");
var (vswhereExit, vswhereOut, _) = Run(vswhere,
    "-latest -requires Microsoft.Component.MSBuild -find MSBuild\\**\\Bin\\MSBuild.exe");

var msbuildPath = vswhereOut
    .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
    .LastOrDefault()?.Trim();

if (string.IsNullOrEmpty(msbuildPath) || !File.Exists(msbuildPath))
{
    Console.Error.WriteLine("MSBuild.exe not found.");
    return 1;
}

// --- Build dacpac ---
Console.WriteLine($"Building {Path.GetFileName(sqlprojPath)}...");
var (buildExit, _, buildErr) = Run(msbuildPath,
    $"\"{sqlprojPath}\" /t:Build /p:Configuration=Debug /v:minimal /nologo");

if (buildExit != 0)
{
    Console.Error.WriteLine($"MSBuild failed (exit {buildExit}):\n{buildErr}");
    return 1;
}

var dacpacPath = Path.Combine(dbProjectDir, "bin", "Debug", "ToDoTimeManager.DataBase.dacpac");
if (!File.Exists(dacpacPath))
{
    Console.Error.WriteLine($"Dacpac not found after build: {dacpacPath}");
    return 1;
}

// --- Publish via profile ---
Console.WriteLine($"Loading publish profile: {publishXmlPath}");
var profile = DacProfile.Load(publishXmlPath);

var dacServices = new DacServices(profile.TargetConnectionString);
dacServices.Message += (_, e) => Console.WriteLine(e.Message);
dacServices.ProgressChanged += (_, e) => Console.WriteLine($"[{e.Status}] {e.Message}");

using var dacPackage = DacPackage.Load(dacpacPath);

// --- Check for schema differences before publishing ---
Console.WriteLine($"Checking '{profile.TargetDatabaseName}' for schema differences...");

try
{
    var deployReport = dacServices.GenerateDeployReport(
        dacPackage,
        targetDatabaseName: profile.TargetDatabaseName,
        options: profile.DeployOptions);

    var doc = System.Xml.Linq.XDocument.Parse(deployReport);
    var ns = doc.Root?.Name.Namespace ?? System.Xml.Linq.XNamespace.None;
    var operationCount = doc.Descendants(ns + "Operation").Count();

    if (operationCount == 0)
    {
        Console.WriteLine("Database is already up to date. Skipping publish.");
        return 0;
    }

    Console.WriteLine($"Detected {operationCount} pending schema operation(s). Publishing...");
}
catch (Exception ex)
{
    Console.WriteLine($"Schema comparison failed ({ex.Message}). Proceeding with publish to be safe.");
}

dacServices.Deploy(dacPackage, targetDatabaseName: profile.TargetDatabaseName,
    upgradeExisting: true, options: profile.DeployOptions);

Console.WriteLine("Database published successfully.");

return 0;

static (int ExitCode, string StdOut, string StdErr) Run(string fileName, string arguments)
{
    using var process = Process.Start(new ProcessStartInfo(fileName, arguments)
    {
        RedirectStandardOutput = true,
        RedirectStandardError  = true,
        UseShellExecute        = false,
    })!;
    var stdoutTask = process.StandardOutput.ReadToEndAsync();
    var stderrTask = process.StandardError.ReadToEndAsync();
    process.WaitForExit();
    return (process.ExitCode, stdoutTask.Result, stderrTask.Result);
}
