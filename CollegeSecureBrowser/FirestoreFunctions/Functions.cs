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
            //var json = new WebClient().DownloadString("https://sahilbhatiya.me/PrivateData/key.json");

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

            return "Sucess";
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
    }
}
