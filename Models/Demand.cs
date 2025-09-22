// using System;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// namespace FintcsApi.Models
// {
//     [Table("Demands")]
//     public class Demand
//     {
//         [Key]
//         public int Id { get; set; }

//         [Required]
//         public int Month { get; set; }

//         [Required]
//         public int Year { get; set; }

//         // JSON snapshot of demand details
//         [Required]
//         public string Data { get; set; } = string.Empty;

//         public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//     }
// }
