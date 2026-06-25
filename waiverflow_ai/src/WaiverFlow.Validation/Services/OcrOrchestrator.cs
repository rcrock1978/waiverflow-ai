using System.Net.Http.Json;
using System.Text.Json;
using WaiverFlow.Validation.Entities;

namespace WaiverFlow.Validation.Services;

public class OcrOrchestrator
{
    private readonly HttpClient _http;
    private readonly ILogger<OcrOrchestrator> _log;

    public OcrOrchestrator(IHttpClientFactory httpFactory, ILogger<OcrOrchestrator> log)
    {
        _http = httpFactory.CreateClient("AiService");
        _log = log;
    }

    public async Task<ComplianceDoc> ProcessDocumentAsync(ComplianceDoc doc, Stream documentStream)
    {
        _log.LogInformation("OCR: Processing document {DocId} via AI service", doc.Id);

        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(documentStream), "document", "waiver.pdf");

        try
        {
            var response = await _http.PostAsync("/api/v1/ai/documents/extract", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OcrApiResponse>();

            doc.OCRStatus = result?.Valid == true ? "completed" : "failed";
            doc.OCRConfidence = result?.Confidence ?? 0;
            doc.ExtractedFields = JsonSerializer.Serialize(result?.Fields ?? []);
            doc.ValidationErrors = result?.Errors is { Count: > 0 }
                ? JsonSerializer.Serialize(result.Errors) : null;

            _log.LogInformation("OCR: Completed for {DocId} — success={Success}, confidence={Conf}",
                doc.Id, doc.OCRStatus == "completed", doc.OCRConfidence);
        }
        catch (HttpRequestException ex)
        {
            _log.LogError(ex, "OCR: AI service call failed for {DocId}", doc.Id);
            doc.OCRStatus = "failed";
            doc.ValidationErrors = JsonSerializer.Serialize(new[] { $"OCR service unavailable: {ex.Message}" });
        }

        return doc;
    }
}

public class OcrApiResponse
{
    public bool Valid { get; set; }
    public double Confidence { get; set; }
    public Dictionary<string, string> Fields { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}
