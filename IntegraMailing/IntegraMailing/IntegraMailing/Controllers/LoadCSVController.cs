using Microsoft.AspNetCore.Mvc;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Identity;
using IntegraMailing.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using IntegraMailing.Services;
using System.Text.Json;
using System.Linq;

namespace IntegraMailing.Controllers
{
    public class LoadCSVController : Controller
    {
        public static ListaViewModel listaViewModel = new ListaViewModel
        {

        };
        private readonly ILogger<LoadCSVController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        private readonly ICampaignService _campaingService;
     

        public LoadCSVController(ILogger<LoadCSVController> logger, UserManager<ApplicationUser> userManager, ApplicationUser currentUser, ApplicationDbContext context, IAccountService accountService, ICampaignService campaingService)
        {
            _logger = logger;
            _userManager = userManager;

            _context = context;
            _accountService = accountService;
            _campaingService = campaingService;
        }

        //Método para criar nova campanha e já salvar no banco de dados SQL
        [HttpPost]
        public async Task<IActionResult> NovaLinha()
        {
            var user = await _userManager.GetUserAsync(User);
            _accountService.GetUserInfo(user, this);

            listaViewModel.MaxPaginaCounter = ((listaViewModel.LinhaLista.Count - 1) / 6) + 1;


            Campanhas campanha = new Campanhas
            {
                Name = "No name",
                user_name = await _userManager.GetEmailAsync(user),
                Status = "Não iniciada",
                Data_Sent = DateTime.Now,
                Executed = false
            };

            await SaveCampanha(campanha);
            return RedirectToAction("GetCampanhas"); 

        }

        //Método que é acionado ao enviar o arquivo .csv que usuário selecionou
        [HttpPost]
        public async Task<IActionResult> AtualizaLinha(IFormFile file,int linhaId)
        {
            await _accountService.GetUserInfoAsync(this);

            int campanhaId = listaViewModel.LinhaLista[linhaId].Id;
            var campanha = await _context.Campanhas.FindAsync(campanhaId);

            if(campanha.Paused)
            {
                campanha.Paused = false;
                campanha.Status = "Em progresso...";
                await _context.SaveChangesAsync();
                return RedirectToAction("GetCampanhas");

            }
            else if (campanha.Executed)
            {
                // Retorna uma mensagem de erro ou algum tipo de resposta indicando que a campanha está em execução
                return BadRequest("A campanha está em execução ou já finalizou");
            }
            

            if (file != null && file.Length > 0)
            {
                // Lê e processa o arquivo CSV para obter os dados desejados

                List<tabela_mailing> numeros = new List<tabela_mailing>();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string line;
                    while((line = await reader.ReadLineAsync()) != null)
                    {
                        numeros.Add(new tabela_mailing { numero = line, campanha_Id = campanhaId});
                        
                    }
                }

                Campanhas campanhasInstance = new Campanhas
                {
                    Name = file.FileName,
                    Status = "Enviada"
                };

                await UpdateCampanha(campanhasInstance, listaViewModel.LinhaLista[linhaId].Id);
                await AddMailing(numeros);
                //await StartMailing(listaViewModel.LinhaLista[linhaId].Id);
                ExecuteMailingScript(listaViewModel.LinhaLista[linhaId].Id);

                campanha.Executed = true;
                campanha.Paused = false;
                campanha.Status = "Em Progresso...";
                await _context.SaveChangesAsync();

            }
                return RedirectToAction("GetCampanhas");
        }

        //Método para deletar as campanhas no banco de dados SQL
        [HttpPost]
        public async Task<IActionResult> DeletaLinha(int index)
        {
            await _accountService.GetUserInfoAsync(this);

            int campanhaId = listaViewModel.LinhaLista[index].Id;


            var campanha = await _context.Campanhas.FindAsync(campanhaId);

            if (campanha != null && campanha.Executed)
            {
                // Retorna uma mensagem de erro ou algum tipo de resposta indicando que a campanha está em execução
                return BadRequest("A campanha está em execução e não pode ser deletada neste momento.");
            }
            

            await DeleteCampanha(campanhaId);
            listaViewModel.LinhaLista.RemoveAt(index);
            return RedirectToAction("GetCampanhas");
        }

        [HttpPost]
        public async Task<IActionResult> PauseCampaign(int linhaId)
        {
            linhaId = listaViewModel.LinhaLista[linhaId].Id;

            var campanha = await _context.Campanhas.FindAsync(linhaId);

            if (campanha != null)
            {
                if(!campanha.Executed)
                    return BadRequest("Campanha não pode ser pausada, pois não está em andamento");


                campanha.Paused = !campanha.Paused;

                if (campanha.Paused)
                    campanha.Status = "Pausada";
                else
                    campanha.Status = "Em Progresso...";

                _context.Campanhas.Update(campanha);
            }

            else
                return NotFound();
            

            await _context.SaveChangesAsync();
            return RedirectToAction("GetCampanhas");
        }
        //Método para controle de paginas da View Lista (Cada página contém 6 registros de campanhas)
        [HttpPost]
        public async Task<IActionResult> TrocaPagina(int index)
        {
            await _accountService.GetUserInfoAsync(this);
            listaViewModel.PaginaCounter = index;
            return RedirectToAction("GetCampanhas");
        }

