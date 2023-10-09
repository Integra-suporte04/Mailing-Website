using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Web;
using IntegraMailing.Models;
namespace IntegraMailing.Controllers
{
    public class LoadCSVController : Controller
    {
        public static ListaViewModel listaViewModel = new ListaViewModel
        {

        };
        private readonly ILogger<LoadCSVController> _logger;

        public LoadCSVController(ILogger<LoadCSVController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Lista()
        {
            return View("~/Views/Home/Lista.cshtml", listaViewModel);
        }

        [HttpPost]
        public IActionResult AtualizaLinha(IFormFile file,int linhaId)
        {
            
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
        public IActionResult NovaLinha()
        {
            listaViewModel.LinhaLista.Add(new Linha("NoFile.Csv", DateTime.Now, "Sem arquivo"));
            ViewData["Title"] = listaViewModel.LinhaLista.Count;
            listaViewModel.MaxPaginaCounter = ((listaViewModel.LinhaLista.Count-1) / 6) +1;
            return View("~/Views/Home/Lista.cshtml", listaViewModel);
        }
        [HttpPost]
        public IActionResult TrocaPagina(int index)
        {
            listaViewModel.PaginaCounter = index;
            return View("~/Views/Home/Lista.cshtml", listaViewModel);
        }
        [HttpPost]
        public IActionResult DeletaLinha(int index)
        {
            listaViewModel.LinhaLista.RemoveAt(index);
            return View("~/Views/Home/Lista.cshtml", listaViewModel);

        }

        public void AtualizarLinha(int linhaId, string novoNome, DateTime novaData, string novoStatus)
        {
            listaViewModel.LinhaLista[linhaId].Nome = novoNome;
            listaViewModel.LinhaLista[linhaId].Data = novaData;
            listaViewModel.LinhaLista[linhaId].Status = novoStatus;

        }
        public IActionResult RecarregarPagina()
        {
            // Redireciona para a própria ação atual, que recarregará a página
            return RedirectToAction("Listas");
        }
    }





}


