using Microsoft.AspNetCore.Mvc;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Identity;
using IntegraMailing.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IntegraMailing.Controllers
{
    public class LoadCSVController : Controller
    {
        public static ListaViewModel listaViewModel = new ListaViewModel
        {

        };
        private readonly ILogger<LoadCSVController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationUser _currentUser;

        public LoadCSVController(ILogger<LoadCSVController> logger, UserManager<ApplicationUser> userManager, ApplicationUser currentUser)
        {
            _logger = logger;
            _userManager = userManager;
            _currentUser = currentUser;
        }
       

        //[HttpGet]
        //public IActionResult Lista()
        //{
        //    return View("~/Views/Home/Lista.cshtml", listaViewModel);
        //}

        [HttpPost]
        public async Task<IActionResult> AtualizaLinha(IFormFile file,int linhaId)
        {
            await GetUserInfo();

            if (file != null && file.Length > 0)
            {
                // Lê e processa o arquivo CSV para obter os dados desejados

                string nomeArquivo = file.FileName;
                DateTime dataEnvio = DateTime.Now; // Data atual como exemplo
                string status = "Em Processamento";

                // Atualiza os valores nas colunas da linha com base no ID da linha
                // Suponhamos que você tenha um serviço ou lógica para fazer isso
                AtualizarLinha(linhaId, nomeArquivo, dataEnvio, status);

                await RunMailingScript();
            }
            
            return View("~/Views/Home/Lista.cshtml",listaViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> NovaLinha()
        {
            await GetUserInfo();
            listaViewModel.LinhaLista.Add(new Linha("NoFile.Csv", DateTime.Now, "Sem arquivo"));
            ViewData["Title"] = listaViewModel.LinhaLista.Count;
            listaViewModel.MaxPaginaCounter = ((listaViewModel.LinhaLista.Count-1) / 6) +1;
            return View("~/Views/Home/Lista.cshtml", listaViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> TrocaPagina(int index)
        {
            await GetUserInfo();
            listaViewModel.PaginaCounter = index;
            return View("~/Views/Home/Lista.cshtml", listaViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeletaLinha(int index)
        {
            await GetUserInfo();
            listaViewModel.LinhaLista.RemoveAt(index);
            return View("~/Views/Home/Lista.cshtml", listaViewModel);

        }

        public void AtualizarLinha(int linhaId, string novoNome, DateTime novaData, string novoStatus)
        {
            listaViewModel.LinhaLista[linhaId].Nome = novoNome;
            listaViewModel.LinhaLista[linhaId].Data = novaData;
            listaViewModel.LinhaLista[linhaId].Status = novoStatus;

        }
        public async Task<IActionResult> RecarregarPagina()
        {
            await GetUserInfo();
            // Redireciona para a própria ação atual, que recarregará a página
            return RedirectToAction("Listas");
        }
        private async Task GetUserInfo()
        {
            _currentUser = await _userManager.GetUserAsync(User);
            if (_currentUser != null)
            {
                ViewData["AccountType"] = _currentUser.AccountType;
                ViewData["UserEmail"] = _currentUser.Email;
                ViewData["UserName"] = _currentUser.UserName;

            }
        }

        [HttpGet]
        public async Task<IActionResult> RunMailingScript()
        {
            try
            {
                // Define o caminho do script Python
                var pythonScriptPath = "/home/valida_script/validaV2.py";

                // Cria uma nova instância da classe ProcessStartInfo
                var startInfo = new ProcessStartInfo
                {
                    FileName = "python3.8", // ou "python3" dependendo do seu sistema
                    Arguments = pythonScriptPath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                // Cria e executa o processo
                using var process = Process.Start(startInfo);
                if (process != null)
                {
                    // Lê a saída padrão do processo (que deve ser o valor retornado pelo script Python)
                    string result = await process.StandardOutput.ReadToEndAsync();

                    // Aguarda a conclusão do processo
                    process.WaitForExit();

                    // Converte a saída para int e retorna
                    if (int.TryParse(result, out int returnValue))
                    {
                        Debug.WriteLine("WORKED I THINK");
                        _logger.Log(LogLevel.Debug,"Code worked and returned an integer value");
                        return Ok(returnValue);
                    }
                    else
                    {
                        Debug.WriteLine("Not int returned");
                        _logger.Log(LogLevel.Debug, "Code worked, but no integer value were returned");

                        return BadRequest("A saída do script Python não é um número inteiro válido.");
                    }
                }
                else
                        _logger.Log(LogLevel.Debug,"Code didn't work for some unexpected reason");
                {
                    return StatusCode(500, "Não foi possível iniciar o processo Python.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Didn't even work bro...");

                return StatusCode(500, $"Erro: {ex.Message}");
            }
        }
    }





}


