using System.Collections.Generic;
using App.Domain.Identity;

namespace App.DTOs.Manager
{
    public class RolePermission
    {
        public List<string> Keys { get; set; } = new List<string>();

        public Role Role { get; set; }

        public int RoleId { get; set; }
            
        public List<ActionDto> Actions { get; set; }
    }
}   