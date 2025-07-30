using MediatR;

namespace CloudWorks.Application.Common.Interfaces.ICommand
{
    public interface ICommand<out TResponse> : IRequest<TResponse>;
}
