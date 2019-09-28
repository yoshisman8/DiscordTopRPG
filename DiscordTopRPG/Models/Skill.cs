using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public ApplicationUser Creator { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string[] Tags { get; set; }
        public List<SkillAction> Actions { get; set; } 

    }
}
