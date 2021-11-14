using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SecureBrowser.OtherFunctions.Functions;
using SecureBrowser.OtherFunctions.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using SecureBrowser.OtherFunctions.Models;
using static Google.Cloud.Firestore.V1.Firestore;

namespace SecureBrowser.FirestoreFunctions
{
    public class Functions
    {
        private static FirestoreDb database;

        public Functions()
        {
            Connect();
        }


        public static string Connect()
        {
            //var file = AppDomain.CurrentDomain.BaseDirectory + @"\FirestoreFunctions\Key.json";
            var file = AppDomain.CurrentDomain.BaseDirectory + @"\FirestoreFunctions\Key2.json";
            var json = File.ReadAllText(file, Encoding.UTF8);

            var jsonString = json.ToString();
            try
            {
                var builder = new FirestoreClientBuilder { JsonCredentials = jsonString };
                //database = FirestoreDb.Create("exam-proctor-8d533", builder.Build());
                database = FirestoreDb.Create("exam-proctor-project", builder.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n\n\n\n\nError \n" + e.ToString() + "\n\n" + e.InnerException + "\n\n\n\n\n");
            }

            return "Success";
        }

        internal static async Task<bool> DeleteCollege(string Email, string password)
        {
            Boolean isCollegeDeleted = false;

            Connect();
            DocumentReference DOC = database
                                   .Collection("College")
                                   .Document(Email);

            College clg = new College()
            {
                Email = Email,
                Password = Hashing.ComputeSha256Hash(password),
            };

            bool isValid = await VerifyCollege(clg);

            if (isValid)
            {
                var result = await DOC.DeleteAsync();
                if (result != null)
                {
                    isCollegeDeleted = true;
                }
                else
                {
                    isCollegeDeleted = false;
                }
            }
            else
            {
                isCollegeDeleted = false;
            }


            return isCollegeDeleted;
        }


        internal static async Task<bool> VerifyStudent(string email, string studentEmail, string password)
        {
            Connect();
            DocumentReference DOC = database
                       .Collection("College")
                       .Document(email)
                       .Collection("Students")
                       .Document(studentEmail);
            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                FirestoreStudent firestoreCollege = snapshot.ConvertTo<FirestoreStudent>();
                if (password == firestoreCollege.Password)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        internal static async Task<FirestoreStudent> GetStudent(string email, string studentEmail)
        {
            Connect();
            bool isExsits = false;

            DocumentReference DOC = database
                                   .Collection("College")
                                   .Document(email)
                                   .Collection("Students")
                                   .Document(studentEmail);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                FirestoreStudent firestoreCollege = snapshot.ConvertTo<FirestoreStudent>();

                return firestoreCollege;
            }
            else
            {
                return null;
            }
        }

        public static async Task<bool> VerifyCollege(College college)
        {
            Connect();
            DocumentReference DOC = database
                                   .Collection("College")
                                   .Document(college.Email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                FirestoreCollege firestoreCollege = snapshot.ConvertTo<FirestoreCollege>();
                if(college.Password == firestoreCollege.Password)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            else
            {
                return false;
            }

        }

        public static async Task<bool> UpdateCollege(College college)
        {
            Connect();
            DocumentReference DOC = database
                       .Collection("College")
                       .Document(college.Email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    {"Country", college.Country },
                    {"State", college.State },
                    {"City", college.City },
                    {"Pincode", college.Pincode },
                    {"Mobile", college.Mobile },
                    {"Name", college.Name },
                    {"DefaultLink", college.DefaultLink }
                };

                await DOC.UpdateAsync(data);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> UpdatePassword(College college)
        {
            Connect();
            DocumentReference DOC = database
                       .Collection("College")
                       .Document(college.Email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                FirestoreCollege firestoreCollege = snapshot.ConvertTo<FirestoreCollege>();

                if(firestoreCollege.Password == Hashing.ComputeSha256Hash(college.Password))
                {
                    Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        {"Password", Hashing.ComputeSha256Hash(college.NewPassword) }
                    };

                    await DOC.UpdateAsync(data);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static string CreateCollege(College college)
        {
            Connect();
            var task = CollegeExsits(college.Email);
            task.Wait();
            bool isValid = task.Result;

            if (!isValid)
            {
                DocumentReference DOC = database
    .Collection("College")
    .Document(college.Email);
                Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Country", college.Country },
                {"State", college.State },
                {"City", college.City },
                {"Pincode", college.Pincode },
                {"Mobile", college.Mobile },
                {"Email", college.Email },
                {"Name", college.Name },
                {"Password", college.Password },
                {"DefaultLink", college.DefaultLink },
                {"Role", "College" },
            };
                DOC.SetAsync(data);

                return "College Created";
            }
            else
            {
                return "College Already Exsits";
            }

        }

        private static async Task<bool> CollegeExsits(string email)
        {
            Connect();
            bool isExsits = false;

            DocumentReference DOC = database
                                   .Collection("College")
                                   .Document(email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if(snapshot.Exists)
            {
                isExsits = true;
            }

            return isExsits;
        }

        public static async Task<FirestoreCollege> GetCollege(string email)
        {
            Connect();
            bool isExsits = false;

            DocumentReference DOC = database
                                   .Collection("College")
                                   .Document(email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                FirestoreCollege firestoreCollege = snapshot.ConvertTo<FirestoreCollege>();

                return firestoreCollege;
            }
            else
            {
                return null;
            }
        }

        public static async Task<List<FirestoreCollege>> GetAllCollege()
        {
            Connect();
            List<FirestoreCollege> models = new List<FirestoreCollege>();

            Query allData = database
                                   .Collection("College");

            QuerySnapshot allDataSnapshot = await allData.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allDataSnapshot.Documents)
            {

                FirestoreCollege model = documentSnapshot.ConvertTo<FirestoreCollege>();
                models.Add(model);
            }
            Task.WaitAll();
            return models;
        }

        public static async Task<List<FirestoreExam>> GetAllExams(string email)
        {
            Connect();
            List<FirestoreExam> lists = new List<FirestoreExam>();

            Query allData = database
                                   .Collection("College")
                                   .Document(email)
                                   .Collection("Exams");

            QuerySnapshot allDataSnapshot = await allData.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allDataSnapshot.Documents)
            {

                FirestoreExam model = documentSnapshot.ConvertTo<FirestoreExam>();
                lists.Add(model);
            }
            return lists;
        }


        public static async Task<List<FirestoreExam>> GetAllExamsInSem(string email, long sem)
        {
            Connect();
            List<FirestoreExam> lists = new List<FirestoreExam>();

            Query allData = database
                                   .Collection("College")
                                   .Document(email)
                                   .Collection("Exams");

            QuerySnapshot allDataSnapshot = await allData.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allDataSnapshot.Documents)
            {

                FirestoreExam model = documentSnapshot.ConvertTo<FirestoreExam>();
                lists.Add(model);
            }
            return lists.Where(x => x.Semester == sem.ToString()).ToList();
        }




    }
}
