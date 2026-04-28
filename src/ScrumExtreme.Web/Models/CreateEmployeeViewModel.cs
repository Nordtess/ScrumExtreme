using System.ComponentModel.DataAnnotations;

namespace ScrumExtreme.Web.Models;

    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "Förnamn är obligatoriskt.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Efternamn är obligatoriskt.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Användarnamn är obligatoriskt.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lösenord är obligatoriskt.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Behörighetsnivå är obligatoriskt.")]
        public string Role { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Ogiltig e-postadress.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adress är obligatoriskt.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stad är obligatoriskt.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postnummer är obligatoriskt.")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonnummer är obligatoriskt.")]
        [RegularExpression(@"^[0-9\+\-\s]+$", ErrorMessage = "Telefonnummer får bara innehålla siffror, +, - och mellanslag.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }

