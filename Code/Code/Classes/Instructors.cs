using System;
using SQLite;

namespace test
{
    public class Instructors
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull,MaxLength(50)]
        public string Name { get; set; }
        [NotNull, MaxLength(50)]
        public string Email { get; set; }
        [NotNull, MaxLength(10)]
        public string Phone { get; set; }
    }
}