        /*
        [HttpPost]
        public async Task<IActionResult> StartMailing(int campanhaId)
        {

            var campanha = await _context.Campanhas.FindAsync(campanhaId);
            Debug.WriteLine(campanha.Status);

            if (campanha == null || campanha.InProgress)
            {
                return BadRequest("A campanha já está em execução ou o ID é inválido.");
            }

            campanha.InProgress = true;
            campanha.Paused = false;
            campanha.Status = "Enviado";
            await _context.SaveChangesAsync();

            return Ok();
        }
        */

        public void ExecuteMailingScript(int campanhaId)
        {
      
            _logger.LogInformation("Executando python com ID: " + campanhaId);



            var startInfo = new ProcessStartInfo
            {
                FileName = "python3.8", // ou "python3" dependendo do seu sistema
                Arguments = $"/home/validacao/Scripts/validar.py {campanhaId}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(startInfo);


        }

        //Cria uma nova campanha ao acionar o método NovaLinha e salva essa campanha
        [HttpPost]
        public async Task<IActionResult> SaveCampanha([FromBody] Campanhas data)
        {

            _context.Campanhas.Add(data);
            await _context.SaveChangesAsync();

            return Ok();

        }
        //Atualiza as colunas de Nome e Status da campanha com os novos dados recebidos
        [HttpPost]
        public async Task<IActionResult> UpdateCampanha([FromBody] Campanhas data, int id)
        {

            var campanha = await _context.Campanhas.FindAsync(id);
            if(campanha != null)
            {
                campanha.Name = data.Name;
                campanha.Status = data.Status;
                await _context.SaveChangesAsync();
            }

            return Ok();

        }
        public async Task<IActionResult> DeleteCampanha(int id)
        {
            var campanha = await _context.Campanhas.FindAsync(id);
            var numeros_finalizados = _context.mailing_finalizado.Where(m => m.campanha_id == id);
            var numeros_mailing = _context.tabela_mailing.Where(m => m.campanha_Id == id);
            if (campanha == null)
            {
                // Se não encontrar a campanha, retorna um 404 Not Found
                return NotFound();
            }

            _context.mailing_finalizado.RemoveRange(numeros_finalizados);
            _context.tabela_mailing.RemoveRange(numeros_mailing);
            // Remove a campanha do contexto
            await _context.SaveChangesAsync();
            _context.Campanhas.Remove(campanha);

            // Salva as mudanças no banco de dados
            await _context.SaveChangesAsync();

            // Retorna um 200 OK
            return Ok();
        }
        //Adiciona os registros recolhidos do arquivo .csv, na tabela mailing
        public async Task<IActionResult> AddMailing([FromBody] List<tabela_mailing> datas)
        {
            foreach(tabela_mailing data in datas)
            {
                _context.tabela_mailing.Add(data);
            }

            await _context.SaveChangesAsync();

            return Ok();

        }


        //Método para pegar as campanhas no SQL e transformar em formato de Lista dentro da Model ListaViewModel.
        //A Model é passada para a View como parametro, com isso os dados serão enviados para lá.
        //Sempre que este método é acionado, a página é recarregada
        public async Task<IActionResult> GetCampanhas()
        {
            Debug.WriteLine("Chamou o getcampanhas");
            var user = await _userManager.GetUserAsync(User);

            var userEmail = await _userManager.GetEmailAsync(user);
            listaViewModel.LinhaLista = await _context.Campanhas.Where(c => c.user_name == userEmail).ToListAsync();
            var campanhasParaFinalizar = await _context.Campanhas
    .Where(c => c.user_name == userEmail && c.Evolution == 100f && c.Status != "Finalizada")
    .ToListAsync();


            if (campanhasParaFinalizar.Any())
            {
                foreach (var campanha in campanhasParaFinalizar)
                {
                    campanha.Status = "Finalizada";
                }
                await _context.SaveChangesAsync();
            }

            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

            }

            return View("~/Views/Home/Lista.cshtml", listaViewModel);

        }
        [HttpPost]
        public IActionResult SetFileName(IFormFile file)
        {
 
            return Json(new {fileName = file.FileName});
        }

        [HttpGet]
        public async Task<IActionResult> CheckCampaignStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            var userEmail = await _userManager.GetEmailAsync(user);

            bool isCampaignRunning = await _context.Campanhas.AnyAsync(c => c.Status != "Finalizada" && c.Executed && !c.Paused && c.user_name == userEmail);
            return Json(new { IsRunning = isCampaignRunning });
        }


    }

}


