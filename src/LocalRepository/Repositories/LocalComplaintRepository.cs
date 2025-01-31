using Cts.Domain.Entities.Attachments;
using Cts.Domain.Entities.ComplaintActions;
using Cts.Domain.Entities.Complaints;
using Cts.Domain.Entities.ComplaintTransitions;
using Cts.TestData;
using System.Linq.Expressions;

namespace Cts.LocalRepository.Repositories;

public sealed class LocalComplaintRepository(
    IAttachmentRepository attachmentRepository,
    IComplaintActionRepository actionRepository,
    IComplaintTransitionRepository transitionRepository)
    : BaseRepository<Complaint, int>(ComplaintData.GetComplaints), IComplaintRepository
{
    public async Task<Complaint?> FindIncludeAllAsync(int id, bool includeDeletedActions = false,
        CancellationToken token = default) =>
        await GetComplaintDetailsAsync(await FindAsync(id, token).ConfigureAwait(false), includeDeletedActions, token)
            .ConfigureAwait(false);

    public async Task<Complaint?> FindIncludeAllAsync(Expression<Func<Complaint, bool>> predicate,
        bool includeDeletedActions = false, CancellationToken token = default) =>
        await GetComplaintDetailsAsync(await FindAsync(predicate, token).ConfigureAwait(false), includeDeletedActions,
            token).ConfigureAwait(false);

    private async Task<Complaint?> GetComplaintDetailsAsync(Complaint? complaint, bool includeDeletedActions,
        CancellationToken token)
    {
        if (complaint is null) return null;

        complaint.Attachments.Clear();
        complaint.Attachments.AddRange((await attachmentRepository
                .GetListAsync(attachment => attachment.Complaint.Id == complaint.Id && !attachment.IsDeleted, token)
                .ConfigureAwait(false))
            .OrderByDescending(attachment => attachment.UploadedDate)
            .ThenBy(attachment => attachment.FileName)
            .ThenBy(attachment => attachment.Id));

        complaint.ComplaintActions.Clear();
        complaint.ComplaintActions.AddRange((await actionRepository
                .GetListAsync(action => action.Complaint.Id == complaint.Id &&
                    (!action.IsDeleted || includeDeletedActions), token).ConfigureAwait(false))
            .OrderByDescending(action => action.ActionDate)
            .ThenByDescending(action => action.EnteredDate)
            .ThenBy(action => action.Id));

        complaint.ComplaintTransitions.Clear();
        complaint.ComplaintTransitions.AddRange((await transitionRepository
                .GetListAsync(transition => transition.Complaint.Id == complaint.Id, token).ConfigureAwait(false))
            .OrderBy(transition => transition.CommittedDate).ThenBy(transition => transition.Id));

        return complaint;
    }

    public Task InsertTransitionAsync(ComplaintTransition transition, bool autoSave = true,
        CancellationToken token = default) =>
        transitionRepository.InsertAsync(transition, autoSave, token);

    // Local repository requires ID to be manually set.
    public int? GetNextId() => Items.Select(e => e.Id).Max() + 1;
}
