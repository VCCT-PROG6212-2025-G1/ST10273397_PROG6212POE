using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public enum Role
        {
            Lecturer,
            ProgCoord,
            AcadMan,
            HR
        }

        public Role UserRole { get; set; }

        public float HourlyRate { get; set; }

        public UserModel() { }

        public UserModel(int userId, string firstName, string lastName, string email, string password, Role role, float hourlyRate)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            UserRole = role;
            HourlyRate = hourlyRate;
        }
    }
}
