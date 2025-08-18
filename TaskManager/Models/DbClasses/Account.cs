using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.DbClasses
{
    public class Account
    {
        [Key]
        public int? AccountId { get; set; }
        public int? LeaderId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string? Email { get; set; }
    }
}
