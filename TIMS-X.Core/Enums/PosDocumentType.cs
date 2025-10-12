using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Enums
{
    public enum PosDocumentType
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Undefined,
        /// <summary>
        /// Invoice
        /// </summary>
        Invoice,
        /// <summary>
        /// Credit
        /// </summary>
        CreditMemo,
        /// <summary>
        /// Payment
        /// </summary>
        Payment,
        /// <summary>
        /// Payment
        /// </summary>
        Applied,
        /// <summary>
        /// InsuranceTransfer
        /// </summary>
        InsuranceTransfer
    }
}
