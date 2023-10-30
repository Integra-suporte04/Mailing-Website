using IntegraMailing.Controllers;
using IntegraMailing.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
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
            /*
            using (var scopeFac = _scopeFactory.CreateScope())
            {
                var dbContext = scopeFac.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                while (!stoppingToken.IsCancellationRequested)
                {
                    //System.Diagnostics.Debug.WriteLine("EXECUTANDO LOOP");
                    if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 18)
                    {
                        var campanhasEmExecucao = await dbContext.Campanhas.ToListAsync(stoppingToken);

                        foreach (var campanha in campanhasEmExecucao)
                        {
                            if (campanha.Status == "Finalizado")
                                continue;
                            else if (campanha.Evolution == 100)
                            {
                                campanha.Status = "Finalizado";
                                campanha.InProgress = false;
                                await dbContext.SaveChangesAsync(stoppingToken);
                                continue;  // Skip the rest of the loop for this campaign
                            }

                                System.Diagnostics.Debug.WriteLine("Veio até antes do pause");
                            if (campanha.Paused)
                                continue;

                                System.Diagnostics.Debug.WriteLine(campanha.Status + " " + campanha.InProgress);

                            if(campanha.Status == "Enviado" && campanha.InProgress)
                            {
                                System.Diagnostics.Debug.WriteLine("Entrou aqui");

                                using (var scope = _serviceProvider.CreateScope())
                                {

                                    campanha.Status = "Em Progresso...";
                                System.Diagnostics.Debug.WriteLine("Entrou aqui tbm");
                                    _logger.LogInformation("Mailing será iniciado");
                                    var controller = scope.ServiceProvider.GetRequiredService<LoadCSVController>();
                                    controller.ExecuteScript(campanha.Id);
                                    _logger.LogInformation("Mailing executado... aguarde");

                                    // Após a execução do script, atualizar o estado
                                    //campanha.ExecutionCount++;
                                    await dbContext.SaveChangesAsync(stoppingToken);
                                }
                            }
                            
 
                        }
                    }

                    // Aguardar antes de verificar novamente
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
           */
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

                campanha.Executed = true;
                await dbContext.SaveChangesAsync();

                return true;
            }
        }


    }


   

    public interface IMailingService
    {
        Task<bool> StartMailing(int campanhaId);
    }
    //public enum MailingStatus
    //{
    //    [Description("Enviado")]
    //    Enviado,
    //    [Description("Em Progresso...")]
    //    EmProgresso,
    //    [Description("Pausado")]
    //    Pausado,
    //    [Description("Finalizado")]
    //    Finalizado

    //}
    //public static class MailingStatusExtension
    //{
    //    public static string ToDescriptionString(this MailingStatus status)
    //    {
    //        FieldInfo field = status.GetType().GetField(status.ToString());
    //        DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
    //        return attribute == null ? status.ToString() : attribute.Description;
    //    }
    //}
}
