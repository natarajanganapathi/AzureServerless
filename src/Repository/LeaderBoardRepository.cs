namespace Incubation.AzConf.Repository;

public class LeaderBoardRepository : BaseRepository<LeaderBoard>
{
    public LeaderBoardRepository(DbContext context) : base(context) { }
    public async Task<List<LeaderBoard>> GetLeaderBoard(PageContext pc = null)
    {
        pc = pc != null ? pc : new PageContext();
        var sorts = GetSortDef(new SortParam[]{
            new SortParam ( nameof(LeaderBoard.Score), SortOrder.Descending),
            new SortParam ( nameof(LeaderBoard.TimeTaken), SortOrder.Ascending)
        });
        var list = await GetAsync(sorts: sorts, pc: pc);
        var result = list.Select((item, index) => { item.Rank = pc.Skip + 1 + index; return item; }).ToList();
        return result;
    }

    public async Task<LeaderBoard> GetLeaderBoardDetailsByParticipantId(string participantId)
    {
        var filter = GetFilterDef().Eq(nameof(LeaderBoard.ParticipantId), ObjectId.Parse(participantId));
        return await GetByFilter(filter).FirstOrDefaultAsync();
    }

    public async Task ReceivedGoodies(string participantId, bool received)
    {
        var filter = GetFilterDef().Eq(nameof(LeaderBoard.ParticipantId), ObjectId.Parse(participantId));
        var update = GetUpdateDef().Set(nameof(LeaderBoard.ReceivedGoodies), received);
        await UpdateAsync(filter, update);
    }
}
