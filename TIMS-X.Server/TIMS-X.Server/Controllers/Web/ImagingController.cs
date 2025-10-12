using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL;
using TIMS_X.Server.Controllers.Common;
using TIMS_X.Server.Hubs;
using TIMS_X.Server.Models;
using TIMS_X.Server.Services;
using TIMS_X.Server.Utils;

namespace TIMS_X.Server.Controllers.Web
{
    [Route("web/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    public class ImagingController : ImagingControllerBase
    {

        public ImagingController(ImagingService imagingService, TimsUpdateService updateService, IHubContext<PatientPhotoHub> patientPhotoHub, ContextHelper contextHelper) : 
            base(imagingService, updateService, patientPhotoHub, contextHelper)
        {
           
        }

        [HttpGet("PatientPhoto")]
        public async Task<IActionResult> GetPatientPhotoWebAsync(Guid patientGuid)
        {
            return await base.GetPatientPhotoAsync(patientGuid);
        }
        

        //[HttpPost("UploadDocuments")]
        //public async Task<IActionResult> PostUploadDocumentsWebAsync(IFormFile patientPhoto, IFormFile insuranceCardFront, IFormFile insuranceCardBack)
        //{
        //    var patientGuidString = Request.Headers.Where(x => x.Key.ToLower() == "patientguid").Select(x => x.Value)
        //        .FirstOrDefault()
        //        .ToString();
        //    if (string.IsNullOrWhiteSpace(patientGuidString))
        //    {
        //        Response.StatusCode = 400;
        //        return new JsonResult("Invalid patient guid");
        //    }
        //    var patientGuid = new Guid(patientGuidString);
        //    var result = await base.UploadDocumentsAsync(patientGuid, patientPhoto, insuranceCardFront, insuranceCardBack);
        //    //var userId = $"{Core.Application.OfficeCode}-{Core.Application.CurrentUser.Id}";
        //    //if (PatientPhotoHub.IsUserConnected(userId))
        //    //{
        //    //    await _photoHub.Clients.User(userId).SendAsync("PhotoUpdated", patientGuid);
        //    //}
        //    return result;
        //}


        [HttpPost("UploadDocumentsV2")]
        public async Task<IActionResult> PostUploadDocumentsV2WebAsync(IFormFile patientPhoto, IFormFile insuranceCardFront, IFormFile insuranceCardBack, 
            [FromQuery] InsuranceSlot frontSlot, [FromQuery] InsuranceSlot backSlot)
        {
            var patientGuidString = Request.Headers.Where(x => x.Key.ToLower() == "patientguid").Select(x => x.Value)
                .FirstOrDefault()
                .ToString();
            if (string.IsNullOrWhiteSpace(patientGuidString))
            {
                Response.StatusCode = 400;
                return new JsonResult("Invalid patient guid");
            }
            var patientGuid = new Guid(patientGuidString);
            var result = await base.UploadDocumentsAsync(patientGuid, patientPhoto, insuranceCardFront, insuranceCardBack, frontSlot, backSlot);
            //var userId = $"{Core.Application.OfficeCode}-{Core.Application.CurrentUser.Id}";
            //if (PatientPhotoHub.IsUserConnected(userId))
            //{
            //    await _photoHub.Clients.User(userId).SendAsync("PhotoUpdated", patientGuid);
            //}
            return result;
        }
    }
}
