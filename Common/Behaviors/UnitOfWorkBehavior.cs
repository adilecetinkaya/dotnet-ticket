using MediatR;

public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

    public UnitOfWorkBehavior(
        IUnitOfWork unitOfWork,
        ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IBaseCommand)
        {
            return await next();
        }

        var response = await next();

        var affected = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (affected > 0)
        {
            _logger.LogInformation(
                "{RequestName}: {Affected} satir kaydedildi",
                typeof(TRequest).Name, affected);
        }

        return response;
    }
}
