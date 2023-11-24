///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var stable = Argument("stable", "false");

var srcProjects  = GetFiles("./src/**/*.csproj");
var packProjects = GetFiles("./src/**/*.csproj");
var testProjects  = GetFiles("./test/**/*.csproj");

var artifacts = "./artifacts/packages";
var branchName = EnvironmentVariable("BUILD_SOURCEBRANCHNAME") ?? "local";

void PrintBuildInfo(ICakeContext context){
   Information($@"branch:{branchName}, agentOs={EnvironmentVariable("Agent_OS")},Platform: {context.Environment.Platform.Family}, IsUnix: {context.Environment.Platform.IsUnix()}
   BuildID:{EnvironmentVariable("BUILD_BUILDID")},BuildNumber:{EnvironmentVariable("BUILD_BUILDNUMBER")},BuildReason:{EnvironmentVariable("BUILD_REASON")}
   ");
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
   PrintBuildInfo(ctx);
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("clean")
    .Description("Clean")
    .Does(() =>
    {
       var deleteSetting = new DeleteDirectorySettings()
       {
          Force = true,
          Recursive = true
       };
      if (DirectoryExists(artifacts))
      {
         DeleteDirectory(artifacts, deleteSetting);
      }
    });

Task("restore")
    .Description("Restore")
    .Does(() => 
    {
      foreach(var project in srcProjects)
      {
         DotNetRestore(project.FullPath);
      }
    });

Task("build")    
    .Description("Build")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .Does(() =>
    {
      var buildSetting = new DotNetBuildSettings{
         NoRestore = true,
         Configuration = configuration
      };
      foreach(var project in srcProjects)
      {
         DotNetBuild(project.FullPath, buildSetting);
      }
    });

Task("test")    
    .Description("Test")
    .IsDependentOn("build")
    .Does(() =>
    {
      var testSettings = new DotNetTestSettings{
         NoRestore = false,
         Configuration = configuration
      };
      foreach(var project in testProjects)
      {
         DotNetTest(project.FullPath, testSettings);
      }
    });

Task("pack")
    .Description("Pack package")
    .IsDependentOn("test")
    .Does((ctx) =>
    {
      var settings = new DotNetPackSettings
      {
         Configuration = configuration,
         OutputDirectory = artifacts,
         VersionSuffix = "",
         NoRestore = true,
         NoBuild = true
      };
      if(branchName != "master" && stable != "true"){
         settings.VersionSuffix = $"preview-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
      }
      foreach (var project in packProjects)
      {
         DotNetPack(project.FullPath, settings);
      }
      PublishArtifacts(ctx);
    });

bool PublishArtifacts(ICakeContext context)
{
   if(context.Environment.Platform.IsUnix())
   {
      return false;
   }
   if(branchName == "master" || branchName == "preview")
   {
      var pushSetting =new DotNetNuGetPushSettings
      {
         Source = EnvironmentVariable("Nuget__SourceUrl") ?? "https://api.nuget.org/v3/index.json",
         ApiKey = EnvironmentVariable("Nuget__ApiKey")
      };
      var packages = GetFiles($"{artifacts}/*.nupkg");
      foreach(var package in packages)
      {
         DotNetNuGetPush(package.FullPath, pushSetting);
      }
      return true;
   }
   return false;
}

Task("Default")
    .IsDependentOn("pack");

RunTarget(target);