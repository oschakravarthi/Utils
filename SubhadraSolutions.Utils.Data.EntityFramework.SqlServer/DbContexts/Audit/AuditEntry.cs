using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SubhadraSolutions.Utils.Data.EntityFramework.SqlServer.DbContexts.Audit;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public string EventType { get; set; }
    public List<string> ChangedValues { get; set; } = [];
    public Dictionary<string, object> OldValues { get; set; } = [];
    public Dictionary<string, object> NewValues { get; set; } = [];
    public string RecordID;

    public List<PropertyEntry> TemporaryProperties { get; } = [];

    public AuditLog ToAudit(string userName)
    {
        TableAttribute tableAttr = Entry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
        string tableName = tableAttr != null ? tableAttr.Name : Entry.Entity.GetType().Name;
        var changeTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pacific Standard Time");

        return new AuditLog()
        {
            AuditLogID = Guid.NewGuid(),
            UserName = userName,
            EventDatePST = changeTime,
            EventType = EventType,
            TableName = tableName,
            RecordID = RecordID,
            ChangedColumns = string.Join(",", ChangedValues),
            OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
            NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues),
        };
    }
}