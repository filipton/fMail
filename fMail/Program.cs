using Microsoft.AspNetCore.Mvc;

namespace fMail;

class Program
{
    public static async Task Main(string[] args)
    {
        await ConfigParser.LoadConfig();

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        
        var app = builder.Build();

        // app.UseResponseCaching();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapControllers();
        
        await app.RunAsync();    
    }
}