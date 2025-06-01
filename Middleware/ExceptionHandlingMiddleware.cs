using System.Net;
using System.Text.Json;
using CarAuctionManagementSystem.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is AggregateException aggEx && aggEx.InnerException is not null)
            exception = aggEx.InnerException;

        var statusCode = exception switch
        {
            CustomExceptions.VehicleNotFoundException => HttpStatusCode.NotFound,
            CustomExceptions.NoVehiclesFoundException => HttpStatusCode.NotFound,
            CustomExceptions.NoAuctionsFoundException => HttpStatusCode.NotFound,
            CustomExceptions.CannotUpdateVehicleType => HttpStatusCode.Conflict,
            CustomExceptions.AuctionAlreadyStartedException => HttpStatusCode.Conflict,
            CustomExceptions.AuctionCantStartedException => HttpStatusCode.BadRequest,
            CustomExceptions.AuctionAlreadyClosedException => HttpStatusCode.Conflict,
            CustomExceptions.AuctionNotActiveException => HttpStatusCode.Conflict,
            CustomExceptions.AuctionWithoutBidsException => HttpStatusCode.BadRequest,
            CustomExceptions.AuctionDoestHaveVehicleException => HttpStatusCode.BadRequest,
            CustomExceptions.AuctionNotFoundException => HttpStatusCode.NotFound,
            CustomExceptions.BidAmountTooLowException => HttpStatusCode.BadRequest,
            CustomExceptions.BidAmountLowerThanStartingPriceException => HttpStatusCode.BadRequest,
            CustomExceptions.ValidationException => HttpStatusCode.BadRequest,
            CustomExceptions.AuctionSameIdException => HttpStatusCode.Conflict,
            CustomExceptions.InvalidVehicleTypeException => HttpStatusCode.BadRequest,
            CustomExceptions.BidderAlreadyExistsException => HttpStatusCode.Conflict,
            CustomExceptions.BidderNotFoundByIdException => HttpStatusCode.NotFound,
            CustomExceptions.BidderHasPlacedBidsException => HttpStatusCode.Conflict,
            CustomExceptions.VehicleHaveActiveAuctionException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };

        var problemDetails = new
        {
            status = (int)statusCode,
            title = "An error occurred",
            detail = exception.Message
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}
