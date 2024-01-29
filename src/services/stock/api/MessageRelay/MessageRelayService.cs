using application.Features.Commands;
using MediatR;

namespace api.MessageRelay
{
    public class MessageRelayService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public MessageRelayService(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                using (var scope = this._serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetService<IMediator>();
                    await mediator.Send(new MessageDispatch.Command());
                } 
            }
        }
    }
}
