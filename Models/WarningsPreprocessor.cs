using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace BIMicon.BIMiconToolbar.Models
{
    /// <summary>
    /// Class to handle warnings before it is displayed in the Revit interface
    /// </summary>
    internal class WarningsPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failures = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor failure in failures)
            {
                FailureSeverity failureSeverity = failuresAccessor.GetSeverity();

                if (failureSeverity == FailureSeverity.Warning)
                    failuresAccessor.DeleteWarning(failure);
                else
                    return FailureProcessingResult.ProceedWithRollBack;
            }
            return FailureProcessingResult.Continue;
        }
    }
}
