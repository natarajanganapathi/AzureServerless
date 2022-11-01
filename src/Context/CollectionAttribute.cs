namespace Incubation.AzConf.Context;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class CollectionAttribute : Attribute
{
    public string Name { get; set; }
    public CollectionAttribute(string name)
    {
        Name = name;
    }
}
