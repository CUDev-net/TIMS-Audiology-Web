using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.BLL.VendorSync.Common;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;
using TIMS_X.DAL.DAL.UoWs;

using TIMSAppointment = TIMS_X.Core.Domain.Appointment;

namespace TIMS_X.BLL.VendorSync.Repositories;

public interface IVendorHAUnitRepository
{
	#region IVendorHAUnitRepository Members

	Task<List<Audigy.HAUnitReturned>> GetHAUnitsReturned( DateTime? fromDate, DateTime? toDate );

	Task<List<Audigy.HAUnitSold>> GetHAUnitsSold( DateTime? fromDate, DateTime? toDate );

	#endregion IVendorHAUnitRepository Members

}

public class VendorHAUnitRepository : IVendorHAUnitRepository
{
	#region Constructors

	public VendorHAUnitRepository( IPracticeUnitOfWork practiceUnitOfWork, IHaHistoryUnitOfWork haHistoryUnitOfWork,
		IPosDocumentUnitOfWork posDocumentUnitOfWork, IHaReturnUnitOfWork haReturnUnitOfWork )
	{
		_practiceUnitOfWork = practiceUnitOfWork;
		_haHistoryUnitOfWork = haHistoryUnitOfWork;
		_posDocumentUnitOfWork = posDocumentUnitOfWork;
		_haReturnUnitOfWork = haReturnUnitOfWork;
	}

	#endregion Constructors

	#region VendorHAUnitRepository Members

	public async Task<List<HAUnitReturned>> GetHAUnitsReturned( DateTime? fromDate, DateTime? toDate )
	{
		var unitsReturned = new List<HAUnitReturned>();
		var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();
		var creditMemos = _GetPosDocuments( PosDocumentType.CreditMemo, fromDate, toDate ).ToList();
		foreach( var creditMemo in creditMemos )
			unitsReturned.AddRange( await _GetUnitsReturnedFromCreditMemoAsync( creditMemo, practiceId ) );
		return unitsReturned;
	}

	public async Task<List<HAUnitSold>> GetHAUnitsSold( DateTime? fromDate, DateTime? toDate )
	{
		var unitsSold = new List<HAUnitSold>();
		var practiceId = await _practiceUnitOfWork.GetPracticeAudigyId();
		var invoices = _GetPosDocuments( PosDocumentType.Invoice, fromDate, toDate ).ToList();
		foreach( var invoice in invoices )
			unitsSold.AddRange( await _GetUnitsSoldFromInvoiceAsync( invoice, practiceId ) );
		return unitsSold;
	}

	#endregion VendorHAUnitRepository Members

	#region Fields

	private readonly IHaHistoryUnitOfWork _haHistoryUnitOfWork;
	private readonly IHaReturnUnitOfWork _haReturnUnitOfWork;
	private readonly IPosDocumentUnitOfWork _posDocumentUnitOfWork;
	private readonly IPracticeUnitOfWork _practiceUnitOfWork;

	#endregion Fields

	#region Private Members

	private IQueryable<PosDocument> _GetPosDocuments( PosDocumentType documentType, DateTime? fromDate, DateTime? toDate )
	{
		IQueryable<PosDocument> posDocumentQuery;
		if( fromDate.HasValue && toDate.HasValue )
		{
			posDocumentQuery = _posDocumentUnitOfWork.GetPosDocuments(
				posDoc => posDoc.DocumentType == documentType &&
						posDoc.UpdatedDate >= fromDate.Value &&
						posDoc.UpdatedDate <= toDate.Value &&
						posDoc.Final && !posDoc.Void,
				null, _Includes
			);
		}
		else if( fromDate.HasValue )
		{
			posDocumentQuery = _posDocumentUnitOfWork.GetPosDocuments(
				posDoc => posDoc.DocumentType == documentType &&
						posDoc.UpdatedDate >= fromDate.Value &&
						posDoc.Final && !posDoc.Void,
				null, _Includes
			);
		}
		else if( toDate.HasValue )
		{
			posDocumentQuery = _posDocumentUnitOfWork.GetPosDocuments(
				posDoc => posDoc.DocumentType == documentType &&
						posDoc.UpdatedDate <= toDate.Value &&
						posDoc.Final && !posDoc.Void,
				null, _Includes
			);
		}
		else
		{
			posDocumentQuery = _posDocumentUnitOfWork.GetPosDocuments(
				p => p.DocumentType == documentType && p.Final && !p.Void, null, _Includes );
		}
		return posDocumentQuery;
	}

