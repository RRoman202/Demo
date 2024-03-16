using System.ComponentModel.DataAnnotations;

namespace Demo.Data.Models
{
    public class Application
    {
        public int Id { get; set; }
        public DateOnly DateAddition { get; set; }
        public string? NameEquipment { get; set; }
        public long EquipmentTypeId { get; set; }
        public long ProblemTypeId { get; set; }

        public string? Comment { get; set; }


        public long StatusId { get; set; }
        public string? ClientName { get; set; }
        public int Cost { get; set; }
        public DateOnly DateEnd { get; set; }
        public TimeOnly TimeWork { get; set; }
        public long UserId { get; set; }
        public DateOnly PeriodExecution { get; set; }
        public int Number {  get; set; }
        public string? Description { get; set; }
        public string? WorkStatus {  get; set; }

        public EquipmentType? Equipment { get; set; }
        public ProblemType? Problem { get; set; }
        public Status? status { get; set; }
        public User? User { get; set; }

    }
}
