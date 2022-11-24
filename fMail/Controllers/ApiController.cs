using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;

namespace fMail.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    [HttpPost("config")]
    public async Task<IActionResult> PostChangeConfig([FromBody] NewConfig newConfig)
    {
        if (newConfig.CurrentUsername != ConfigParser.CurrentConfig.AdminUsername ||
            newConfig.CurrentPassword != ConfigParser.CurrentConfig.AdminPassword)
        {
            return StatusCode(401);
        }
            
        if (newConfig.Config.Equals(null) || newConfig.Config.Equals(new Config()))
        {
            return BadRequest();
        }
            
        ConfigParser.CurrentConfig = newConfig.Config;
        await ConfigParser.SaveConfig();
        return StatusCode(200);
    }

    [HttpPost("send")]
    public async Task<IActionResult> PostSendMails([FromBody] MailInfo mailInfo)
    {
        if (mailInfo.CurrentUsername != ConfigParser.CurrentConfig.AdminUsername ||
            mailInfo.CurrentPassword != ConfigParser.CurrentConfig.AdminPassword)
        {
            return StatusCode(401);
        }

        if (!ConfigParser.TryGetSmtpClient(mailInfo.SmtpId, out ConfigSmtpClient cfgSmtpClient))
            return StatusCode(404);

        Task sendMailTask = new Task(() =>
        {
            SendMails(cfgSmtpClient, mailInfo);
        });
        sendMailTask.Start();
        
        // await SendMailsAsync(cfgSmtpClient, sendMails);
        return Ok();
    }

    [HttpPost("auth")]
    public async Task<IActionResult> PostLogin([FromBody] AdminLogin adminLogin)
    {
        if (adminLogin.Username != ConfigParser.CurrentConfig.AdminUsername ||
            adminLogin.Password != ConfigParser.CurrentConfig.AdminPassword)
        {
            return StatusCode(401);
        }

        return Ok();
    }


    static void SendMails(ConfigSmtpClient configSmtpClient, MailInfo mailInfo)
    {
        var smtp = new SmtpClient();
        smtp.UseDefaultCredentials = false;
        var NetworkCredentials = new NetworkCredential()
        {
            UserName = configSmtpClient.Username,
            Password = configSmtpClient.Password
        };

        smtp.Port = configSmtpClient.Port;
        smtp.EnableSsl = configSmtpClient.EnableSsl;
        smtp.Host = configSmtpClient.Host;
        smtp.Credentials = NetworkCredentials;

        foreach (var email in mailInfo.Emails)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(configSmtpClient.Username, mailInfo.DisplayName);
                msg.To.Add(email);
                msg.Subject = mailInfo.Subject;
                msg.IsBodyHtml = mailInfo.IsBodyHtml;
                msg.Body = mailInfo.Content;
                smtp.Send(msg);
            }
            catch
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Failed to send mail to {email}...");
            }
        }
    }

    public struct AdminLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public struct NewConfig
    {
        public string CurrentUsername { get; set; }
        public string CurrentPassword { get; set; }

        public Config Config { get; set; }
    }

    public struct MailInfo
    {
        public string CurrentUsername { get; set; }
        public string CurrentPassword { get; set; }

        public string SmtpId { get; set; }

        public string[] Emails { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string DisplayName { get; set; }
    }
}