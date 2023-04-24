using ProiectPractica.Data.Entities;

namespace ProiectPractica.DTOs
{
    public class RegisterDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }

    }
}
