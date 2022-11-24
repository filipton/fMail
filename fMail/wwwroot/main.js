var currUsername;
var currPassword;

var smtpClients = [];

function checkLogin()
{
    var form = document.getElementById('loginContainer');
    var formData = new FormData(form);
    
    fetch("/api/auth", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            username: formData.get("username"),
            password: formData.get("password")
        })
    }).then(async (x) => {
        if(x.ok){
            currUsername = formData.get("username");
            currPassword = formData.get("password");

            smtpClients = await x.json();
            
            afterLogin();
        }
    });
    
    return false;
}

function afterLogin(){
    document.getElementById('loginContainer').outerHTML = "";
    document.getElementById('adminContainer').style.display = 'block';
    renderSmtps();
}

function removeSmtp(index)
{
    smtpClients = smtpClients.filter(x => x !== smtpClients[index]);
    renderSmtps();
}

function renderSmtps()
{
    var sc = document.getElementById("smtpClients");
    sc.innerHTML = "";
    
    for(var i = 0; i < smtpClients.length; i++)
    {
        sc.innerHTML += `<p>${smtpClients[i].id} | ${smtpClients[i].username} ${smtpClients[i].password} | ${smtpClients[i].host} ${smtpClients[i].port} ${smtpClients[i].enableSsl}</p>`;
    }
}

function addNewSmtp()
{
    var form = document.getElementById('addNewContainer');
    var formData = new FormData(form);
    
    smtpClients.push({
        id: formData.get("id"),
        username: formData.get("user"),
        password: formData.get("pass"),
        port: parseInt(formData.get("port")),
        enableSsl: document.getElementById("ssl").checked,
        host: formData.get("host")
    });

    console.log(smtpClients);
    
    fetch("/api/config", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            currentUsername: currUsername,
            currentPassword: currPassword,
            
            Config: {
                adminUsername: currUsername,
                adminPassword: currPassword,
                
                smtpClients: smtpClients
            }
        })
    }).then(x =>{
        if(!x.ok) alert("ERROR!")
        renderSmtps();
    });
    
    return false;
}