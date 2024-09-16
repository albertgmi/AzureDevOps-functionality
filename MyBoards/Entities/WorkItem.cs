using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBoards.Entities
{
    public class Epic : WorkItem
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class Issue : WorkItem
    {
        public decimal Efford { get; set; }
    }
    public class Task : WorkItem
    {
        public string Activity { get; set; }
        public decimal RemaningWork { get; set; }
    }
    public abstract class WorkItem
    {
        public int Id { get; set; }
        public virtual WorkItemState State { get; set; }
        public int StateId { get; set; }
        public string Area { get; set; }
        public string IterationPath { get; set; }
        public int Priority { get; set; }

        // Relacja 1 work item do wielu komentarzy
        public virtual IEnumerable<Comment> Comments { get; set; } = new List<Comment>();

        // Relacja 1 user do wielu workitemów
        public virtual User Author { get; set; }
        public Guid AuthorId { get; set; }
        // Relacja wiele tagów do wielu workitemów
        public virtual List<Tag> Tags { get; set; } = new List<Tag>();

        
    }
}
