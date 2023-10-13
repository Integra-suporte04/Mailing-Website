using Microsoft.AspNetCore.Mvc;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Identity;
using IntegraMailing.Data;

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
                // Suponhamos que você obtenha o nome do arquivo, data e status do arquivo

                string nomeArquivo = file.FileName;
                DateTime dataEnvio = DateTime.Now; // Data atual como exemplo
                string status = "Em Processamento";

                // Atualiza os valores nas colunas da linha com base no ID da linha
                // Suponhamos que você tenha um serviço ou lógica para fazer isso
                AtualizarLinha(linhaId, nomeArquivo, dataEnvio, status);


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
    }





}


