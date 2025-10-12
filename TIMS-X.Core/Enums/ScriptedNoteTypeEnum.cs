using System.ComponentModel;

namespace TIMS_X.Core.Enums
{
    public enum ScriptedNoteTypeEnum
    {
        /// <summary>
        ///     first
        /// </summary>
        [Description("Chart")] Chart = 0,

        /// <summary>
        ///     second
        /// </summary>
        [Description("Diagnosis")] Diagnosis = 1,

        /// <summary>
        ///     third
        /// </summary>
        [Description("Recommendation")] Recommendation = 2,

        /// <summary>
        ///     Marketing
        /// </summary>
        [Description("Communication")] Marketing = 3,

        /// <summary>
        ///     Marketing
        /// </summary>
        [Description("Office")] Office = 4,

        /// <summary>
        ///     SLP Progress Notes
        /// </summary>
        [Description("SLP Progress Notes")] SLPProgressNotes = 5,

        /// <summary>
        ///     SLP Diagnosis
        /// </summary>
        [Description("SLP Diagnosis")] SLPDiagnosis = 6,

        /// <summary>
        ///     SLP Recommendation
        /// </summary>
        [Description("SLP Recommendation")] SLPRecommendation = 7,

        /// <summary>
        ///     SLP Goals
        /// </summary>
        [Description("SLP Goals")] SLPGoals = 8,

        [Description("Patient Messaging")] PatientMessaging = 9,

        /// <summary>
        ///     third
        /// </summary>
        [Description("Editor Only")] EditorOnly = 99
    }
}