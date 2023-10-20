using IntegraMailing.Controllers;
using IntegraMailing.Data;
using System.Collections.Concurrent;
using System.Text.Json;

namespace IntegraMailing.Services
{
    public class MailingService : BackgroundService, IMailingService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MailingService> _logger;


        public MailingService(IServiceProvider serviceProvider, ILogger<MailingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        } 


        public static readonly string _allMailingsPath = "/home/validacao/generalState.json";
        public static readonly string _allMailingsPathBackup = "/home/validacao/backup/generalState_backup.json";
        public static readonly string _filePath = "/home/validacao/script-execution-state-{0}.json";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MailingService is starting.");
            // Carregar o estado inicial
            //await LoadStateAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 18 && !AllCampanhas.AllCampanhasRunning.IsEmpty)
                {
                    var runningCampaignIds = AllCampanhas.AllCampanhasRunning.Keys.ToList();

                    foreach (var campanhaId in runningCampaignIds)
                    {
                        if (AllCampanhas.AllCampanhasRunning.TryGetValue(campanhaId, out var scriptExecutionState))
                        {
                            if (!scriptExecutionState.Executing)
                            {
                                scriptExecutionState.Executing = true;

                                // Atualizar o estado antes de executar o script
                                await SaveScriptExecutionStateAsync(scriptExecutionState);

                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var controller = scope.ServiceProvider.GetRequiredService<LoadCSVController>();
                                    controller.ExecuteScript(scriptExecutionState.CampanhaId);

                                    // Após a execução do script, atualizar o estado
                                    scriptExecutionState.ExecutionCount++;
                                    scriptExecutionState.Executing = true;
                                    await SaveScriptExecutionStateAsync(scriptExecutionState);
                                }
                            }
                        }
                    }
                }

                // Aguardar antes de verificar novamente
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        public async Task<bool> StartMailing(int campanhaId)
        {
            var scriptExecutionState = new ScriptExecutionState
            {
                CampanhaId = campanhaId,
                Executing = false,
                ExecutionCount = 0,

            };

            if (!AllCampanhas.AllCampanhasRunning.TryAdd(campanhaId, scriptExecutionState))
            {
                return false;
            }

            // Salvar o estado individual e o estado geral
            await SaveScriptExecutionStateAsync(scriptExecutionState);
            await SaveStateAsync();

            return true;
        }

        public async Task SaveStateAsync()
        {
            var json = JsonSerializer.Serialize(AllCampanhas.AllCampanhasRunning);
            await File.WriteAllTextAsync(_allMailingsPath, json);
            await File.WriteAllTextAsync(_allMailingsPathBackup, json);
        }
        private async Task LoadStateAsync()
        {
            AllCampanhas.AllCampanhasRunning.Clear();
            try
            {
                var json = await File.ReadAllTextAsync(_allMailingsPath);
                var state = JsonSerializer.Deserialize<ConcurrentDictionary<int, ScriptExecutionState>>(json);
                foreach (var item in state)
                {
                    AllCampanhas.AllCampanhasRunning.TryAdd(item.Key, item.Value);
                }
            }
            catch
            {
                // Se falhar ao ler o arquivo principal, tente o arquivo de backup
                var json = await File.ReadAllTextAsync(_allMailingsPathBackup);
                var state = JsonSerializer.Deserialize<ConcurrentDictionary<int, ScriptExecutionState>>(json);
                foreach (var item in state)
                {
                    AllCampanhas.AllCampanhasRunning.TryAdd(item.Key, item.Value);
                }
            }
        }


        private async Task<ScriptExecutionState> LoadScriptExecutionStateAsync(int campanhaId)
        {
            var filePath = string.Format(_filePath, campanhaId);
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<ScriptExecutionState>(json);
        }

        public async Task SaveScriptExecutionStateAsync(ScriptExecutionState scriptExecutionState)
        {
            var json = JsonSerializer.Serialize(scriptExecutionState);
            var filePath = string.Format(_filePath, scriptExecutionState.CampanhaId);
            await File.WriteAllTextAsync(filePath, json);
        }


    }

    public class ScriptExecutionState
    {
        
        public int CampanhaId { get; set; }
        public int ExecutionCount { get; set; } = 0;
        public bool Executing { get; set; }
    }
    public static class AllCampanhas
    {
        public static ConcurrentDictionary<int, ScriptExecutionState> AllCampanhasRunning { get; } = new ConcurrentDictionary<int, ScriptExecutionState>();

        // Método para adicionar um novo ScriptExecutionState
        public static bool AddScriptExecutionState(ScriptExecutionState scriptExecutionState)
        {
            return AllCampanhasRunning.TryAdd(scriptExecutionState.CampanhaId, scriptExecutionState);
        }

        // Método para remover um ScriptExecutionState
        public static bool RemoveScriptExecutionState(int campanhaId)
        {
            return AllCampanhasRunning.TryRemove(campanhaId, out _);
        }

        // ... outros métodos conforme necessário
    }

    public interface IMailingService
    {
        Task<bool> StartMailing(int campanhaId);
    }

}
