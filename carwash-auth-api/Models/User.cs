using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace carwash_auth_api.Models;

[Table("User")]
public class User : BaseEntity
{
    [Column("userid")]
    public override Guid Id { get; set; }
    
    [ForeignKey("SubjectInfo")]
    [Column("subjectid")]
    public Guid SubjectId { get; set; } 
    
    public Subject SubjectInfo { get; set; }
    
    [Column("ispartner")]
    public bool IsPartner { get; set; }
    
    [Required]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; }
    
    [Required]
    [Column("hashedpassword")]
    public string PasswordHash { get; set; }
    
    [Column("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [Column("refresh_token_expiry")]
    public DateTime? RefreshTokenExpiry { get; set; }
}