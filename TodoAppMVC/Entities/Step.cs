using Microsoft.AspNetCore.Authorization;

namespace TodoAppMVC.Entities
{
    public class Step
    {
        public Guid Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int Order { get; set; }
    }
}
