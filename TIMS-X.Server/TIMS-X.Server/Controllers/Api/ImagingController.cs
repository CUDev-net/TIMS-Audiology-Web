using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TIMS_X.Core;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL;
using TIMS_X.Server.Controllers.Common;
using TIMS_X.Server.Hubs;
using TIMS_X.Server.Models;
using TIMS_X.Server.Services;


namespace TIMS_X.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    public class ImagingController : ImagingControllerBase
    {
        
        public ImagingController(ImagingService imagingService, TimsUpdateService updateService, 
            IHubContext<PatientPhotoHub> patientPhotoHub, ContextHelper contextHelper) : base(imagingService, updateService, patientPhotoHub, contextHelper)
        {
            _contextHelper = contextHelper;
        }

        [HttpGet("PatientPhoto")]
        public async Task<IActionResult> GetPatientPhotoApiAsync(Guid patientGuid)
        {
            return await base.GetPatientPhotoAsync(patientGuid);
        }

        [HttpGet("InsuranceCard")]
        public async Task<IActionResult> GetInsuranceCardApiAsync(Guid patientGuid, bool front)
        {
            return await base.GetInsuranceCardAsync(patientGuid, front);
        }

        [HttpGet("InsuranceCardV2")]
        public async Task<IActionResult> GetInsuranceCardV2ApiAsync(Guid patientGuid, InsuranceSlot slot, bool front)
        {
            return await base.GetInsuranceCardAsync(patientGuid, slot, front);
        }

        [HttpPost("UploadPatientPhoto")]
        public async Task<IActionResult> UploadPatientPhotoAsync([FromQuery] Guid patientGuid, [FromBody] byte[] patientPhoto)
        {
            var images = new List<PatientImageModel>();
            if (patientPhoto != null && patientPhoto.Length > 0)
            {
                images.Add(new PatientImageModel
                {
                    DocumentTypeId = ImagingService.PhotoDocumentType,
                    Image = patientPhoto,
                    Description = $"Patient Photo - {DateTime.Now.ToShortDateString()}"
                });
            }
            await _imagingService.UploadImagesAsync(patientGuid, images);
            await _photoHub.Clients.Group(_contextHelper.OfficeCode).SendAsync(PatientPhotoHub.PhotoUpdated, patientGuid);
            await _photoHub.Clients.Group(_contextHelper.OfficeCode).SendAsync(PatientPhotoHub.PhotoUpdated2, patientGuid, patientPhoto);
            return new JsonResult("Success");
        }

        [HttpPost("UploadInsuranceCardPhoto")]
        public async Task<IActionResult> UploadInsuranceCardPhotoAsync([FromQuery] Guid patientGuid, [FromQuery] bool front, [FromBody] byte[] insuranceCardPhoto)
        {
            var images = new List<PatientImageModel>();
            if (insuranceCardPhoto != null && insuranceCardPhoto.Length > 0)
            {
                var docType = await _imagingService.GetInsuranceCardDocumentTypeAsync();
                images.Add(new PatientImageModel
                {
                    DocumentTypeId = docType.Id,
                    Image = insuranceCardPhoto,
                    Description = front ? "front" : "back"
                });
            }
            await _imagingService.UploadImagesAsync(patientGuid, images);

            return new JsonResult("Success");
        }


        [HttpPost("UploadInsuranceCardPhotoV2")]
        public async Task<IActionResult> UploadInsuranceCardPhotoAsync([FromQuery] Guid patientGuid, [FromQuery] InsuranceSlot slot, [FromQuery] bool front, [FromBody] byte[] insuranceCardPhoto)
        {
            var images = new List<PatientImageModel>();
            if (insuranceCardPhoto != null && insuranceCardPhoto.Length > 0)
            {
                var docType = await _imagingService.GetInsuranceCardDocumentTypeAsync();
                images.Add(new PatientImageModel
                {
                    DocumentTypeId = docType.Id,
                    Image = insuranceCardPhoto,
                    Description = front ? $"Front ({slot})" : $"Back ({slot})"
                });
            }
            await _imagingService.UploadImagesAsync(patientGuid, images);

            return new JsonResult("Success");
        }

        [HttpGet("DoesVideoExist")]
        public async Task<IActionResult> DoesVideoExistAsync(int id)
        {
            var exists = await _imagingService.DoesVideoExistAsync(id);
            return Ok(exists);
        }

        [HttpPost("UpdateShareVideo")]
        public async Task<IActionResult> UpdateShareVideoAsync(ShareVideoModel model)
        {
            await _imagingService.UpdateShareVideoAsync(model);
            return Ok();
        }

        [HttpGet("GetShareVideo")]
        public async Task<IActionResult> GetShareVideoAsync(int id)
        {
            var model = await _imagingService.GetShareVideoAsync(id);
            return Ok(model);
        }
        private readonly ContextHelper _contextHelper;
    }
}
