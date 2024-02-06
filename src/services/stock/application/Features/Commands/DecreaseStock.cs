using application.Abstractions;
using core_domain.Entities;
using core_messages.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace application.Features.Commands
{
    public static class DecreaseStock
    {
        #region Command
        public class Command : IRequest
        {
            public Guid MessageId { get; set; }
            public Guid OrderId { get; set; }
            public List<OrderItem> Items { get; set; }
        }

        public class OrderItem
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IUnitOfWork _uow;
            private readonly IConfiguration _configuration;
            public CommandHandler(IUnitOfWork uow, IConfiguration configuration)
            {
                this._uow = uow;
                this._configuration = configuration;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                if (await this._uow.InboxMessageRepo.FindOneAsync(x => x.MessageId == request.MessageId.ToString()
                                                                    && x.ConsumerType == this.GetType().FullName) != null)
                {
                    return;
                }

                var stock = await this._uow.StockRepo.FindOneAsync(x => true);
                if (stock == null)
                {
                    await StoreStockNotDecreasedEventAsync(this._uow, this._configuration, request.OrderId, request.MessageId);
                    return;
                }

                try
                {
                    var session = await this._uow.StartNewSessionAsync();

                    var stockProductIds = stock.Products.Select(x => Guid.Parse(x.Id)).Distinct().ToList();
                    var orderProductIds = request.Items.Select(x => x.ProductId).Distinct().ToList();
                    if (orderProductIds.Except(stockProductIds).Any())
                    {
                        await this._uow.RollbackAsync();
                        await StoreStockNotDecreasedEventAsync(this._uow, this._configuration, request.OrderId, request.MessageId);
                        return;
                    }

                    foreach (var orderProductId in orderProductIds)
                    {
                        var stockProduct = stock.Products.Single(x => x.Id == orderProductId.ToString());
                        var orderProduct = request.Items.Single(x => x.ProductId == orderProductId);
                        if (stockProduct.Quantity - orderProduct.Quantity <= 0)
                        {
                            await this._uow.RollbackAsync();
                            await StoreStockNotDecreasedEventAsync(this._uow, this._configuration, request.OrderId, request.MessageId);
                            return;
                        }

                        stockProduct.Quantity -= orderProduct.Quantity;
                    }

                    await this._uow.StockRepo.ReplaceOneAsync(stock, session);

                    await this._uow.InboxMessageRepo.InsertOneAsync(new InboxMessage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ConsumerType = this.GetType().FullName,
                        MessageId = request.MessageId.ToString(),
                        CreatedOn = DateTime.Now
                    }, session);

                    await this._uow.OutboxMessageRepo.InsertOneAsync(new OutboxMessage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ServiceName = "stock",
                        Type = typeof(StockDecreasedEvent).AssemblyQualifiedName,
                        Message = JsonSerializer.Serialize(new StockDecreasedEvent
                        {
                            OrderId = request.OrderId
                        }),
                        CreatedOn = DateTime.Now
                    }, session);

                    await this._uow.CommitAsync();

                }
                catch (Exception ex)
                {
                    await this._uow.RollbackAsync();
                    throw;
                }
            }
        }
        #endregion

        private static async Task StoreStockNotDecreasedEventAsync(IUnitOfWork uow, IConfiguration config, Guid orderId, Guid messageId)
        {
            try
            {
                var session = await uow.StartNewSessionAsync();

                await uow.InboxMessageRepo.InsertOneAsync(new InboxMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    ConsumerType = typeof(CommandHandler).FullName,
                    MessageId = messageId.ToString(),
                    CreatedOn = DateTime.Now
                }, session);

                await uow.OutboxMessageRepo.InsertOneAsync(new OutboxMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    ServiceName = "stock",
                    Type = typeof(StockNotDecreasedEvent).AssemblyQualifiedName,
                    Message = JsonSerializer.Serialize(new StockNotDecreasedEvent
                    {
                        OrderId = orderId
                    }),
                    CreatedOn = DateTime.Now
                }, session);

                await uow.CommitAsync();
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                throw;
            }
        }
    }
}
