using SubhadraSolutions.Utils.Contracts;

namespace SubhadraSolutions.Utils.Execution.Abstractions;

public abstract class AbstractNamed : INamed
{
    public string Name { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is not INamed named)
        {
            return false;
        }

        return Name.Equals(named.Name);
    }

    public override int GetHashCode()
    {
        if (Name == null)
        {
            return 0;
        }

        return Name.GetHashCode();
    }

    public string GetName()
    {
        return Name;
    }
}