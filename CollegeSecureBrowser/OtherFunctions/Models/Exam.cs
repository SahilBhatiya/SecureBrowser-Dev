using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollegeSecureBrowser.OtherFunctions.Models
{

    public class Exam
    {
        public String Id { get; set; }
        public String CollegeEmail { get; set; }


        public DateTime Start { get; set; }
        public DateTime End { get; set; }


        public String Name { get; set; }
        public String Link { get; set; }


        public String Semester { get; set; }
    }
}
