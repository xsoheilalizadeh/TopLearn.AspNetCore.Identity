using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.DTOs.Manager.User
{
    public class AddUserRole
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string SelectedRole { get; set; }    

        public List<SelectListItem> Roles { get; set; }
    }
}
    