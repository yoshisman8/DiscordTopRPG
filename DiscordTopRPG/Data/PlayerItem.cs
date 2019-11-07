﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Data
{
    public class PlayerItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Character")]
        public Character CharacterId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}