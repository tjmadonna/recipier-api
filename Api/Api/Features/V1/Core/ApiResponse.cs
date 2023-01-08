namespace Api.Features.V1.Core;

public record SuccessResponse<T>(T? Data);

public record FailureResponse(object Errors);
