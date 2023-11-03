// Localização do arquivo: /Services/CampaignService.cs

using IntegraMailing.Data;
using IntegraMailing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntegraMailing.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(ApplicationDbContext context, ILogger<CampaignService> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Cria uma nova campanha ao acionar o método NovaLinha e salva essa campanha
        public async Task SaveCampaign(Campanhas campaign)
        {
            _context.Campanhas.Add(campaign);
            await _context.SaveChangesAsync();
        }
        //Atualiza as colunas de Nome e Status da campanha com os novos dados recebidos
        public async Task UpdateCampaign(Campanhas campaign, int id)
        {
            var existingCampaign = await _context.Campanhas.FindAsync(id);
            if (existingCampaign != null)
            {
                existingCampaign.Name = campaign.Name;
                existingCampaign.Status = campaign.Status;
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteCampaign(int id)
        {
            var campaign = await _context.Campanhas.FindAsync(id);
            var finishedNumbers = _context.mailing_finalizado.Where(m => m.campanha_id == id);
            var mailingNumbers = _context.tabela_mailing.Where(m => m.campanha_Id == id);
            if (campaign == null)
            {
                throw new InvalidOperationException("Campanha não encontrada.");
            }

            _context.mailing_finalizado.RemoveRange(finishedNumbers);
            _context.tabela_mailing.RemoveRange(mailingNumbers);
            _context.Campanhas.Remove(campaign);
            await _context.SaveChangesAsync();
        }
        //Adiciona os registros recolhidos do arquivo .csv, na tabela mailing
        public async Task AddMailing(List<tabela_mailing> datas)
        {
            foreach (tabela_mailing data in datas)
            {
                _context.tabela_mailing.Add(data);
            }

            await _context.SaveChangesAsync();
        
        }

        public async Task StartMailing(int campaignId)
        {
            var campaign = await _context.Campanhas.FindAsync(campaignId);
            if (campaign == null || campaign.Executed)
            {
                throw new InvalidOperationException("A campanha já está em execução ou o ID é inválido.");
            }

            campaign.Executed = true;
            await _context.SaveChangesAsync();
        }

        public void ExecuteScript(int campaignId)
        {
            _logger.LogInformation("Executando python com ID: " + campaignId);
            var startInfo = new ProcessStartInfo
            {
                FileName = "python3.8",
                Arguments = $"/home/validacao/Scripts/validarTeste.py {campaignId}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(startInfo);
        }
    }

    public interface ICampaignService
    {
        Task SaveCampaign(Campanhas campaign);
        Task UpdateCampaign(Campanhas campaign, int id);
        Task DeleteCampaign(int id);
        Task AddMailing(List<tabela_mailing> datas);
        Task StartMailing(int campaignId);
        void ExecuteScript(int campaignId);
    }
}
