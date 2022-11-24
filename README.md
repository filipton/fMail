
# fMail

Easy to use self-hosted mail api.
## Installation/Startup
(Docker images in future)

At first startup, when no config.json is presented it will 
generate new config with random generated password!

Default username: admin
## Usage

```csharp
// Setup first (Only once)
await FMailConnector.SetupConnection("url-to-api", "username", "password");


// Then you can send emails like that:
var fMailObject = new FMailConnector.fMailObject()
{
    Content = "<h1>test</h1>",
    DisplayName = "Test",
    Emails = new []
    {
        "example@example.com"
    },
    Subject = "Test Mail",
    IsBodyHtml = true
};

// Set your id same as id in your config.json
await FMailConnector.SendMessage("smtpID", fMailObject);
```


## API Reference
It's gonna be changed!

#### Sets new config

```http
  POST /api/config
```

| Json Field Name | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `CurrentUsername` | `string` | **Required**. Your admin username |
| `CurrentPassword` | `string` | **Required**. Your admin password |
| `Config` | `config` | **Required**. New config |

#### Auth

```http
  POST /api/auth
```

| Json Field Name | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `Username`      | `string` | **Required**. Your admin username |
| `Password`      | `string` | **Required**. Your admin password |


#### Send mail(s)

```http
  POST /api/send
```

| Json Field Name | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `CurrentUsername`      | `string` | **Required**. Your admin username |
| `CurrentPassword`      | `string` | **Required**. Your admin password |
| `SmtpId`      | `string` | **Required**. Id of smtp server you would like to use |
| `Emails`      | `string[]` | **Required**. List of emails to send email |
| `IsBodyHtml`      | `bool` | **Required**. Indicates if body is written in HTML |
| `Subject`      | `string` | **Required**. Subject of email |
| `Content`      | `string` | **Required**. Content of email |
| `DisplayName`      | `string` | **Required**. Display name |

#### 