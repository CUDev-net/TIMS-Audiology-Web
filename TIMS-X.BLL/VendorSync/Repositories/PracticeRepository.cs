using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.VendorSync.Repositories
{
    public interface IPracticeRepository
    {
        Task<Practice> GetPractice();
    }

    public class PracticeRepository : IPracticeRepository
    {
        private readonly IPracticeUnitOfWork _practiceUnitOfWork;

        public PracticeRepository(IPracticeUnitOfWork practiceUnitOfWork)
        {
            _practiceUnitOfWork = practiceUnitOfWork;
        }

        public async Task<Practice> GetPractice()
        {
            var practice = await _practiceUnitOfWork.GetPracticeSummary();
            var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId(practice.OfficeCode);
            var practiceDo = new Practice
            {
                PracticeID = practiceId,
                PracticeCode = practice.OfficeCode.ToUpper() + practiceId,
                Name = practice.Name,
                Address1 = practice.BillingAddress1,
                Address2 = practice.BillingAddress2,
                City = practice.BillingCity,
                State = practice.BillingState,
                ZipCode = practice.BillingZipCode,
                Phone = practice.BillingPhoneNumber,
                Fax = practice.Fax,
                TaxId = practice.TaxId
            };
            return practiceDo;
        }
    }
}