using IntegraMailing.Data;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace IntegraMailing.Controllers
{
    public class ResultadoCampanhaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAntiforgery _antiforgery;
        public static  MailingResults mailingResults;
        public static CampanhasResults campanhasResults;
        public ResultadoCampanhaController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IAntiforgery antiforgery)
        {
            _userManager = userManager;
            _context = context;
            _antiforgery = antiforgery;
        }
        public async Task<IActionResult> GetCampanhas()
        {
            var user = await _userManager.GetUserAsync(User);
            var userEmail = await _userManager.GetEmailAsync(user);  // obter o email do usuário como você já fez
            var query = _context.Campanhas.Where(c => c.user_name == userEmail);
            var campanhas = query.ToListAsync();

            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

            }
            int maxPaginaCounter = campanhas.Result.Count % 6 == 0 ? campanhas.Result.Count / 6 : (campanhas.Result.Count / 6) + 1;

            campanhasResults = new CampanhasResults { MaxPaginaCounter = maxPaginaCounter, Campanhas = campanhas.Result, PaginaCounter = 1 };
            return View("~/Views/Home/Resultados.cshtml", campanhasResults);
        }
        [HttpPost]
        public async Task<IActionResult> GetCampanhasFilter(string searchName, DateTime? startDate, DateTime? endDate, string statusFilter)
        {
            var user = await _userManager.GetUserAsync(User);
            var userEmail = await _userManager.GetEmailAsync(user);  // obter o email do usuário como você já fez
            var query = _context.Campanhas.Where(c => c.user_name == userEmail);

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(c => c.Name.ToLower().Contains(searchName.ToLower()));
            }
            if(startDate.HasValue)
            {
                query = query.Where(c => c.Data_Sent.Value.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {              
                query = query.Where(c => c.Data_Sent.Value.Date <= endDate.Value.Date);
            }

            // Filtro por status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(c => c.Status.ToLower() == statusFilter.ToLower());
            }


            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

            }
            var campanhas = await query.ToListAsync();
            int maxPaginaCounter = campanhas.Count % 6 == 0 ? campanhas.Count / 6 : (campanhas.Count / 6) + 1;
            campanhasResults = new CampanhasResults { MaxPaginaCounter = maxPaginaCounter, Campanhas = campanhas, PaginaCounter = 1 };
            return View("~/Views/Home/Resultados.cshtml", campanhasResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetMailings(int campanhaId)
        {
            var user = await _userManager.GetUserAsync(User);
            var userEmail = await _userManager.GetEmailAsync(user);

            var campanha = await _context.Campanhas.FindAsync(campanhaId);

            if (campanha == null || campanha.user_name != userEmail)
                return Forbid();


            var mailings = await _context.mailing_finalizado.Where(m => m.campanha_id == campanhaId).ToListAsync();
            //int paginaCounter = mailings.Count; 
            int maxPaginaCounter = mailings.Count % 10 == 0 ? mailings.Count / 10 : (mailings.Count / 10) + 1;

            mailingResults = new MailingResults { MaxPaginaCounter = maxPaginaCounter, MailingsFinalizados = mailings, PaginaCounter = 1 };
            
            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

            }

            return View("~/Views/Home/ResultadosMailing.cshtml", mailingResults);
        }

        [HttpPost]
        public async Task<IActionResult> TrocaPagina(int index)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

            }

            if (mailingResults.MailingsFinalizados != null)
                mailingResults.PaginaCounter = index;

            return View("~/Views/Home/ResultadosMailing.cshtml", mailingResults);
        }
        [HttpPost]
        public async Task<IActionResult> TrocaPaginaCampanhas(int index)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

            }

            if (campanhasResults.Campanhas != null)
                campanhasResults.PaginaCounter = index;

            return View("~/Views/Home/Resultados.cshtml", campanhasResults);
        }

        public IActionResult ExportarCsv(int campanhaId)
        {

            var mailings = _context.mailing_finalizado.Where(m => m.campanha_id == campanhaId).ToList();
            var csvString = ConvertToCsv(mailings);
            var bytes = Encoding.UTF8.GetBytes(csvString);

            var fileName = _context.Campanhas.Find(campanhaId).Name;
            fileName = fileName.Remove(fileName.IndexOf(".csv"));

            return File(bytes, "text/csv", fileName + "_Resultado.csv");
        }

        private string ConvertToCsv(List<MailingFinalizado> mailings)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()));
            stringBuilder.AppendLine("Número;Status 1;Hora da Tentativa 1;Status 2;Hora da Tentativa 2;Status 3;Hora da Tentativa 3;Status Final");  // header line

            foreach (var mailing in mailings)
            {
                var horaTentativa1String = mailing.hora_tentativa_1?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                var horaTentativa2String = mailing.hora_tentativa_2?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                var horaTentativa3String = mailing.hora_tentativa_3?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";

                stringBuilder.AppendLine($"{mailing.numero};{mailing.status_1};{horaTentativa1String};{mailing.status_2};{horaTentativa2String};{mailing.status_3};{horaTentativa3String};{mailing.statusFinal}");
            }

            return stringBuilder.ToString();
        }
        [HttpGet]
        public IActionResult GetAntiForgeryToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return Ok(tokens.RequestToken);
        }

    }
    public struct MailingResults
    {
        public int PaginaCounter { get; set; }
        public int MaxPaginaCounter { get; set; }
        public List<MailingFinalizado> MailingsFinalizados { get; set; }
    }
    public struct CampanhasResults
    {
        public int PaginaCounter { get; set; }
        public int MaxPaginaCounter { get; set; }
        public List<Campanhas> Campanhas { get; set; }

    }
}
