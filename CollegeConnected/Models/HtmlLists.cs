using System.Collections.Generic;

namespace CollegeConnected.Models
{
    public class HtmlLists
    {
        public static IEnumerable<ConstituentType> Types = new List<ConstituentType>
    {
    new ConstituentType
    {
        TypeID = 1,
        Name = "Student"
    },
    new ConstituentType
    {
        TypeID = 2,
        Name = "Spouse"
    },
        new ConstituentType
    {
        TypeID = 3,
        Name = "Patron"
    },
    };
    }
}