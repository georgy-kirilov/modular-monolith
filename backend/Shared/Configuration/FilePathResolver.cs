using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Shared.Configuration;

public sealed class FilePathResolver(IWebHostEnvironment environment)
{
    public string GetResourceFilePath(string fileName)
    {
        var projectName = Assembly.GetCallingAssembly().GetName().Name;
        var basePath = environment.IsDevelopment() ? $"/src/{projectName}/{projectName}/Resources" : $"./{projectName}/{projectName}/Resources";
        return Path.Combine(basePath, fileName);
    }

    public string ResolvePath(string relativeRootPath)
    {
        var projectName = Assembly.GetCallingAssembly().GetName().Name;
        var basePath = environment.IsDevelopment() ? $"/src/{projectName}/{projectName}" : $"./{projectName}/{projectName}";
        return Path.Combine(basePath, relativeRootPath);
    }
}
