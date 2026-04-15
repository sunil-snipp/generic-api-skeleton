using Generic.Api.Application.Contracts.Reports;

namespace Generic.Api.Application.Abstractions.Reports;

public interface IReportService
{
    Task<IReadOnlyCollection<ReportSummaryDto>> GetReportsAsync(CancellationToken cancellationToken = default);

    Task<ReportDetailsDto?> GetReportByIdAsync(string reportId, CancellationToken cancellationToken = default);

    Task<EmbedTokenDto> CreateEmbedTokenAsync(string reportId, CancellationToken cancellationToken = default);
}
