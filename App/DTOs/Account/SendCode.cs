using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.DTOs.Account
{
    public class SendCode
    {
        public string SelectedProvider { get; set; }

        public string ReturnTo { get; set; }

        public bool RememberMe { get; set; }    

        public List<SelectListItem> Providers { get; set; }
    }
}       