using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carwash_auth_api.Models;

[Table("subject")]
public class Subject
{
   [Key]
   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
   [Column("subjectid")]
   public Guid Id { get; set; }
   
   [Required]
   [Column("firstname")]
   public string FirstName { get; set; }
   
   [Required]
   [Column("lastname")] 
   public string LastName { get; set; }
   
   [Required]
   [Column("phone")]
   public string PhoneNumber { get; set; }
}