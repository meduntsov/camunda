namespace WorkflowMvp.Api.Models;

public enum RequestStatus
{
    Draft = 0,
    PendingManagerApproval = 1,
    PendingFinanceApproval = 2,
    Approved = 3,
    Rejected = 4
}
