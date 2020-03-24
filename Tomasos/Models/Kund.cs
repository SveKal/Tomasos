using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Tomasos.Models
{
    public partial class Kund
    {
        public Kund()
        {
            Bestallning = new HashSet<Bestallning>();
        }

        public int KundId { get; set; }

        [Required(ErrorMessage = "Enter your name")]
        [StringLength(100, ErrorMessage = "{0} Can not have more than {1} characters")]
        public string Namn { get; set; }

        [Required(ErrorMessage = "Enter your address")]
        [StringLength(50, ErrorMessage = "{0} Can not have more than {1} characters")]
        public string Gatuadress { get; set; }

        [Required(ErrorMessage = "Enter your zip code")]
        [StringLength(20, ErrorMessage = "{0} Can not have more than {1} characters")]
        public string Postnr { get; set; }

        [Required(ErrorMessage = "Enter your city")]
        [StringLength(100, ErrorMessage = "{0} Can not have more than {1} characters")]
        public string Postort { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Enter a valid e-mail address")]
        [StringLength(50, ErrorMessage = "{0} Can not have more than {1} characters")]
        [Remote("VerifyEmail", "Account")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone]
        [StringLength(50, ErrorMessage = "{0} Can not have more than {1} characters")]
        public string Telefon { get; set; }

        [Required(ErrorMessage = "Enter your user name")]
        [DisplayName("Username")]
        [StringLength(20, ErrorMessage = "{0} Can not have more than {1} characters")]
        public string AnvandarNamn { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [StringLength(20, ErrorMessage = "{0} Can not have more than {1} characters", MinimumLength = 5)]
        public string Losenord { get; set; }

        public virtual ICollection<Bestallning> Bestallning { get; set; }
    }
}
