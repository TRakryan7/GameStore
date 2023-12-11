using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.Server.Models;
public class Game
{
        
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public required string Name { get; set; } 
    
    [Required]
    [StringLength(20)]
    public required string Genre { get; set; }

    [Range(100000,1000000)]
    public double Price { get; set; }
    public DateTime RelaseDate { get; set; }
}
