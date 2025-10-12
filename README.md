# TIMS-Audiology-Web
The source code for the web portal for TIMS Audiology.  It consists of 2 parts
 
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

## Source

The application consists of a C# ASP.net core back end and Angular 17 SPA
- Angular front end
    - The client is found in the folder `\TIMS-X.Server\TIMS-X.Server\TIMS-X.Server`
    - MOBX is used as the client backend store for state management
- .net core server application consisting of:
    - A [ASP.net core application](https://github.com/CUDev-net/TIMS-Audiology-Web/tree/main/TIMS-X.DAL) for exposing endpoints and authentication
    - A [middle tier](https://github.com/CUDev-net/TIMS-Audiology-Web/tree/main/TIMS-X.BLL) for handling business logic
    - A [database layer](https://github.com/CUDev-net/TIMS-Audiology-Web/tree/main/TIMS-X.DAL) for handling database operations
        - Utilizes EF Core
        - Uses a Unit of work pattern and an [abstract Unit of Work](https://github.com/CUDev-net/TIMS-Audiology-Web/blob/main/TIMS-X.DAL/DAL/UoWs/UnitOfWorkBase.cs) to reduce duplicate code for CRUD actions

## TIMS Desktop Application (includes SQL scripts for Database creation) 
Please see the desktop repository for the backing desktop database(required)
