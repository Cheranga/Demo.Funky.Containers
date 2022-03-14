using System.Threading.Tasks;
using FunkyContainers.Core;

namespace FunkyContainers.Infrastructure.DataAccess
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task<Result> ExecuteAsync(TCommand command);
    }
}