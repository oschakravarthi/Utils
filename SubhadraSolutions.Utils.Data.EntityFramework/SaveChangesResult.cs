using System;

namespace SubhadraSolutions.Utils.Data.EntityFramework;

public class SaveChangesResult
{
    public SaveChangesResult(int numberofRowsChanged, Exception exceptionInAudit)
    {
        NumberofRowsChanged = numberofRowsChanged;
        ExceptionInAudit = exceptionInAudit;
    }

    public int NumberofRowsChanged { get; private set; }

    public Exception ExceptionInAudit { get; private set; }
}