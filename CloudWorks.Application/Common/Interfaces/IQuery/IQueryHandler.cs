using MediatR;

namespace CloudWorks.Application.Common.Interfaces.IQuery
{
    public interface IQueryHandler<in TQuery, TResponse>
        : IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse> where TResponse : notnull;
}
