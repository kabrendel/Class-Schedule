using System;
using SQLite;

namespace test
{
    public class Terms
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        [NotNull,MaxLength(50)]
        public string Title { get; set; }
        [NotNull]
        public DateTime Start { get; set; }
        [NotNull]
        public bool AlertsStart { get; set; }
        [NotNull]
        public DateTime End { get; set; }
        [NotNull]
        public bool AlertsEnd { get; set; }
    }
}