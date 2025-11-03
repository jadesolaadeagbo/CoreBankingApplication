using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBanking.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse>:IPipelineBehavior<TRequest, TResponse> where TRequest:IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogInformation("Handling {CommandName} with payload: {@Request}", requestName, request);

            var timer = System.Diagnostics.Stopwatch.StartNew();
            var response = await next();
            timer.Stop();   

            _logger.LogInformation("Command {CommandName} with response: {ElapsedMilliseconds}ms", requestName, timer.ElapsedMilliseconds);
            return response;
        }
    }
}
