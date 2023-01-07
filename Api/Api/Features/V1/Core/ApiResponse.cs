namespace Api.Features.V1.Core;

public record ApiResponse<T>(T? Data, ErrorResponse[]? Errors)
{
    public ApiResponse(T? Data) : this(Data, null) { }
    public ApiResponse(ErrorResponse[] Errors) : this(default, Errors) { }
}

public record ErrorResponse(string Location, string[] Messages);
