using Microsoft.EntityFrameworkCore;

namespace TodoAppMVC.Entities
{
    public class AttachedFile
    {
        public Guid Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }
        [Unicode]
        public string Url { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
