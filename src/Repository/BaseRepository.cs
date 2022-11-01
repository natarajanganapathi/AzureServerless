namespace Incubation.AzConf.Repository;

public abstract class BaseRepository<T> where T : BaseModel
{
    internal readonly DbContext _context;
    private IMongoCollection<T> _collection;
    internal virtual IMongoCollection<T> Collection
    {
        get
        {
            if (_collection == null)
            {
                var collectionName = typeof(T).GetCustomAttribute<CollectionAttribute>()?.Name;
                _collection = _context.GetCollection<T>(collectionName);
            }
            return _collection;
        }
    }
    public BaseRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAsync(string[] projection = null, FilterDefinition<T> filter = null, List<SortDefinition<T>> sorts = null, PageContext pc = null)
    {
        pc = pc != null ? pc : new PageContext();
        filter = filter != null ? filter : GetFilterDef().Empty;
        var query = GetByFilter(filter);

        if (projection != null && projection.Length > 0)
        {
            projection.Select(x => Builders<T>.Projection.Include(x))
            .ToList()
            .ForEach(x => query = query.Project(x) as IFindFluent<T, T>);
        }
        if (sorts != null && sorts.Count() > 0)
        {
            var sort = GetSortDef().Combine(sorts);
            query = query.Sort(sort);
        }
        var result = await query.Skip(pc.Skip).Limit(pc.PageSize).ToListAsync();
        return result;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var filter = GetFilterDef().Eq("_id", id);
        return await GetByFilter(filter).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetByIdsAsync(string[] ids)
    {
        var filter = GetFilterDef().In(x => x.Id, ids);
        return await GetByFilter(filter).ToListAsync();
    }

    public async Task AddAsync(T data)
    {
        await Collection.InsertOneAsync(data);
    }
    public async Task AddManyAsync(IEnumerable<T> data)
    {
        await Collection.InsertManyAsync(data);
    }

    public async Task UpsertAsync(T data)
    {
        var filter = GetFilterDef().Eq("_id", ObjectId.Parse(data.Id));
        var update = GetUpdateDefinition(data);
        await UpsertAsync(filter, update);
    }

    public async Task<UpdateResult> UpsertAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        return await Collection
                     .UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task<UpdateResult> UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        return await Collection
                     .UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
    }

    public async Task<UpdateResult> UpdateAsync(Guid id, T data)
    {
        var filter = GetFilterDef().Eq("_id", id);
        var update = GetUpdateDefinition(data);
        return await UpdateAsync(filter, update);
    }

    public async Task<ReplaceOneResult> ReplaceAsync(T data)
    {
        return await Collection
                     .ReplaceOneAsync(Builders<T>.Filter.Eq("_id", data.Id), data);
    }
    public async Task DeleteAsync(Guid id)
    {
        var filter = GetFilterDef().Eq("_id", id);
        await Collection.DeleteOneAsync(filter);
    }

    public async Task<long> GetCountAsync(FilterDefinition<T> filter = null)
    {
        filter = filter != null ? filter : Builders<T>.Filter.Empty;
        var result = Collection.Find(filter);
        return await result.CountDocumentsAsync();
    }

    internal IFindFluent<T, T> GetByFilter(FilterDefinition<T> filter)
    {
        return Collection.Find(filter);
    }

    internal UpdateDefinition<T> GetUpdateDefinition(T data)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public);
        var update = Builders<T>.Update;
        foreach (var property in properties)
        {
            var val = property.GetValue(data);
            if (val != null)
            {
                update.Set(property.Name, val);
            }
        }
        return update.Combine();
    }

    internal SortDefinitionBuilder<T> GetSortDef()
    {
        return Builders<T>.Sort;
    }
    internal List<SortDefinition<T>> GetSortDef(SortParam[] sorts)
    {
        var sortDef = new List<SortDefinition<T>>();
        foreach (var sort in sorts)
        {
            var sortBuilder = Builders<T>.Sort;
            var sortDefinition = sort.Order == SortOrder.Descending ? GetSortDef().Descending(sort.Field) : GetSortDef().Ascending(sort.Field);
            sortDef.Add(sortDefinition);
        }
        return sortDef;
    }
    internal FilterDefinitionBuilder<T> GetFilterDef()
    {
        return Builders<T>.Filter;
    }

    internal UpdateDefinitionBuilder<T> GetUpdateDef()
    {
        return Builders<T>.Update;
    }
}

public enum SortOrder { Ascending, Descending }

public class SortParam
{
    public SortParam(string field, SortOrder order)
    {
        Field = field;
        Order = order;
    }
    public string Field { get; set; }
    public SortOrder Order { get; set; }
}
