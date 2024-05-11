using System.ComponentModel.DataAnnotations;
using big_agi_syncserver.Data;
using Microsoft.EntityFrameworkCore;

namespace big_agi_syncserver;

public static partial class Endpoints
{
    public static bool IsSyncKeyValid(string? syncKey)
        => !string.IsNullOrEmpty(syncKey) && syncKey.Length < 50;


    public static async Task<long> GetLastSync(string syncKey, ApplicationDbContext context, CancellationToken cancellationToken)
    {
        // return error if syncKey is not valid
        if (!IsSyncKeyValid(syncKey))
            throw new ValidationException("syncKey is not valid");

        var lastSyncTimestamp = await context.Conversations
            .Where(x => x.SyncKey == syncKey)
            .OrderByDescending(x => x.Updated)
            .Select(x => x.Updated)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastSyncTimestamp < 0)
            return 0;

        return lastSyncTimestamp;
    }
}