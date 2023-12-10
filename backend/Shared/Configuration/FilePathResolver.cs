using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Shared.Configuration;

public sealed class FilePathResolver(IWebHostEnvironment environment)
{
    public string GetResourceFilePath(string fileName) => ResolvePath($"Resources/{fileName}");

    public string ResolvePath(string relativeRootPath)
    {
        var projectName = Assembly.GetCallingAssembly().GetName().Name;
        var basePath = environment.IsDevelopment() ? $"/src/{projectName}" : $"./{projectName}";
        return Path.Combine(basePath, relativeRootPath);
    }
}
