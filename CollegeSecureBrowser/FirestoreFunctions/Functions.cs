using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CollegeSecureBrowser.OtherFunctions.Functions;
using CollegeSecureBrowser.OtherFunctions.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using static Google.Cloud.Firestore.V1.Firestore;

namespace CollegeSecureBrowser.FirestoreFunctions
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
            catch(Exception e)
            {
                Console.WriteLine("\n\n\n\n\n\nError \n"+ e.ToString() + "\n\n"+ e.InnerException +"\n\n\n\n\n");
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







        private static async Task<bool> StudentExsits(string email, String StudentEmail)
        {
            Connect();
            bool isExsits = false;

            DocumentReference DOC = database
                                   .Collection("College")
                                   .Document(email)
                                   .Collection("Students")
                                   .Document(StudentEmail);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                isExsits = true;
            }

            return isExsits;
        }


        public static string CreateStudent(Student model)
        {
            Connect();
            var task = StudentExsits(model.CollegeEmail, model.Email);
            task.Wait();
            bool isValid = task.Result;

            if (!isValid)
            {
                DocumentReference DOC = database
                                        .Collection("College")
                                        .Document(model.CollegeEmail)
                                        .Collection("Students")
                                        .Document(model.Email);

                Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"BatchYear", model.BatchYear },
                {"Semester", model.Semester },

                {"Email", model.Email },
                {"Name", model.Name },
                {"Mobile", model.Mobile },
                {"EnrollNumber", model.EnrollNumber },

                {"Country", model.Country },
                {"State", model.State },
                {"City", model.City },

                {"Password", model.Email },
                {"Role", "Student" },

            };
                DOC.SetAsync(data);

                return "Student Created";
            }
            else
            {
                return "Student Already Exsits";
            }

        }

        public static string UpdateStudent(Student model)
        {
            Connect();
            var task = StudentExsits(model.CollegeEmail, model.Email);
            task.Wait();
            bool isValid = task.Result;

            if (isValid)
            {
                DocumentReference DOC = database
                                        .Collection("College")
                                        .Document(model.CollegeEmail)
                                        .Collection("Students")
                                        .Document(model.Email);

                Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"BatchYear", model.BatchYear },
                {"Semester", model.Semester },

                {"Name", model.Name },
                {"Mobile", model.Mobile },
                {"EnrollNumber", model.EnrollNumber },

                {"Country", model.Country },
                {"State", model.State },
                {"City", model.City },

                {"Password", model.Email },
                {"Role", "Student" },

            };
                DOC.UpdateAsync(data);

                return "Student Updated";
            }
            else
            {
                return "Student Doesnot Exsits";
            }

        }

        public static async Task<FirestoreStudent> GetStudent(string email, String studentEmail)
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
                FirestoreStudent model = snapshot.ConvertTo<FirestoreStudent>();

                return model;
            }
            else
            {
                return null;
            }
        }

        public static async Task<List<FirestoreStudent>> GetAllStudent(string email)
        {
            Connect();
            List<FirestoreStudent> Students = new List<FirestoreStudent>();

            Query allStudents = database
                                   .Collection("College")
                                   .Document(email)
                                   .Collection("Students");

            QuerySnapshot allStudentsSnapshot = await allStudents.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in allStudentsSnapshot.Documents)
            {

                FirestoreStudent model = documentSnapshot.ConvertTo<FirestoreStudent>();
                Students.Add(model);
            }
            return Students;
        }



        internal static async Task<bool> DeleteStudent(string Email, string StudentEmail)
        {
            Boolean isDeleted = false;

            Connect();
            DocumentReference DOC = database
                                   .Collection("College")
                                   .Document(Email)
                                   .Collection("Students")
                                   .Document(StudentEmail);

                var result = await DOC.DeleteAsync();
                if (result != null)
                {
                    isDeleted = true;
                }
                else
                {
                    isDeleted = false;
                }

            return isDeleted;
        }

        public async static Task<string> CreateExam(Exam model)
        {
            Connect();
            model.Id = Guid.NewGuid().ToString();
            DocumentReference DOC = database
                                        .Collection("College")
                                        .Document(model.CollegeEmail)
                                        .Collection("Exams")
                                        .Document(model.Id);


            model.Start = DateTime.SpecifyKind(new DateTime(model.Start.Ticks) , DateTimeKind.Utc);

            model.End = DateTime.SpecifyKind(new DateTime(model.End.Ticks), DateTimeKind.Utc);

            Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    {"Id", model.Id },
                    {"Semester", model.Semester },

                    {"Name", model.Name },

                    {"Start", model.Start },
                    {"End", model.End },
                    {"Link", model.Link },

                    {"CollegeEmail", model.CollegeEmail },

                };
            await DOC.SetAsync(data);

            return "Exam Created";
        }

        internal static async Task<bool> DeleteExam(string Email, string Id)
        {
            Boolean isDeleted = false;

            Connect();
            DocumentReference DOC = database
                                   .Collection("College")
                                   .Document(Email)
                                   .Collection("Exams")
                                   .Document(Id);

            var result = await DOC.DeleteAsync();
            if (result != null)
            {
                isDeleted = true;
            }
            else
            {
                isDeleted = false;
            }

            return isDeleted;
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

    }
}
