namespace Incubation.AzConf.Repository;

public class ParticipantRepository : BaseRepository<Participant>
{
    public ParticipantRepository(DbContext context) : base(context) { }
    public async Task<Participant> GetByEmailAsync(string email)
    {
        var filter = GetFilterDef().Eq("EMail", email);
        return await GetByFilter(filter).FirstOrDefaultAsync();
    }
}
