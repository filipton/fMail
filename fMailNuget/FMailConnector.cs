using System.Net.Http.Json;

namespace fMail;

public class FMailConnector
{
    private static readonly HttpClient Client = new();

    private static bool Setup = false;
    
    private static string Username;
    private static string Password;
    
    public static async Task SetupConnection(string url, string username, string password)
    {
        Username = username;
        Password = password;
        
        Client.BaseAddress = new Uri(url);
        Client.Timeout = TimeSpan.FromSeconds(30);

        var response = await Client.PostAsJsonAsync("api/auth", new
        {
            Username,
            Password
        });

        if(!response.IsSuccessStatusCode) throw new Exception("Authorization failed!");
        Setup = true;
    }

    public static async Task<bool> SendMessage(string smtpID, fMailObject fMailObject)
    {
        if(!Setup) throw new Exception("Setup first!");
        var response = await Client.PostAsJsonAsync("api/send", new
        {
            CurrentUsername = Username,
            CurrentPassword = Password,
            
            SmtpId = smtpID,
            
            fMailObject.Emails,
            fMailObject.IsBodyHtml,
            fMailObject.Subject,
            fMailObject.Content,
            fMailObject.DisplayName
        });

        return response.IsSuccessStatusCode;
    }

    public struct fMailObject
    {
        public string[] Emails { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string DisplayName { get; set; }
    } 
}