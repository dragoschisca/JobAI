using System.ComponentModel.DataAnnotations.Schema;

namespace JobAPI.Domain.Entites;

public class Candidat : User
{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CvPath { get; set; }
        public string AboutMe { get; set; }
}