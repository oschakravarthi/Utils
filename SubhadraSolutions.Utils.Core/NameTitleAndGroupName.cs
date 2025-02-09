namespace SubhadraSolutions.Utils;

public class NameTitleAndGroupName : NameAndTitle
{
    public NameTitleAndGroupName(string name, string title, string groupName) : base(name, title)
    {
        this.GroupName = groupName;
    }

    public string GroupName { get; }
}