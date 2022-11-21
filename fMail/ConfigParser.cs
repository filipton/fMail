using System.Diagnostics;
using Newtonsoft.Json;

namespace fMail;

public class ConfigParser
{
    public static Config CurrentConfig;

    // Config path (current directory)
    private const string ConfigPath = "config.json";
    
    
    /// <summary>
    /// Async task to load config (when config file is not presented it creates new config and saves it)
    /// </summary>
    public static async Task LoadConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            CurrentConfig = new ()
            {
                AdminUsername = "admin",
                AdminPassword = "AdminPassw0rd" + Random.Shared.Next(Int32.MaxValue/2, Int32.MaxValue),
                SmtpClients = new List<ConfigSmtpClient>()
            };
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Generate new admin account '{CurrentConfig.AdminUsername}' " +
                              $"with password '{CurrentConfig.AdminPassword}'");
            
            
            await File.WriteAllTextAsync(ConfigPath, JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented));
        }

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Config load...");
        CurrentConfig = JsonConvert.DeserializeObject<Config>(await File.ReadAllTextAsync(ConfigPath));
    }

    /// <summary>
    /// Async task to save config to ConfigPath
    /// </summary>
    public static async Task SaveConfig()
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Saving config...");
        await File.WriteAllTextAsync(ConfigPath, JsonConvert.SerializeObject(CurrentConfig, Formatting.Indented));
    }


    public static bool TryGetSmtpClient(string id, out ConfigSmtpClient smtpClient)
    {
        int smtpIndex = CurrentConfig.SmtpClients.FindIndex(x => x.Id == id);

        if (smtpIndex < 0)
        {
            smtpClient = new ConfigSmtpClient();
            return false;
        }

        smtpClient = CurrentConfig.SmtpClients[smtpIndex];
        return true;
    }
}


// FOR NOW WE ARE USING SINGLE ADMIN ACCOUNT (ITS NOT FULL MAIL SERVICE)
public struct Config
{
    public string AdminUsername { get; set; }
    public string AdminPassword { get; set; }
    
    public List<ConfigSmtpClient> SmtpClients { get; set; }
}

public struct ConfigSmtpClient
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string Host { get; set; }
}