	private async Task<List<HAUnitReturned>> _GetUnitsReturnedFromCreditMemoAsync( PosDocument invoice, string practiceId )
	{
		var unitsReturned = new List<HAUnitReturned>();
		var lineItems = invoice.PosLines.Where( x => x.TableID == 170 && x.AAItemID > 0 ).ToList();
		foreach( var lineItem in lineItems )
		{
			var haHistory = await _haHistoryUnitOfWork.GetHaHistory( lineItem.AAItemID, _Includes );
			if( haHistory == null || haHistory.Side == HaSide.Accessory )
				continue;
			var haUnitReturned = new HAUnitReturned
			{
				PracticeID = practiceId,
				PatientID = invoice.PatientId,
				AudiologistID = haHistory.ProviderId,
				TransactionID = lineItem.Id,
				HAUnitID = haHistory.Id,
				SerialNumber = haHistory.SerialNumber,
                Side = EnumUtilities.GetDescriptionFromEnum(haHistory.Side),
                ReturnAmount = lineItem.Amount.ToString( "F" ),
				LastUpdatedDate = invoice.UpdatedDate,
			};

			var originalSaleDate = string.Empty;
			var returnReason = string.Empty;
			var returnAmount = string.Empty;
			var returnDate = string.Empty;
			var returnNotes = string.Empty;

			var haReturn = await _haReturnUnitOfWork.GetReturnByHaHistory( haHistory.Id );
			if( haReturn != null )
			{
				returnDate = haReturn.ReturnDate.ToString();
				returnReason = haReturn.ReturnReason?.Name ?? string.Empty;
				returnNotes = haReturn.Notes;
			}
			if( haHistory.PurchaseDate.HasValue )
			{
				originalSaleDate = haHistory.PurchaseDate.Value.ToString();
			}
			haUnitReturned.ReturnDate = returnDate;
			haUnitReturned.OriginalSaleDate = originalSaleDate;
			haUnitReturned.ReturnReason = returnReason;
			haUnitReturned.Notes = returnNotes;
			unitsReturned.Add( haUnitReturned );
		}
		return unitsReturned;
	}

	private async Task<List<HAUnitSold>> _GetUnitsSoldFromInvoiceAsync( PosDocument invoice, string practiceId )
	{
		var unitsSold = new List<HAUnitSold>();
		var lineItems = invoice.PosLines.Where( x => x.TableID == 170 && x.AAItemID > 0 ).ToList();
		foreach( var lineItem in lineItems )
		{
			var haHistory = await _haHistoryUnitOfWork.GetHaHistory( lineItem.AAItemID, _Includes );
			if( haHistory == null || haHistory.Side == HaSide.Accessory )
				continue;
			var haUnitSold = new HAUnitSold
			{
				PracticeID = practiceId,
				PatientID = invoice.PatientId,
				AudiologistID = haHistory.ProviderId,
				TransactionID = lineItem.Id,
				HAUnitID = haHistory.Id,
				PurchaseDate = haHistory.PurchaseDate.HasValue ? haHistory.PurchaseDate.Value.ToString() : string.Empty,
				Side = EnumUtilities.GetDescriptionFromEnum( haHistory.Side ),
				SerialNumber = haHistory.SerialNumber,
				WarrantyDate = haHistory.WarrantyDate.HasValue ? haHistory.WarrantyDate.Value.ToString() : string.Empty,
				OrderDate = haHistory.HaOrder?.OrderDate.ToString() ?? string.Empty,
				UnitPrice = lineItem.Amount.ToString( "F" ),
				Notes = haHistory.Notes,
				LastUpdatedDate = invoice.UpdatedDate,
                InsertDateTime = invoice.DateCreated,
				AppointmentID = invoice.AppointmentId > 0 ? invoice.AppointmentId.ToString() : null,
			};

			var manufacturerName = string.Empty;
			var haTechnology = string.Empty;
			var batterySize = string.Empty;
			var orderDate = string.Empty;

			if( haHistory.Model != null )
			{
				manufacturerName = haHistory.Model.Manufacturer?.Name ?? string.Empty;
				batterySize = haHistory.Model.BatterySize?.Name ?? string.Empty;
				var sb = new StringBuilder( haHistory.Model.Name + " " );
				if( !string.IsNullOrEmpty( haHistory.Model.Style?.Name ) )
				{
					sb.Append( haHistory.Model.Style?.Name + " " );
				}
				if( !string.IsNullOrEmpty( haHistory.Model.HaType?.Name ) )
				{
					sb.Append( haHistory.Model.HaType?.Name );
				}
				haTechnology = sb.ToString().TrimEnd();
			}

			haUnitSold.HATechnology = haTechnology;
			haUnitSold.ManufacturerName = manufacturerName;
			haUnitSold.BatterySize = batterySize;

			// Discount will be in following line, must be flat discount with audigy-sync on name
			var discount = invoice.PosLines.FirstOrDefault( x => x.Seq == lineItem.Seq + 1 && x.TableID == 420 && x.ItemType == 5 );
			var discountString = string.Empty;
			var purchasePrice = lineItem.Amount;
			if( discount != null )
			{
				var discountAmount = -discount.Amount;
				discountString = discountAmount.ToString( "F" );
				purchasePrice -= discountAmount;
			}
			haUnitSold.Discount = discountString;
			haUnitSold.PurchasePrice = purchasePrice.ToString( "F" );

			unitsSold.Add( haUnitSold );
		}
		return unitsSold;
	}

	private IIncludableQueryable<PosDocument, object> _Includes( IQueryable<PosDocument> a )
	{
		return a.Include( x => x.PosLines );
	}

	private IIncludableQueryable<HaModel, object> _Includes( IQueryable<HaModel> h )
	{
		return h.Include( x => x.Style )
			.Include( x => x.HaType )
			.Include( x => x.Manufacturer );
	}

	private IIncludableQueryable<HaHistory, object> _Includes( IQueryable<HaHistory> h )
	{
		return h.Include( x => x.Model )
					.ThenInclude( m => m.Manufacturer )
				.Include( x => x.Model )
					.ThenInclude( m => m.BatterySize )
				.Include( x => x.Model )
					.ThenInclude( m => m.Style )
				.Include( x => x.Model )
					.ThenInclude( m => m.HaType )
				.Include( x => x.HaOrder );
	}

	#endregion Private Members

}