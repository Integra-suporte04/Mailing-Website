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

            if (user != null)
            {
                ViewData["AccountType"] = user.AccountType;
                ViewData["UserEmail"] = user.Email;
                ViewData["UserName"] = user.UserName;

            }

            return View("~/Views/Home/Resultados.cshtml", await query.ToListAsync());
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
                query = query.Where(c => c.Data_Sent >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(c => c.Data_Sent <= endDate.Value);
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
            var result = await query.ToListAsync();
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetMailings(int campanhaId)
        {
            var userEmail = User.Identity.Name; // Asumindo que o email do usuário é o identificador

            var campanha = await _context.Campanhas.FindAsync(campanhaId);

            if (campanha == null || campanha.user_name != userEmail)
                return Forbid(); // ou return NotFound(); dependendo de como você quer lidar com isso


            var mailings = await _context.mailing_finalizado.Where(m => m.campanha_id == campanhaId).ToListAsync();


            return View("~/Views/Home/ResultadosMailing.cshtml", mailings);
        }

        public IActionResult ExportarCsv(int campanhaId)
        {

            var mailings = _context.mailing_finalizado.Where(m => m.campanha_id == campanhaId).ToList();
            var csvString = ConvertToCsv(mailings);
            var bytes = Encoding.UTF8.GetBytes(csvString);

            var fileName = _context.Campanhas.Find(campanhaId).Name;
            fileName = fileName.Remove(fileName.IndexOf(".csv") - 4);

            return File(bytes, "text/csv", fileName + "_Resultado.csv");
        }

        private string ConvertToCsv(List<MailingFinalizado> mailings)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble()));
            stringBuilder.AppendLine("Número;Status;Hora da Tentativa 1;Hora da Tentativa 2;Hora da Tentativa 3");  // header line

            foreach (var mailing in mailings)
            {
                var horaTentativa1String = mailing.hora_tentativa_1?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                var horaTentativa2String = mailing.hora_tentativa_2?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                var horaTentativa3String = mailing.hora_tentativa_3?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";

                stringBuilder.AppendLine($"{mailing.numero};{mailing.status};{horaTentativa1String};{horaTentativa2String};{horaTentativa3String}");
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
}
