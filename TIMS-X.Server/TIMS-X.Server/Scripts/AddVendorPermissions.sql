INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetPractice', N'Returns practice and site details', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetProvider', N'Returns provider details', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetProviders', N'Returns list of provider details', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetPatient', N'Returns patient details', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetUpdatedPatients', N'Returns list of patient detials', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/FindPatient', N'Returns patient details. Can create patient if not found', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/CalculatePatientBatches', N'Returns list of patient id ranges', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetPatientBatch', N'Returns list of patient details', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/FindAppointmentOpenings', N'Returns opening times', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetAppointmentStatuses', N'Returns list of appointment statuses', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetAppointmentTypes', N'Returns list of appointment types', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetUpdatedAppointments', N'Returns list of appointment details', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/CreateAppointment', N'Creates a patient appointment in the database', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/CreatePatient', N'Create patient', 0, CAST(N'2022-06-08T16:02:32.973' AS DateTime))
GO
INSERT [dbo].[VendorPermission] ([Name], [Description], [Inactive], [DateCreated]) VALUES (N'View Patient Records', N'Allows the vendor to view patient demographic information.', 0, GETDATE())
GO
INSERT [dbo].[VendorPermission] ([Name], [Description], [Inactive], [DateCreated]) VALUES (N'View Appointment Records', N'Allows the vendor to view appointment details, including patient and non-patient appointments.', 0, GETDATE())
GO
INSERT [dbo].[VendorPermission] ([Name], [Description], [Inactive], [DateCreated]) VALUES (N'Create Appointment Records', N'Allows the vendor to create patient appointments in the database.', 0, GETDATE())
GO
INSERT [dbo].[VendorPermission] ([Name], [Description], [Inactive], [DateCreated]) VALUES (N'View Provider Records', N'Allows the vendor to view provider details.', 0, GETDATE())
GO
INSERT [dbo].[VendorPermission] ([Name], [Description], [Inactive], [DateCreated]) VALUES (N'View Practice/Site Records', N'Allows vendor to access practice and site details.', 0, GETDATE())
GO
INSERT [dbo].[VendorPermission] ([Name], [Description], [Inactive], [DateCreated]) VALUES (1N'Create Patients', N'Allows vendor to create patients.', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetUnitsSold', N'Get Units Sold', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetUnitsReturned', N'Get Units Returned', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetLocations', N'Get Locations', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetPractices', N'Get Practices', 0, GETDATE())
GO

INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetAudiologists', N'Get Audiologists', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetPatients', N'Get Patients', 0, GETDATE())
GO
INSERT [dbo].[ApiUrl] ([Url], [Description], [Inactive], [DateCreated]) VALUES (N'/api/v1/GetAppointments', N'Get Appointments', 0, GETDATE())
GO