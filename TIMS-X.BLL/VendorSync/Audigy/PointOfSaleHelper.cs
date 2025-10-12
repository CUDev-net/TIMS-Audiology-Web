using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.VendorSync.Audigy;

public interface IPointOfSaleHelper
{
    IQueryable<PosDocument> CreateLineItemQuery(DateTime? fromDate, DateTime? toDate, PosDocumentType documentType);
}

public class PointOfSaleHelper : IPointOfSaleHelper
{
    private readonly IPosDocumentUnitOfWork _posDocumentUnitOfWork;

    public PointOfSaleHelper(IPosDocumentUnitOfWork posDocumentUnitOfWork)
    {
        _posDocumentUnitOfWork = posDocumentUnitOfWork;
    }

    public IQueryable<PosDocument> CreateLineItemQuery(DateTime? fromDate, DateTime? toDate,
        PosDocumentType documentType)
    {
        IQueryable<PosDocument> rawItems;
        // 170 is a hearing aid
        if (fromDate.HasValue && toDate.HasValue)
            rawItems = _posDocumentUnitOfWork.GetPosDocuments(p => p.DocumentType == documentType &&
                                                                   p.Final && !p.Void &&
                                                                   p.UpdatedDate >= fromDate &&
                                                                   p.UpdatedDate <= toDate.Value &&
                                                                   p.PosLines.Any(l =>
                                                                       l.TableID == 170 && p.DocumentType ==
                                                                       documentType),
                null, PosIncludes);
        else if (fromDate.HasValue)
            rawItems = _posDocumentUnitOfWork.GetPosDocuments(p => p.DocumentType == documentType &&
                                                                   p.UpdatedDate >= fromDate
                                                                   && p.Final && !p.Void
                                                                   && p.PosLines.Any(l =>
                                                                       l.TableID == 170 && p.DocumentType ==
                                                                       documentType), null, PosIncludes);
        else if (toDate.HasValue)
            rawItems = _posDocumentUnitOfWork.GetPosDocuments(p => p.DocumentType == documentType &&
                                                                   p.UpdatedDate <= toDate
                                                                   && p.Final && !p.Void
                                                                   && p.PosLines.Any(l =>
                                                                       l.TableID == 170 && p.DocumentType ==
                                                                       documentType), null, PosIncludes);
        else
            rawItems = _posDocumentUnitOfWork.GetPosDocuments(p => p.DocumentType == documentType
                                                                   && p.Final && !p.Void
                                                                   && p.PosLines.Any(l =>
                                                                       l.TableID == 170 && p.DocumentType ==
                                                                       documentType), null, PosIncludes);
        return rawItems;
    }

    public static PointOfSaleDocument MapPosDocumentToInvoice(PosDocument posDocument)
    {
        var pointOfSaleDocument = new PointOfSaleDocument
        {
            Id = posDocument.Id,
            PatientId = posDocument.PatientId,
            AppointmentId = posDocument.AppointmentId,
            ClinicId = posDocument.SiteId,
            Date = posDocument.DocumentDate,
            LastUpdate = posDocument.UpdatedDate
        };

        var orderedLines = posDocument.PosLines.OrderBy(p => p.Seq).ToList();
        var count = orderedLines.Count();
        for (var i = 0; i < count; i++)
        {
            var posLineItem = orderedLines[i];
            if (posLineItem.HaHistory == null || (HaSide)posLineItem.HaHistory.Side == HaSide.Accessory)
                continue;
            if (posLineItem.TableID != 170) continue;

            var ha = new PointOfSaleLine
            {
                Id = posLineItem.Id,
                SerialNumber = posLineItem.HaHistory.SerialNumber,
                Side = ((HaSide)posLineItem.HaHistory.Side).ToString(),
                BatterySize = posLineItem.HaHistory.Model?.BatterySize?.Name,
                Manufacturer = posLineItem.HaHistory.Model?.Manufacturer?.Name,
                HaType = posLineItem.HaHistory.Model?.HaType?.Name,
                Style = posLineItem.HaHistory.Model?.Style?.Name,
                OrderDate = posLineItem.HaHistory.HaOrder?.OrderDate,
                PurchaseDate = posDocument.DocumentDate,
                Price = posLineItem.Amount,
                Discount = 0,
                NetPrice = posLineItem.Amount
            };
            if (i + 1 < count)
            {
                // Look for discount
                var next = orderedLines[i + 1];
                if (next.TableID == 420 && next.ItemType == 5)
                {
                    ha.Discount = next.Amount;
                    ha.NetPrice = ha.Price + ha.Discount;
                }
            }

            pointOfSaleDocument.Lines.Add(ha);
        }

        return pointOfSaleDocument;
    }

    public static IIncludableQueryable<PosDocument, object> PosIncludes(IQueryable<PosDocument> doc)
    {
        return doc.Include(x => x.PosLines)
            .ThenInclude(p => p.HaHistory)
            .ThenInclude(m => m.Model)
            .ThenInclude(b => b.BatterySize)
            // EF core should optimize this
            .Include(x => x.PosLines)
            .ThenInclude(p => p.HaHistory)
            .ThenInclude(m => m.Model)
            .ThenInclude(b => b.Manufacturer)
            .Include(x => x.PosLines)
            .ThenInclude(p => p.HaHistory)
            .ThenInclude(m => m.Model)
            .ThenInclude(b => b.Style)
            .Include(x => x.PosLines)
            .ThenInclude(p => p.HaHistory)
            .ThenInclude(m => m.Model)
            .ThenInclude(b => b.HaType)
            .Include(x => x.PosLines)
            .ThenInclude(p => p.HaHistory)
            .ThenInclude(m => m.HaOrder);
    }
}