using SubhadraSolutions.Utils.Contracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SubhadraSolutions.Utils.Abstractions;

[Serializable]
[DataContract]
public abstract class AbstractIdentity : IIdentity
{
    protected AbstractIdentity()
    {
        Id = Guid.NewGuid();
    }

    [DataMember][Key][Column(Order = 0)] public virtual Guid Id { get; set; }
}