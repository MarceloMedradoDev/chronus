using Chronos.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Utils
{
    public class RotinaBuscaPontoMais : BackgroundService
    {
        private readonly ILogger<RotinaBuscaPontoMais> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public RotinaBuscaPontoMais(ILogger<RotinaBuscaPontoMais> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var agora = DateTime.Now;

                var proximaExecucao = agora.Date.AddHours(8);

                if (proximaExecucao <= agora)
                {
                    proximaExecucao = proximaExecucao.AddDays(1);
                }

                var tempoAguardar = proximaExecucao - agora;

                await Task.Delay(tempoAguardar);

                using var scope = _scopeFactory.CreateScope();

                var pontoMaisService =
                    scope.ServiceProvider
                        .GetRequiredService<IPontoMaisService>();

                await pontoMaisService.StartPontoMaisService(true);
            }
        }
    }
}
