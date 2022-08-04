using System;
using SQLite;

namespace test
{
    public enum AsType { Objective,Performance}

    public class Assessments
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        public AsType Type { get; set; }
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
        [NotNull]
        public int CourseID { get; set; }
    }
}
