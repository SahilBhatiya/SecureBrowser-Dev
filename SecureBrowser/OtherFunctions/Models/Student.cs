using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBrowser.OtherFunctions.Models
{
    public class Student
    {
        public String Name { get; set; }
        public String Mobile { get; set; }
        public String Email { get; set; }
        public String EnrollNumber { get; set; }
        public String CollegeEmail { get; set; }
        public Int64 BatchYear { get; set; }
        public Int64 Semester { get; set; }
        public String Country { get; set; }
        public String State { get; set; }
        public String City { get; set; }
        public String Password { get; set; }
        public Boolean isAuth { get; set; }
        public Boolean isSuspended { get; set; }
        public String Role { get; set; }

    }
}
