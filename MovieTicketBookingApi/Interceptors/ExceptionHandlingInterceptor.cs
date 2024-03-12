using Core.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Status = Grpc.Core.Status;

namespace MovieTicketBookingApi.Interceptors;

public class ExceptionHandlingInterceptor : Interceptor
{
	public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
		TRequest request,
		ServerCallContext context,
		UnaryServerMethod<TRequest, TResponse> continuation)
	{
		try
		{
			return await continuation(request, context);
		}
		catch (Exception ex)
		{
			throw new RpcException(new Status(GetGrpcStatusCode(ex), ex.Message));
		}
	}

	private static StatusCode GetGrpcStatusCode(Exception exception) =>
		exception switch
		{
			NullReferenceException => StatusCode.NotFound,
			IncorrectPasswordException => StatusCode.Cancelled,
			_ => StatusCode.Internal
		};
}
