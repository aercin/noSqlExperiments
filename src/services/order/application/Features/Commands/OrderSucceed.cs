using application.Abstractions;
using core_application.Common;
using core_domain.Entities;
using domain.Enumerations;
using MediatR;

namespace application.Features.Commands
{
    public static class OrderSucceed
    {
        #region Command
        public class Command : IRequest<Result>
        {
            public Guid OrderId { get; set; }

            public Guid MessageId { get; set; }
        }
        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command, Result>
        { 
            private readonly IUnitOfWork _uow;
            public CommandHandler(IUnitOfWork uow)
            {
                this._uow = uow; 
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await this._uow.InboxMessageRepo.FindOneAsync(x => x.MessageId == request.MessageId.ToString()
                                                                    && x.ConsumerType == this.GetType().FullName) != null)
                    return Result.Failure(new List<string> { "Sipariş sonuçlandırma daha önce ele alınmıştı" });

                var existedOrder = await this._uow.OrderRepo.FindOneAsync(x => x.Id == request.OrderId.ToString());
                if (existedOrder == null)
                    return Result.Failure(new List<string> { "Sonuçlandırılmak istenen sipariş bulunamadı" });

                try
                {
                    var session = await this._uow.StartNewSessionAsync();

                    existedOrder.Status = OrderStatus.Successed;

                    await this._uow.OrderRepo.ReplaceOneAsync(existedOrder, session);

                    await this._uow.InboxMessageRepo.InsertOneAsync(new InboxMessage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ConsumerType = this.GetType().FullName,
                        MessageId = request.MessageId.ToString(),
                        CreatedOn = DateTime.Now
                    }, session);

                    await this._uow.CommitAsync();
                }
                catch
                {
                    await this._uow.RollbackAsync();
                    throw;
                }

                return Result.Success();
            }
        }
        #endregion 
    }
}
