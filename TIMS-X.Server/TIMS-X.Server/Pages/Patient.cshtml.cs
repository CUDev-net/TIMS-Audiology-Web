using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using Remotion.Linq.Clauses;

using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;
using TIMS_X.Server.Data;
using TIMS_X.Server.Models;
using TIMS_X.Server.Queries;
using TIMS_X.Server.Services;

namespace TIMS_X.Server.Pages
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = StringConstants.Customer)]
    public class PatientModel : PageModel
    {
        #region Constructors

        public PatientModel(PatientService patientService, ImagingService imagingService, PracticeQuery practiceQuery)
        {
            _patientService = patientService;
            _imagingService = imagingService;
            _practiceQuery = practiceQuery;
            PreparedDocuments = new List<PatientImageModel>();
        }

        #endregion Constructors

        #region PatientModel Members
        

        public List<ImageDocumentType> DocumentTypes { get; set; }

        [BindProperty]
        public List<IFormFile> DocumentUploads { get; set; }

        //public bool HasUpload =>
        //    (PhotoUpload != null && PhotoUpload.Length > 0) || 
        //    (InsuranceCardFront != null && InsuranceCardFront.Length > 0) ||
        //    (InsuranceCardBack != null && InsuranceCardBack.Length > 0) || 
        //    (DocumentUploads != null && DocumentUploads.Any(x => x.Length > 0));

        [BindProperty]
        public Patient Patient
        {
            get => _patient;
            set { _patient = value; }
        }

        [BindProperty]
        public List<InsuranceSlot> InsuranceSlots => Enum.GetValues(typeof(InsuranceSlot)).Cast<InsuranceSlot>().ToList();

        [BindProperty]
        public InsuranceSlot InsuranceSlotFront { get; set; } = InsuranceSlot.Primary;

        [BindProperty]
        public InsuranceSlot InsuranceSlotBack { get; set; } = InsuranceSlot.Primary;
        /*

                [BindProperty]
                public IFormFile PhotoUpload { get; set; }

                [BindProperty]
                public IFormFile InsuranceCardFront { get; set; }

                [BindProperty]
                public IFormFile InsuranceCardBack { get; set; }*/

        [BindProperty]
        public List<PatientImageModel> PreparedDocuments { get; set; }
        

        public bool DigitalIntakeFormExists { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Patient = await _patientService.GetAsync(id);

            if (Patient == null)
            {
                Response.StatusCode = 404;
                Patient = new Patient();
            }


            DigitalIntakeFormExists = await _practiceQuery.DigitalFormExistsAsync(PatientFormTypeEnum.Intake);

            return Page();
        }

        //public async Task<IActionResult> OnPostAsync(int id)
        //{
        //    Patient = await _patientService.GetAsync(id);
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }
        //    if (!PreparedDocuments.Any())
        //    {
        //        await _PrepareDocumentsForUploadAsync();
        //        return Page();
        //    }
        //    await _imagingService.UploadDocumentsAsync(Patient.Guid, PreparedDocuments);
        //    PreparedDocuments.Clear();
        //    return Page();
        //}

        #endregion PatientModel Members

        #region Fields

        private readonly ImagingService _imagingService;
        private readonly PatientService _patientService;
        private readonly PracticeQuery _practiceQuery;
        private Patient _patient;

        #endregion Fields

        #region Private Members
        

        

        private async Task _PrepareDocumentsForUploadAsync()
        {
            DocumentTypes = await _imagingService.GetDocumentTypesAsync(true);

            //if (PhotoUpload != null && PhotoUpload.Length > 0)
            //{
            //    using (var stream = new MemoryStream())
            //    {
            //        await PhotoUpload.CopyToAsync(stream);
            //        var bitmap = new Bitmap(stream);
            //        using (var resizedImage = _imagingService.ResizeImage(bitmap, 105, 105))
            //        {
            //            using (var resizeStream = new MemoryStream())
            //            {
            //                resizedImage.Save(resizeStream, ImageFormat.Jpeg);
            //                PreparedDocuments.Add(new PatientImageModel
            //                {
            //                    Name = PhotoUpload.FileName,
            //                    DocumentTypeId = ImagingService.PhotoDocumentType,
            //                    Image = resizeStream.ToArray(),
            //                    Description = $"Patient Photo - {DateTime.Now.ToShortDateString()}"
            //                });
            //            }
            //        }

            //    }
            //}

            //if ((InsuranceCardFront != null && InsuranceCardFront.Length > 0) || (InsuranceCardBack != null && InsuranceCardBack.Length > 0))
            //{
            //    var insDocType = await _GetInsuranceCardDocumentTypeAsync();

            //    if (InsuranceCardFront != null && InsuranceCardFront.Length > 0)
            //    {
            //        using (var stream = new MemoryStream())
            //        {
            //            await InsuranceCardFront.CopyToAsync(stream);
            //            var bitmap = new Bitmap(stream);
            //            using (var resizedImage = _imagingService.ResizeImage(bitmap, 600, 343))
            //            {
            //                using (var resizeStream = new MemoryStream())
            //                {
            //                    resizedImage.Save(resizeStream, ImageFormat.Jpeg);
            //                    PreparedDocuments.Add(new PatientImageModel
            //                    {
            //                        Name = InsuranceCardFront.FileName,
            //                        DocumentTypeId = insDocType.Id,
            //                        Image = resizeStream.ToArray(),
            //                        Description = "Front"
            //                    });
            //                }
            //            }
            //        }
            //    }

            //    if (InsuranceCardBack != null && InsuranceCardBack.Length > 0)
            //    {
            //        using (var stream = new MemoryStream())
            //        {
            //            await InsuranceCardBack.CopyToAsync(stream);
            //            var bitmap = new Bitmap(stream);
            //            using (var resizedImage = _imagingService.ResizeImage(bitmap, 600, 343))
            //            {
            //                using (var resizeStream = new MemoryStream())
            //                {
            //                    resizedImage.Save(resizeStream, ImageFormat.Jpeg);
            //                    PreparedDocuments.Add(new PatientImageModel
            //                    {
            //                        Name = InsuranceCardBack.FileName,
            //                        DocumentTypeId = insDocType.Id,
            //                        Image = resizeStream.ToArray(),
            //                        Description = "Back"
            //                    });
            //                }
            //            }
            //        }
            //    }
            //}

            //if (DocumentUploads != null)
            //{
            //    foreach (var formFile in DocumentUploads.Where(x => x.Length > 0))
            //    {
            //        using (var stream = new MemoryStream())
            //        {
            //            await formFile.CopyToAsync(stream);

            //            PreparedDocuments.Add(new PatientImageModel
            //            {
            //                Name = formFile.FileName,
            //                DocumentTypeId = DocumentTypes.Select(x => x.Id).FirstOrDefault(),
            //                Image = stream.ToArray()
            //            });
            //        }
            //    }
            //}

        }

        #endregion Private Members

    }
}