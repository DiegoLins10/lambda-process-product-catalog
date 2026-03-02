namespace ProductCatalogLambda.Models;

public class ProcessResponse
{
    public int StatusCode { get; set; }
    public string? Status { get; set; }
    public string? Message { get; set; }
    public bool Sucess { get; set; }
}
