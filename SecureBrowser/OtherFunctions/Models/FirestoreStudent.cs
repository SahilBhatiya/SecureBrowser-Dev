using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBrowser.OtherFunctions.Models
{
    [FirestoreData]
    public class FirestoreStudent
    {
        [FirestoreProperty]
        public String Name { get; set; }

        [FirestoreProperty]
        public String Mobile { get; set; }

        [FirestoreProperty]
        public String Email { get; set; }

        [FirestoreProperty]
        public String EnrollNumber { get; set; }

        [FirestoreProperty]
        public String CollegeEmail { get; set; }

        [FirestoreProperty]
        public Int64 BatchYear { get; set; }

        [FirestoreProperty]
        public Int64 Semester { get; set; }

        [FirestoreProperty]
        public String Country { get; set; }
        [FirestoreProperty]
        public String State { get; set; }

        [FirestoreProperty]
        public String City { get; set; }

        [FirestoreProperty]
        public String Password { get; set; }

        [FirestoreProperty]
        public Boolean isAuth { get; set; }

        [FirestoreProperty]
        public Boolean isSuspended { get; set; }

        [FirestoreProperty]
        public String Role { get; set; }

    }
}
