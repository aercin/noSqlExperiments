using application.Abstractions;
using core_application.Abstractions;
using MediatR;

namespace application.Features.Commands
{
    public static class MessageDispatch
    {
        #region Command
        public class Command : IRequest
        {
        }
        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IUnitOfWork _uow;
            private readonly IEventDispatcher _dispatcher;
            public CommandHandler(IUnitOfWork uow,
                                  IEventDispatcher dispatcher)
            {
                this._uow = uow;
                this._dispatcher = dispatcher;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var outboxMessages = await this._uow.OutboxMessageRepo.FilterByAsync(x => x.ServiceName == "stock", option =>
                {
                    option.PageNumber = 1;
                    option.PageSize = 100;
                });

                foreach (var outboxMessage in outboxMessages)
                {
                    try
                    {
                        await this._dispatcher.DispatchEventAsync(outboxMessage.Topic, outboxMessage.Message);

                        await this._uow.OutboxMessageRepo.DeleteOneAsync(x => x.Id == outboxMessage.Id);
                    }
                    catch (Exception ex)
                    {
                        //loglama yapılabilir.
                    }
                }
            }
        }
        #endregion
    }
}
