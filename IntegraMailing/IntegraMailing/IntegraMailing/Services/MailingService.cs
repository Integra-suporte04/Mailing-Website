using IntegraMailing.Controllers;
using IntegraMailing.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace IntegraMailing.Services
{
    public class MailingService : BackgroundService, IMailingService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MailingService> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        //private readonly ApplicationDbContext _dbContext;
        private readonly IServiceScopeFactory _scopeFactory;


        public MailingService(IServiceProvider serviceProvider, ILogger<MailingService> logger, IHostEnvironment hostEnvironment, IServiceScopeFactory scopeFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            //_dbContext = dbContext;
            _scopeFactory = scopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scopeFac = _scopeFactory.CreateScope())
            {
                var dbContext = scopeFac.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 18)
                    {
                        var campanhasEmExecucao = await dbContext.Campanhas
                            .Where(c => c.InProgress && !c.Paused)
                            .ToListAsync(stoppingToken);

                        foreach (var campanha in campanhasEmExecucao)
                        {
                            // Logica de execução de campanha
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                _logger.LogInformation("Mailing será iniciado");
                                var controller = scope.ServiceProvider.GetRequiredService<LoadCSVController>();
                                controller.ExecuteScript(campanha.Id);
                                _logger.LogInformation("Mailing executado... aguardando");

                                // Após a execução do script, atualizar o estado
                                campanha.ExecutionCount++;
                                campanha.InProgress = true;  // ou false, conforme a lógica desejada
                                await dbContext.SaveChangesAsync(stoppingToken);
                            }
                        }
                    }

                    // Aguardar antes de verificar novamente
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
           
        }

        public async Task<bool> StartMailing(int campanhaId)
        {
            using (var scopeFac = _scopeFactory.CreateScope())
            {
                var dbContext = scopeFac.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var campanha = await dbContext.Campanhas.FindAsync(campanhaId);
                if (campanha == null)
                {
                    return false;
                }

                campanha.InProgress = true;
                await dbContext.SaveChangesAsync();

                return true;
            }
        }


    }


   

    public interface IMailingService
    {
        Task<bool> StartMailing(int campanhaId);
    }

}
