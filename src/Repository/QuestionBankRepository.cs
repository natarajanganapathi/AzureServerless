
namespace Incubation.AzConf.Repository;

public class QuestionBankRepository : BaseRepository<QuestionBank>
{
    public QuestionBankRepository(DbContext context) : base(context) { }

    public List<String> GetCategory()
    {
        var cateory = Collection.AsQueryable()
                     .GroupBy(x => x.Category)
                     .Select(x => x.Key).ToList();
        return cateory;
    }
}
