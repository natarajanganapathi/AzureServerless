namespace Incubation.AzConf.Context;
public class CollectionAttribute : Attribute
{
    public string Name { get; set; }
    public CollectionAttribute(string name)
    {
        Name = name;
    }
}
