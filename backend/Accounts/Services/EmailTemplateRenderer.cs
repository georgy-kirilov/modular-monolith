using HandlebarsDotNet;
using Shared.Configuration;

namespace Accounts.Services;

public sealed class EmailTemplateRenderer(FilePathResolver _filePathResolver)
{
    public async Task<string> RenderAsync(string templateName, Dictionary<string, object> parameters)
    {
        var path = _filePathResolver.GetResourceFilePath($"{templateName}.html");
        var templateContent = await File.ReadAllTextAsync(path);
        var compiledTemplate = Handlebars.Compile(templateContent);
        var html = compiledTemplate.Invoke(parameters);
        return html;
    }
}
