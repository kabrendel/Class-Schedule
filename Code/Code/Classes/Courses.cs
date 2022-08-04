using System;
using SQLite;

namespace test
{
    public enum StatusType { InProgress,Completed,Dropped,PlanToTake}
    
    public class Courses
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull, MaxLength(50)]
        public string Name { get; set; }
        [NotNull]
        public DateTime Start { get; set; }
        [NotNull]
        public bool AlertsStart { get; set; }
        [NotNull]
        public DateTime End { get; set; }
        [NotNull]
        public bool AlertsEnd { get; set; }
        [NotNull, MaxLength(50)]
        public StatusType Status { get; set; }
        [NotNull]
        public int InstructorID { get; set; }
        [NotNull, MaxLength(250)]
        public string Notes { get; set; }
        [NotNull]
        public int TermId { get; set; }
    }
}