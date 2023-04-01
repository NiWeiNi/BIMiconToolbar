using Autodesk.Revit.DB;
using BIMicon.BIMiconToolbar.Models;

namespace BIMicon.BIMiconToolbar.Helpers
{
    internal class RevitTransaction
    {
        /// <summary>
        /// Set supression of warning dialogs that happens inside a transaction
        /// </summary>
        /// <param name="transaction">The transaction to set the warning dialogs supression</param>
        public static void SetWarningDialogSupressor(Transaction transaction)
        {
            FailureHandlingOptions failrureHandlingOptions = transaction.GetFailureHandlingOptions();
            failrureHandlingOptions.SetFailuresPreprocessor(new WarningsPreprocessor());
            transaction.SetFailureHandlingOptions(failrureHandlingOptions);
        }
    }
}
