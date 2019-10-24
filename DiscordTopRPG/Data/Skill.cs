using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Data
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }
        public ApplicationUser Creator { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Tags { get; set; }
        public List<SkillAction> Actions { get; set; } 

    }
}
