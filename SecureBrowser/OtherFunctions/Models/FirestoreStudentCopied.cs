using Google.Cloud.Firestore;
using System;

namespace SecureBrowser.OtherFunctions.Models
{
    [FirestoreData]
    public class FirestoreStudentCopied
    {
        [FirestoreProperty]
        public string crrTime { get; set; }

        [FirestoreProperty]
        public string Image { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Id { get; set; }

    }
}
