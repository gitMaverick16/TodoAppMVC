using System.ComponentModel.DataAnnotations;

namespace TodoAppMVC.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public DateTime DateCreated { get; set; }
        public List<Step> Steps { get; set; }
        public List<AttachedFile> AttachedFiles { get; set; }
    }
}
