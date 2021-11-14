using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBrowser.OtherFunctions.Models
{
    [FirestoreData]
    public class FirestoreExam
    {
        [FirestoreProperty]
        public String Id { get; set; }
        [FirestoreProperty]
        public String CollegeEmail { get; set; }


        [FirestoreProperty]
        public dynamic Start { get; set; }
        [FirestoreProperty]
        public dynamic End { get; set; }


        [FirestoreProperty]
        public String Name { get; set; }
        [FirestoreProperty]
        public String Link { get; set; }


        [FirestoreProperty]
        public String Semester { get; set; }
    }
}
