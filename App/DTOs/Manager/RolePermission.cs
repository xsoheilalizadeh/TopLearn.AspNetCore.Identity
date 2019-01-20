using System.Collections.Generic;
using App.Domain.Identity;

namespace App.DTOs.Manager
{
    public class RolePermission
    {
        public string[] Keys { get; set; }

        public Role Role { get; set; }

        public int RoleId { get; set; }
            
        public List<ActionDto> Actions { get; set; }
    }
}   