# TIMS-Audiology-Web
The source code for the web portal for TIMS Audiology.  It consists of 3 parts
 
## Support Portal
Support where users can configure Customer sites:
- Add or Edit Customers
- Add or Edit Servers
- Add or Edit Support Users
- Add or Edit Vendors(used for exchanging data with vendors)
- Add or Edit Vendoer permissions

## Audiologist Portal
Allows Customers to:
- Add or Edit Patients
- Add/View/Edit/Delete Patient Appointments
- Add/View/Edit/Delete Non-Patient Appointments
- Edit online patient intake form
- View the online desktop help information

## TIMS Assistant
A WPF desktop application for accessing patient [NOAH data](https://www.himsa.com/products/all-about-noah-system-4/)
- Allows access to the web patients to create Noah Sessions and Actions

## Source
The application consists of a C# ASP.net core back end and Angular 17 SPA
- Angular front end
    - The [spa client](https://github.com/CUDev-net/TIMS-Audiology-Web/tree/main/TIMS-X.Server/TIMS-X.Server/web-client)
    - MOBX is used as the client backend store for state management
- .net core server application consisting of:
    - A [ASP.net core application](https://github.com/CUDev-net/TIMS-Audiology-Web/tree/main/TIMS-X.Server/TIMS-X.Server) for exposing endpoints and authentication
    - A [middle tier](https://github.com/CUDev-net/TIMS-Audiology-Web/tree/main/TIMS-X.BLL) for handling business logic
    - A [database layer](https://github.com/CUDev-net/TIMS-Audiology-Web/tree/main/TIMS-X.DAL) for handling database operations
        - Utilizes EF Core
        - Uses a Unit of work pattern and an [abstract Unit of Work](https://github.com/CUDev-net/TIMS-Audiology-Web/blob/main/TIMS-X.DAL/DAL/UoWs/UnitOfWorkBase.cs) to reduce duplicate code for CRUD actions
 - [TIMS Assistant](https://github.com/CUDev-net/TIMS-Audiology-Web/tree/main/TIMS-X.CloudAssistant)
     - Desktop Client for accessing Noah data 

## Desktop
A desktop app for this can be found [here](https://github.com/CUDev-net/TIMS-Audiology-Desktop). The schema for the OMS part of the app is included.

## Build
Once the source code has been pulled, open the solution [TIMS-X](https://github.com/CUDev-net/TIMS-Audiology-Web/blob/main/TIMS-X.sln) in Visual Studio. 
The solution has been tested with Visual Studion 2022.

## Database
TIMS-X has an internal [databse](https://github.com/CUDev-net/TIMS-Audiology-Web/blob/main/TIMS-X.sql) used to manage support users, customer info, etc.

TIMS database is included in the TIMS repoistory.

### Configuration
App Settings in the [app-settings](https://github.com/CUDev-net/TIMS-Audiology-Web/blob/main/TIMS-X.Server/TIMS-X.Server/appsettings.json)
#### Keys
| Setting           | Value                     |
|-------------------|---------------------------|
|DbUsername         |TIMS-X database username   |
|DbPassword         |TIMS-X database password   |
|SupportPassword    |Support login password     |
|ImagingKey         |Key for data encryption    |
|ApiKey             |Twilio API key             |
|TimsToken          |Authentication token       |
|DefaultPassword    |Default new user password  |
|HelpPassword6_8    |Password for online help   |
|TwilioAccountSid   |Twilio account             |
|TwilioAuthToken    |Twilio account             |
|MailgunApiKey      |Mailgun integration        |

| Dpeprecated                                   |
|-----------------------------------------------|
|AwsAccessKey                                   |
|AwsAccessKeySecret                             |
|TwilioSmsStatusCallbackUrl                     |
|TwilioVoiceStatusCallbackUrl                   |
|TwilioVoiceCallScriptCallbackUrl               |
|ZoomTokenAuthUrl                               |
|ZoomUserAuthUrl                                |

#### ConnectionStrings
| Setting           | Value                     |
|-------------------|---------------------------|
|TIMSInternal       |TIMS-X internal database   |
|AzureBlobStorage   |Blob storage in Azure      |

#### LogFile
File for server side logging

#### Database connectivity
The TIMS-X internal database connection is configured [here](https://github.com/CUDev-net/TIMS-Audiology-Web/blob/main/TIMS-X.Server/TIMS-X.Server/Startup.cs#L237)

