using application.Abstractions;
using core_application.Abstractions;
using core_application.Common;
using MediatR;
using System.Text.Json;

namespace application.Features.Commands
{
    public static class MessageDispatch
    {
        #region Command
        [IgnoreBehaviours]
        public class Command : IRequest<Result>
        {
        }
        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IUnitOfWork _uow;
            private readonly IEventDispatcher _dispatcher;

            public CommandHandler(IUnitOfWork uow,
                                  IEventDispatcher dispatcher)
            {
                this._uow = uow;
                this._dispatcher = dispatcher;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var outboxMessages = await this._uow.OutboxMessageRepo.FilterByAsync(x => x.ServiceName == "order", option =>
                {
                    option.PageNumber = 1;
                    option.PageSize = 100;
                });

                foreach (var outboxMessage in outboxMessages)
                {
                    try
                    {
                        var message = JsonSerializer.Deserialize(outboxMessage.Message, Type.GetType(outboxMessage.Type));

                        await this._dispatcher.DispatchEventAsync(message);

                        await this._uow.OutboxMessageRepo.DeleteOneAsync(x => x.Id == outboxMessage.Id);
                    }
                    catch (Exception ex)
                    {
                        //loglama yapılabilir.
                    }
                }

                return Result.Success();
            }
        }
        #endregion
    }
}
