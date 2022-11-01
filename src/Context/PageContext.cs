namespace Incubation.AzConf.Context;
public class PageContext
{
    public PageContext(int pageNo = 0, int pageSize = 10)
    {
        PageNumber = pageNo;
        PageSize = pageSize;
    }
    public int PageNumber { get; set; }
    internal int PageSize { get; private set; }
    internal int Skip => (PageNumber > 1 ? PageNumber - 1 : 0) * PageSize;

}

public static class Extensions
{
    public static PageContext NextPage(this PageContext pc)
    {
        pc.PageNumber++;
        return pc;
    }
}