using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        [ForeignKey("AspNetUserRefId")]
        public IdentityUser OwnerId { get; set; }
        /// <summary>
        /// <para>The actor's statblock.</para>
        /// <para>Sorted in Strength, Dexterity, Agility, Endurance, Will </para>
        /// </summary>
        //public int[,] Statblock { get; set; } = new int[5, 3]
        //{
        //    {100,0,0 },
        //    {100,0,0 },
        //    {100,0,0 },
        //    {100,0,0 },
        //    {100,0,0 },
        //};
        //public Dictionary<Conditions,int> Conditions { get; set; }
    }
}
