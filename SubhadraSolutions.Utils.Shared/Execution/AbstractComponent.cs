using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Xml.Serialization;

namespace SubhadraSolutions.Utils.Execution;

[Serializable]
[XmlRoot("component")]
public abstract class AbstractComponent : AbstractInitializableAndDisposable, IComponent
{
    public string Name { get; set; } = "Component" + GeneralHelper.Identity;

    public override string ToString()
    {
        return Name;
    }

    protected override void Dispose(bool disposing)
    {
    }

    protected override void InitializeProtected()
    {
    }
}