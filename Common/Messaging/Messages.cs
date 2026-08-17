using MediatR;


public interface IBaseCommand
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand
{
}

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
