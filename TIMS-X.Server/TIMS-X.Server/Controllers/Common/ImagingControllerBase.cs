using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using TIMS_X.Core;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.DAL.DAL;
using TIMS_X.Server.Hubs;
using TIMS_X.Server.Models;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Controllers.Common
{
    public class ImagingControllerBase : ControllerBase
    {
        protected readonly ImagingService _imagingService;
        protected readonly TimsUpdateService _updateService;
        protected readonly IHubContext<PatientPhotoHub> _photoHub;
        private readonly ContextHelper _contextHelper;
        public ImagingControllerBase(ImagingService imagingService, TimsUpdateService updateService, 
            IHubContext<PatientPhotoHub> photoHub, ContextHelper contextHelper)
        {
            _imagingService = imagingService;
            _updateService = updateService;
            _photoHub = photoHub;
            _contextHelper = contextHelper;
        }

        public virtual async Task<IActionResult> GetPatientPhotoAsync(Guid patientGuid)
        {
            var patientImage = await _imagingService.GetPatientPhotoAsync(patientGuid);
            if (patientImage != null && patientImage.Image != null && patientImage.Image.Length > 0)
            {
                return new FileContentResult(patientImage.Image, "image/bmp");
            }

            // Try to tell the browser not to cache the anonymous image
            Response.Headers.Add(
                new KeyValuePair<string, StringValues>("Pragma-directive", new StringValues("no-cache")));
            Response.Headers.Add(
                new KeyValuePair<string, StringValues>("Cache-directive", new StringValues("no-cache")));
            Response.Headers.Add(new KeyValuePair<string, StringValues>("Cache-control", new StringValues("no-cache")));
            Response.Headers.Add(new KeyValuePair<string, StringValues>("Pragma", new StringValues("no-cache")));
            Response.Headers.Add(new KeyValuePair<string, StringValues>("Expires", new StringValues("0")));
            return new VirtualFileResult("~/images/anonymous.png", "image/png");
        }

        public virtual async Task<IActionResult> GetInsuranceCardAsync(Guid patientGuid, bool front)
        {
            var image = await _imagingService.GetInsuranceCardAsync(patientGuid, front);
            if (image != null && image.Image != null && image.Image.Length > 0)
            {
                return new FileContentResult(image.Image, "image/jpeg");
            }

            return new EmptyResult();
        }

        public virtual async Task<IActionResult> GetInsuranceCardAsync(Guid patientGuid, InsuranceSlot slot, bool front)
        {
            var image = await _imagingService.GetInsuranceCardAsync(patientGuid, slot, front);
            if (image != null && image.Image != null && image.Image.Length > 0)
            {
                return new FileContentResult(image.Image, "image/jpeg");
            }

            return new EmptyResult();
        }

        public virtual async Task<IActionResult> GetInsuranceCardDateAsync(Guid patientGuid, bool front)
        {
            var date = await _imagingService.GetInsuranceCardDateAsync(patientGuid, front);
            if (date.HasValue)
            {
                return new JsonResult(date);
            }

            return new EmptyResult();
        }

        //public virtual async Task<IActionResult> UploadDocumentsAsync(Guid patientGuid, IFormFile patientPhoto, IFormFile insuranceCardFront, IFormFile insuranceCardBack)
        //{
        //    try
        //    {
        //        var images = new List<PatientImageModel>();
        //        if (patientPhoto != null && patientPhoto.Length > 0)
        //        {
        //            using (var stream = new MemoryStream())
        //            {
        //                await patientPhoto.CopyToAsync(stream);
        //                images.Add(new PatientImageModel
        //                {
        //                    DocumentTypeId = ImagingService.PhotoDocumentType,
        //                    Image = stream.ToArray(),
        //                    Description = $"Patient Photo - {DateTime.Now.ToShortDateString()}"
        //                });
        //            }
        //        }

        //        if ((insuranceCardFront != null && insuranceCardFront.Length > 0) ||
        //            (insuranceCardBack != null && insuranceCardBack.Length > 0))
        //        {
        //            var insDocType = await _imagingService.GetInsuranceCardDocumentTypeAsync();

        //            if (insuranceCardFront != null && insuranceCardFront.Length > 0)
        //            {
        //                using (var stream = new MemoryStream())
        //                {
        //                    await insuranceCardFront.CopyToAsync(stream);
        //                    images.Add(new PatientImageModel
        //                    {
        //                        DocumentTypeId = insDocType.Id,
        //                        Image = stream.ToArray(),
        //                        Description = "Front"
        //                    });
        //                }
        //            }

        //            if (insuranceCardBack != null && insuranceCardBack.Length > 0)
        //            {
        //                using (var stream = new MemoryStream())
        //                {
        //                    await insuranceCardBack.CopyToAsync(stream);
        //                    images.Add(new PatientImageModel
        //                    {
        //                        DocumentTypeId = insDocType.Id,
        //                        Image = stream.ToArray(),
        //                        Description = "Back"
        //                    });
        //                }
        //            }
        //        }

        //        await _imagingService.UploadImagesAsync(patientGuid, images);
        //        var patientPhotoImage = images.FirstOrDefault(x => x.DocumentTypeId == ImagingService.PhotoDocumentType);
        //        if (patientPhotoImage != null)
        //        {
        //            await _photoHub.Clients.Group(_contextHelper.OfficeCode.ToLower()).SendAsync(PatientPhotoHub.PhotoUpdated, patientGuid);
        //            await _photoHub.Clients.Group(_contextHelper.OfficeCode.ToLower()).SendAsync(PatientPhotoHub.PhotoUpdated2, patientGuid, patientPhotoImage.Image);
        //        }

        //        return new JsonResult("Success");
        //    }
        //    catch(Exception ex)
        //    {
        //        Response.StatusCode = 500;
        //        return new JsonResult("Internal Server Error: " + ex.Message);
        //    }


        //}


        public virtual async Task<IActionResult> UploadDocumentsAsync(Guid patientGuid, IFormFile patientPhoto, IFormFile insuranceCardFront, IFormFile insuranceCardBack, InsuranceSlot frontSlot, InsuranceSlot backSlot)
        {
            try
            {
                var images = new List<PatientImageModel>();
                if (patientPhoto != null && patientPhoto.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await patientPhoto.CopyToAsync(stream);
                        images.Add(new PatientImageModel
                        {
                            DocumentTypeId = ImagingService.PhotoDocumentType,
                            Image = stream.ToArray(),
                            Description = $"Patient Photo - {DateTime.Now.ToShortDateString()}"
                        });
                    }
                }

                var insDocType = await _imagingService.GetInsuranceCardDocumentTypeAsync();

                if ((insuranceCardFront != null && insuranceCardFront.Length > 0) ||
                    (insuranceCardBack != null && insuranceCardBack.Length > 0))
                {
                    if (insuranceCardFront != null && insuranceCardFront.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await insuranceCardFront.CopyToAsync(stream);
                            images.Add(new InsuranceCardModel
                            {
                                DocumentTypeId = insDocType.Id,
                                Image = stream.ToArray(),
                                Description = $"Front ({frontSlot})",
                                Slot = frontSlot,
                                Front = true
                            });
                        }
                    }

                    if (insuranceCardBack != null && insuranceCardBack.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await insuranceCardBack.CopyToAsync(stream);
                            images.Add(new InsuranceCardModel
                            {
                                DocumentTypeId = insDocType.Id,
                                Image = stream.ToArray(),
                                Description = $"Back ({backSlot})",
                                Slot = backSlot,
                                Front = false
                            });
                        }
                    }
                }

                // Customers running TIMS 6.0.7.31541 and lower need to use old method.
                // the documents get saved client side in new method
                var version = await _updateService.GetCurrentVersionIntAsync(_contextHelper.OfficeCode);
                if (version.Item1 == 7 && version.Item2 <= 31541)
                {
                    await _imagingService.UploadImagesAsync(patientGuid, images);
                }
                var userId = $"{_contextHelper.OfficeCode}-{_contextHelper.CurrentUser.Id}".ToLower();
                var patientPhotoImage = images.FirstOrDefault(x => x.DocumentTypeId == ImagingService.PhotoDocumentType);
                if (patientPhotoImage != null)
                {
                    await _photoHub.Clients.Group(_contextHelper.OfficeCode.ToLower()).SendAsync(PatientPhotoHub.PhotoUpdated, patientGuid);
                    await _photoHub.Clients.Group(_contextHelper.OfficeCode.ToLower()).SendAsync(PatientPhotoHub.PhotoUpdated2, patientGuid, patientPhotoImage.Image);

                    
                    if (PatientPhotoHub.IsUserConnected(userId))
                    {
                        await _photoHub.Clients.User(userId).SendAsync(PatientPhotoHub.PhotoUpdated3, patientGuid, patientPhotoImage.Image);
                    }
                    //await _photoHub.Clients.Group(_contextHelper.OfficeCode.ToLower()).SendAsync(PatientPhotoHub.PhotoUpdated3, patientGuid, patientPhotoImage.Image);
                }

                var insCards = images.Where(x => x.DocumentTypeId == insDocType.Id).ToList();
                foreach (var card in insCards.Cast<InsuranceCardModel>())
                {
                    await _photoHub.Clients.Group(_contextHelper.OfficeCode.ToLower()).SendAsync(PatientPhotoHub.InsuranceCardUpdated, patientGuid, card.Slot, card.Front, card.Image);
                    
                    if (PatientPhotoHub.IsUserConnected(userId))
                    {
                        await _photoHub.Clients.User(userId).SendAsync(PatientPhotoHub.InsuranceCardUpdated2, patientGuid, card.Slot, card.Front, card.Image);
                    }

                    //await _photoHub.Clients.Group(_contextHelper.OfficeCode.ToLower()).SendAsync(PatientPhotoHub.InsuranceCardUpdated2, patientGuid, card.Slot, card.Front, card.Image);
                }

                return new JsonResult("Success");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult("Internal Server Error: " + ex.Message);
            }


        }
    }
}