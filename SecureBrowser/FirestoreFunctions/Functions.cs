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
            
            //var file = AppDomain.CurrentDomain.BaseDirectory + @"\FirestoreFunctions\Key.json";
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://raw.githubusercontent.com/SahilBhatiya/SecureBrowser-Dev/master/SecureBrowser/FirestoreFunctions/key.json?token=GHSAT0AAAAAABUFWMOEHNJIVRLUP4OYLC6UYTZFT5Q");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            
            var json = content;

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
                if (college.Password == firestoreCollege.Password)
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

                if (firestoreCollege.Password == Hashing.ComputeSha256Hash(college.Password))
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

            if (snapshot.Exists)
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



        internal static async Task<bool> SendCopyData(string clgEmail, string ExamId, string studentEmail, string Image)
        {
            Connect();
            StudentCopied student = new StudentCopied();

            DocumentReference DOC = database
                       .Collection("College")
                       .Document(clgEmail)
                       .Collection("Exams")
                       .Document(ExamId)
                       .Collection("Copied")
                       .Document(studentEmail)
                       .Collection("Images")
                       .Document(student.Id);

            student.Email = studentEmail;
            student.Image = Image;

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Email", student.Email },
                {"Image", student.Image },
                {"crrTime", student.crrTime },
            };

            await DOC.SetAsync(data);

            DocumentReference DOC1 = database
                       .Collection("College")
                       .Document(clgEmail)
                       .Collection("Exams")
                       .Document(ExamId)
                       .Collection("Copied")
                       .Document(studentEmail);

            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                {"Email", student.Email },
            };

            await DOC1.SetAsync(data1);


            return true;
        }







    }
}
