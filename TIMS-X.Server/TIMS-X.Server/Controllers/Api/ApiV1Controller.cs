using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.BLL.VendorSync.Repositories;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Models;
using TIMS_X.Core.Services;
using TIMS_X.DAL.DAL;
using TIMS_X.DAL.DAL.UoWs;
using TIMS_X.Server.Config;
using TIMS_X.Server.Data;
using TIMS_X.Server.Middleware;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;
using Appointment = TIMS_X.BLL.VendorSync.Audigy.Appointment;
using Patient = TIMS_X.BLL.VendorSync.Audigy.Patient;

namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/v1")]
    [ApiController]
    [AllowAnonymous]
    public class ApiV1Controller : ControllerBase
    {
        private readonly IVendorAppointmentRepository _vendorAppointmentRepository;
        private readonly IVendorHAUnitRepository _vendorHaUnitRepository;
        private readonly IPracticeRepository _practiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CustomerQuery _customerQuery;
        private readonly ILocationRepository _locationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAudiologistRepository _audiologistRepository;
        private readonly IVendorPatientRepository _vendorPatientRepository;
        private readonly IPointOfSaleRepository _pointOfSaleRepository;
        private readonly IDataMiningRepository _dataMiningRepository;
        private readonly ImagingService _imagingService;
        private readonly PatientService _patientService;
        private readonly ProviderQuery _providerQuery;
        private readonly UserQuery _userQuery;
        private readonly TimsInternalDbContext _dbContext;

		public ApiV1Controller(IVendorAppointmentRepository vendorAppointmentRepository,
            IPracticeRepository practiceRepository,
            CustomerQuery customerQuery,
            IHttpContextAccessor httpContextAccessor,
            ILocationRepository locationRepository,
            IEmployeeRepository employeeRepository,
            IAudiologistRepository audiologistRepository,
            IVendorPatientRepository vendorPatientRepository,
            IVendorHAUnitRepository vendorHAUnitRepository,
            IPointOfSaleRepository pointOfSaleRepository,
            IDataMiningRepository dataMiningRepository,
            ImagingService imagingService,
            PatientService patientService,
            UserQuery userQuery,
            ProviderQuery providerQuery,
            TimsInternalDbContext dbContext)
        {
            _vendorAppointmentRepository = vendorAppointmentRepository;
            _practiceRepository = practiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _customerQuery = customerQuery;
            _locationRepository = locationRepository;
            _employeeRepository = employeeRepository;
            _audiologistRepository = audiologistRepository;
            _vendorPatientRepository = vendorPatientRepository;
            _vendorHaUnitRepository = vendorHAUnitRepository;
            _pointOfSaleRepository = pointOfSaleRepository;
            _dataMiningRepository = dataMiningRepository;
            _imagingService = imagingService;
            _patientService = patientService;
            _userQuery = userQuery;
            _providerQuery = providerQuery;
            _dbContext = dbContext;
        }
        
        /// <summary>
        /// Get Practice
        /// </summary>
        /// <remarks>
        /// Returns general information (name, address, hours of operation) about the practice.<br/>
        /// <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetPractice"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <response code="200">Practice and site information is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetPractice")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(BadRequest), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetPracticeAsync()
        {
	        try
	        {
		        var practice = await _practiceRepository.GetPractice();
		        return Ok(practice);
			}
	        catch (Exception e)
	        {
		         await _dbContext.TimsLogs.AddAsync(new TimsLog()
			        { DateCreated = DateTime.Now, Message = "ApiV1Controller:GetPracticeAsync", Error = e.ToString() });
		        await _dbContext.SaveChangesAsync();
				return BadRequest(e.Message);
	        }
        }

        /// <summary>
        /// Get Practices
        /// </summary>
        /// <remarks>
        /// Returns general information (name, address, hours of operation) about all audigy practices.<br/>
        /// <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetPractice"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <response code="200">Practice and site information is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetPractices")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(BadRequest), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetPracticesAsync()
        {
            var practiceList = new List<BLL.VendorSync.Audigy.Practice>();
            try
            {
                var customers = AudigyCustomers.GetCustomers();
                foreach(var officeCode in customers.Keys)
                {
                    var connectionInfo = await _customerQuery.GetConnectionInfoAsync(officeCode);
                    var options = new DbContextOptionsBuilder<TimsContext>();
                    ConnectionStringBuilder.SetConnectionString(options, connectionInfo.Server, connectionInfo.Database, connectionInfo.User, connectionInfo.Password);
                    var dbContext = new TimsContext(options.Options);
                    var practiceUnitOfWork = new PracticeUnitOfWork(dbContext, _httpContextAccessor);
                    var practiceRepository = new PracticeRepository(practiceUnitOfWork);
                    var practice = await practiceRepository.GetPractice();
                    practiceList.Add(practice);
                }
            }
            catch(Exception e)
            {
				 await _dbContext.TimsLogs.AddAsync(new TimsLog()
					{ DateCreated = DateTime.Now, Message = "ApiV1Controller:GetPractices", Error = e.ToString() });
				await _dbContext.SaveChangesAsync();
				return BadRequest(e.Message);
			}

            return Ok(practiceList);
        }

        /// <summary>
        /// Get Locations
        /// </summary>
        /// <remarks>
        /// Returns general information (name, address, hours of operation) about each location.<br/>
        /// <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetLocations"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <response code="200">Location information is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetLocations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(BadRequest), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetLocationsAsync()
        {
	        try
	        {
		        var locations = await _locationRepository.GetLocations();
		        return Ok(locations);
			}
	        catch (Exception e)
	        {
				await _dbContext.TimsLogs.AddAsync(new TimsLog()
					{ DateCreated = DateTime.Now, Message = "ApiV1Controller:GetLocationsAsync", Error = e.ToString() });
				await _dbContext.SaveChangesAsync();
				return BadRequest(e.Message);
			}

        }

        /// <summary>
        /// Get Patient
        /// </summary>
        /// <remarks>
        /// Returns a single patient by id.<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetPatient?patientId=22"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <response code="200">If exists, the patient record is returned, otherwise null</response>
        /// <response code="500">An error occurred</response>
        /// <param name="patientId">the id of the patient</param>
        [HttpGet("GetPatient")]
        [ProducesResponseType(typeof(Patient), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetPatientAsync(int patientId)
        {
            var patient = await _vendorPatientRepository.GetPatient(patientId);
            return Ok(patient);
        }

        /// <summary>
        /// Get Audiograms for a patient
        /// </summary>
        /// <remarks>
        /// Returns a list of audiograms in date order.<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetAudiograms?patientId=22"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <response code="200">A list of audiograms is returned</response>
        /// <response code="500">An error occurred</response>
        /// <param name="patientId">the id of the patient</param>
        [HttpGet("GetAudiograms")]
        [ProducesResponseType(typeof(IEnumerable<HearingTest>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetAudiogramsAsync(int patientId)
        {
            var audiograms = await _dataMiningRepository.GetAudiograms(patientId);
            return Ok(audiograms);
        }

        /// <summary>
        /// Get Employees
        /// </summary>
        /// <remarks>
        /// Returns all employees<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetEmployees?includeInactive=true"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <response code="200">A list of employees is returned</response>
        /// <response code="500">An error occurred</response>
        /// <param name="includeInactive">If true, inactive employees are included</param>
        [HttpGet("GetEmployees")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetEmployeesAsync(bool includeInactive)
        {
            var employees = await _employeeRepository.GetEmployees(includeInactive);
            return Ok(employees);
        }

        /// <summary>
        /// Get Audiologists
        /// </summary>
        /// <remarks>
        /// Returns all audiologists<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetAudiologists?includeInactive=true"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <response code="200">A list of audiologists is returned</response>
        /// <response code="500">An error occurred</response>
        /// <param name="includeInactive">If true, inactive audiologists are included</param>
        [HttpGet("GetAudiologists")]
        [ProducesResponseType(typeof(IEnumerable<Audiologist>), 200)]
        [ProducesResponseType(typeof(BadRequest), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetAudiologistsAsync(bool includeInactive)
        {
	        try
	        {
		        var audiologists = await _audiologistRepository.GetAudiologists(includeInactive);
		        return Ok(audiologists);
			}
	        catch (Exception e)
	        {
		        await _dbContext.TimsLogs.AddAsync(new TimsLog()
			        { DateCreated = DateTime.Now, Message = "ApiV1Controller:GetAudiologistsAsync", Error = e.ToString() });
		        await _dbContext.SaveChangesAsync();
		        return BadRequest(e.Message);
	        }
        }

        /// <summary>
        /// Get Patients
        /// </summary>
        /// <remarks>
        /// Returns patients<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetPatients"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="fromDate">Optional: From date inclusive to get patients from</param>
        /// <param name="toDate">Optional: To date inclusive to get patients to</param>
        /// <response code="200">A list of patients is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetPatients")]
        [ProducesResponseType(typeof(IEnumerable<Patient>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetPatientsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var patients = await _vendorPatientRepository.GetPatients(fromDate, toDate);
            return Ok(patients);
        }

        /// <summary>
        /// Get Appointments
        /// </summary>
        /// <remarks>
        /// Returns Appointments<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetAppointments"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="fromDate">Optional: From date inclusive to get appointments from</param>
        /// <param name="toDate">Optional: To date inclusive to get appointments to</param>
        /// <response code="200">A list of appointments is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetAppointments")]
        [ProducesResponseType(typeof(IEnumerable<Appointment>), 200)]
        [ProducesResponseType(typeof(BadRequest), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetAppointmentsAsync(DateTime? fromDate, DateTime? toDate)
        {
	        try
	        {
		        var appointments = await _vendorAppointmentRepository.GetAppointments(fromDate, toDate);
		        return Ok(appointments);
			}
	        catch (Exception e)
	        {
		        await _dbContext.TimsLogs.AddAsync(new TimsLog()
			        { DateCreated = DateTime.Now, Message = "ApiV1Controller:GetAppointmentsAsync", Error = e.ToString() });
		        await _dbContext.SaveChangesAsync();
				return BadRequest(e.Message);
	        }
        }

        /// <summary>
        /// Get Units Sold
        /// </summary>
        /// <remarks>
        /// Returns Units Sold<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetUnitsSold"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="fromDate">Optional: From date inclusive to get units sold from</param>
        /// <param name="toDate">Optional: To date inclusive to get units sold to</param>
        /// <response code="200">A list of units sold is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetUnitsSold")]
        [ProducesResponseType(typeof(IEnumerable<HAUnitSold>), 200)]
        [ProducesResponseType(typeof(BadRequest), 400)]
		[ProducesResponseType(typeof(ErrorResponse), 500)]
		public async Task<ActionResult> GetUnitsSoldAsync(DateTime? fromDate, DateTime? toDate)
        {
	        try
	        {
		        var unitsSold = await _vendorHaUnitRepository.GetHAUnitsSold(fromDate, toDate);
		        return Ok(unitsSold);
			}
	        catch (Exception e)
	        {
		        await _dbContext.TimsLogs.AddAsync(new TimsLog()
			        { DateCreated = DateTime.Now, Message = "ApiV1Controller:GetUnitsSoldAsync", Error = e.ToString() });
		        await _dbContext.SaveChangesAsync();
		        return BadRequest(e.Message);
	        }
        }

        /// <summary>
        /// Get Units Returned
        /// </summary>
        /// <remarks>
        /// Returns Units Returned<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetUnitsReturned"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="fromDate">Optional: From date inclusive to get units returned from</param>
        /// <param name="toDate">Optional: To date inclusive to get units returned to</param>
        /// <response code="200">A list of units returned is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetUnitsReturned")]
        [ProducesResponseType(typeof(IEnumerable<HAUnitReturned>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetUnitsReturnedAsync(DateTime? fromDate, DateTime? toDate)
        {
            var unitsReturned = await _vendorHaUnitRepository.GetHAUnitsReturned(fromDate, toDate);
            return Ok(unitsReturned);
        }

        /// <summary>
        /// Get Appointment
        /// </summary>
        /// <remarks>
        /// Returns Appointment<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetAppointment?appointmentId=22"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="appointmentId">Id of the appointment</param>
        /// <response code="200">Appointment is returned if found, otherwise null</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetAppointment")]
        [ProducesResponseType(typeof(Appointment), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetAppointmentAsync(int appointmentId)
        {
            var appointment = await _vendorAppointmentRepository.GetAppointment(appointmentId);
            return Ok(appointment);
        }

        /// <summary>
        /// Get Invoices
        /// </summary>
        /// <remarks>
        /// Returns Invoices<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetInvoices"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="fromDate">Optional: From date inclusive to get invoices from</param>
        /// <param name="toDate">Optional: To date inclusive to get invoices to</param>
        /// <response code="200">A list of invoices is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetInvoices")]
        [ProducesResponseType(typeof(IEnumerable<PointOfSaleDocument>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetInvoicesAsync(DateTime? fromDate, DateTime? toDate)
        {
            var invoices = await _pointOfSaleRepository.GetInvoices(fromDate, toDate);
            return Ok(invoices);
        }

        /// <summary>
        /// Get Invoice
        /// </summary>
        /// <remarks>
        /// Returns Invoice<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetInvoice?invoiceId=22"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="invoiceId">Id of the invoice</param>
        /// <response code="200">Invoice is returned if found, otherwise null</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetInvoice")]
        [ProducesResponseType(typeof(PointOfSaleDocument), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetInvoiceAsync(int invoiceId)
        {
            var invoice = await _pointOfSaleRepository.GetPosDocument(invoiceId);
            return Ok(invoice);
        }

        /// <summary>
        /// Get Returns
        /// </summary>
        /// <remarks>
        /// Returns Returns<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetReturns"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="fromDate">Optional: From date inclusive to get returns from</param>
        /// <param name="toDate">Optional: To date inclusive to get returns to</param>
        /// <response code="200">A list of returns is returned</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetReturns")]
        [ProducesResponseType(typeof(IEnumerable<PointOfSaleDocument>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetReturnsAsync(DateTime? fromDate, DateTime? toDate)
        {
            var returns = await _pointOfSaleRepository.GetReturns(fromDate, toDate);
            return Ok(returns);
        }

        /// <summary>
        /// Get Return
        /// </summary>
        /// <remarks>
        /// Returns Return<br/>
        ///  <fieldset><legend>Example</legend><code>curl -X GET "https://us.timsaudiology.com/api/v1/GetInvoice?invoiceId=22"<br/>
        /// -H "accept: application/json" -H "OfficeCode: Sales" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="returnId">Id of the return</param>
        /// <response code="200">Return is returned if found, otherwise null</response>
        /// <response code="500">An error occurred</response>
        [HttpGet("GetReturn")]
        [ProducesResponseType(typeof(PointOfSaleDocument), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<ActionResult> GetReturnAsync(int returnId)
        {
            var invoice = await _pointOfSaleRepository.GetPosDocument(returnId);
            return Ok(invoice);
        }

        /// <summary>
        /// Upload Patient Document
        /// </summary>
        /// <remarks>
        /// Uploads PDF document to patient record
        /// <fieldset><legend>Example</legend><code>curl -v -include --form "PatientId=your-patient-id" --form "DocumentTitle=your-document-title" --form "AlertMessage=your-alert-message" --form Document=@your-pdf-file.pdf "https://audhhiapp1.timsaudiology.com:44376/api/v1/UploadPatientDocument" -H "accept: application/json" -H "OfficeCode: HHI" -H "Authorization: your-api-key"</code></fieldset>
        /// </remarks>
        /// <param name="docUpload">The details of the document upload</param>
        /// <response code="200">Successful upload</response>
        /// <response code="400">Invalid document</response>
        /// <response code="500">An internal error occurred</response>

        [HttpPost("UploadPatientDocument")]
        public async Task<IActionResult> UploadPatientDocumentAsync([FromForm] DocumentUpload docUpload)
        {
            try
            {
                PatientImageModel patientImage = null;
                var patient = await _patientService.GetAsync(docUpload.PatientId);
                if (patient != null && docUpload != null && docUpload.Document != null && docUpload.Document.Length > 0)
                {
                    var apiUploadDocType = await _imagingService.GetApiUploadDocumentTypeAsync();
                    using var stream = new MemoryStream();
                    await docUpload.Document.CopyToAsync(stream);
                    patientImage = new PatientImageModel
                    {
                        DocumentTypeId = apiUploadDocType.Id,
                        Image = stream.ToArray(),
                        Description = docUpload.DocumentTitle
                    };
                    await _imagingService.UploadPatientDocumentAsync(patient.Guid, patientImage);

                    // create task for provider to review document
                    var userId = await _providerQuery.GetUserIdFromProviderAsync(patient.ProviderId ?? 0);
                    var taskType = await _userQuery.GetOrCreateUserTaskTypeAsync("Review Document");

                    var task = new UserTask
                    {
                        MadeBy = 0,
                        UpdatedDate = DateTime.Now,
                        UpdatedUserId = 0,
                        CreatedDate = DateTime.Now,
                        UserTaskTypeId = taskType.Id,
                        PatientId = patient.Id,
                        DueDate = DateTime.Now.AddDays(2),
                    };
                    await _userQuery.PutUserTaskAsync(task, userId, "Review Document", docUpload.AlertMessage);


                    return Ok();
                }

            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult("Internal Server Error: " + ex.Message);
            }
            Response.StatusCode = 400;
            return new JsonResult("Invalid Document");
        }
    }
}