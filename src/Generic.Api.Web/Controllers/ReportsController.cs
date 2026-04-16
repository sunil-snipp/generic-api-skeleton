using Generic.Api.Application.Reports;
using Generic.Api.Application.Reports.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Generic.Api.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ReportSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ReportSummaryDto>>> GetReports(CancellationToken cancellationToken)
    {
        var reports = await reportService.GetReportsAsync(cancellationToken);
        return Ok(reports);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReportDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportDetailsDto>> GetReportById(string id, CancellationToken cancellationToken)
    {
        var report = await reportService.GetReportByIdAsync(id, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }

    [HttpPost("{id}/embed-token")]
    [ProducesResponseType(typeof(EmbedTokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EmbedTokenDto>> GenerateEmbedToken(string id, CancellationToken cancellationToken)
    {
        var token = await reportService.CreateEmbedTokenAsync(id, cancellationToken);
        return Ok(token);
    }
}
