﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Models
{
    public class SkillAction
    {
        [Key]
        public int Id { get; set; }
        public ApplicationUser Creator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ActionEconomy ActionEconomy { get; set; }
        public string Tags { get; set; }
    }
}
