﻿using System.ComponentModel.DataAnnotations;

namespace IntegraMailing.Data
{
    public class tabela_mailing
    {
        [Key]
        public int Id { get; set; }
        public string? numero { get; set; }
        public int campanha_Id { get; set; }

    }
}
