using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.Core.Enums;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.VendorSync.Repositories;

public interface IPointOfSaleRepository
{
    Task<List<PointOfSaleDocument>> GetInvoices(DateTime? fromDate, DateTime? toDate);
    Task<PointOfSaleDocument> GetPosDocument(int id);
    Task<List<PointOfSaleDocument>> GetReturns(DateTime? fromDate, DateTime? toDate);
}

public class PointOfSaleRepository : IPointOfSaleRepository
{
    private readonly IPointOfSaleHelper _pointOfSaleHelper;
    private readonly IPosDocumentUnitOfWork _posDocumentUnitOfWork;

    public PointOfSaleRepository(IPosDocumentUnitOfWork posDocumentUnitOfWork, IPointOfSaleHelper pointOfSaleHelper)
    {
        _posDocumentUnitOfWork = posDocumentUnitOfWork;
        _pointOfSaleHelper = pointOfSaleHelper;
    }

    public async Task<PointOfSaleDocument> GetPosDocument(int id)
    {
        var posDocument = await _posDocumentUnitOfWork.GetPosDocument(id, PointOfSaleHelper.PosIncludes);

        if (posDocument == null) return null;
        return PointOfSaleHelper.MapPosDocumentToInvoice(posDocument);
    }

    public async Task<List<PointOfSaleDocument>> GetInvoices(DateTime? fromDate, DateTime? toDate)
    {
        var rawItems = _pointOfSaleHelper.CreateLineItemQuery(fromDate, toDate, PosDocumentType.Invoice);

        var invoices = new List<PointOfSaleDocument>();
        foreach (var posDocument in await rawItems.ToListAsync())
        {
            var item = PointOfSaleHelper.MapPosDocumentToInvoice(posDocument);
            invoices.Add(item);
        }

        return invoices;
    }

    public async Task<List<PointOfSaleDocument>> GetReturns(DateTime? fromDate, DateTime? toDate)
    {
        var rawItems = _pointOfSaleHelper.CreateLineItemQuery(fromDate, toDate, PosDocumentType.CreditMemo);

        var returns = new List<PointOfSaleDocument>();
        foreach (var posDocument in await rawItems.ToListAsync())
        {
            var item = PointOfSaleHelper.MapPosDocumentToInvoice(posDocument);
            returns.Add(item);
        }

        return returns;
    }
}