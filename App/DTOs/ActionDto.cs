namespace App.DTOs
{
    public class ActionDto
    {
        public string Key => $"{AreaName}:{ControllerName}:{ActionName}";

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string ActionDisplayName { get; set; }   

        public string AreaName { get; set; }
    }
}