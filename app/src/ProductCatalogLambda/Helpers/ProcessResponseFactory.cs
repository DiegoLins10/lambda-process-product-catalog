using ProductCatalogLambda.Models;

namespace ProductCatalogLambda.Helpers;

public static class ProcessResponseFactory
{
    public static ProcessResponse Success(string message)
    {
        return new ProcessResponse
        {
            StatusCode = 201,
            Status = "OK",
            Message = message,
            Sucess = true
        };
    }

    public static ProcessResponse BadRequest(string message)
    {
        return new ProcessResponse
        {
            StatusCode = 400,
            Status = "BadRequest",
            Message = message,
            Sucess = false
        };
    }

    public static ProcessResponse Error(string message)
    {
        return new ProcessResponse
        {
            StatusCode = 500,
            Status = "Error",
            Message = message,
            Sucess = false
        };
    }
}
