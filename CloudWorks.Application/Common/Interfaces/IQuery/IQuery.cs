using MediatR;

namespace CloudWorks.Application.Common.Interfaces.IQuery
{
    public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull;
}
