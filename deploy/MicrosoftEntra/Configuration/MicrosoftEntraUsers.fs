namespace AuthZI.Deploy.MicrosoftEntra.Configuration

type MicrosoftEntraUser = {
    DisplayName: string
    MailNickname: string
    Password: string
}

module MicrosoftEntraUsers =
    [<Literal>]
    let GeneralPassword = "Passw@rd+1"
    let adeleV = { DisplayName = "Adele Vance"; MailNickname = "AdeleV"; Password = GeneralPassword }
    let alexW = { DisplayName = "Alex Wilber"; MailNickname = "AlexW"; Password = GeneralPassword }