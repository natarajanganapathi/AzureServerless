namespace Incubation.AzConf.Repository;

public class ParticipantRepository : BaseRepository<Participant>
{
    public ParticipantRepository(DbContext context) : base(context) { }
    // internal override IMongoCollection<Participant> Collection => _context.Participant;


    public async Task<Participant> GetByEmailAsync(string email)
    {
        var filter = Builders<Participant>
                    .Filter
                    .Eq("EMail", email);
        return await GetByFilter(filter).FirstOrDefaultAsync();
    }
}
