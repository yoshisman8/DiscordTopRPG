﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Data
{
    public class Character
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Bio { get; set; }

        public ApplicationUser Owner { get; set; }

        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Agility { get; set; }
        public int Constitution { get; set; }
        public int Memory { get; set; }
        public int Intuition { get; set; }
        public int Charisma { get; set; }

        public List<Skill> Skill {get;set;}
        public List<PlayerItem> Inventory { get; set; }
    }
}
