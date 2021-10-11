using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AdminSecureBrowser.OtherFunctions.Functions;
using AdminSecureBrowser.OtherFunctions.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using static Google.Cloud.Firestore.V1.Firestore;

namespace AdminSecureBrowser.FirestoreFunctions
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

            //var file = AppDomain.CurrentDomain.BaseDirectory + @"\FirestoreFunctions\Key2.json";
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

        internal static async Task<bool> DeleteAdmin(string Email, string password)
        {
            Boolean isAdminDeleted = false;

            Connect();
            DocumentReference DOC = database
                                   .Collection("Admin")
                                   .Document(Email);

            Admin model = new Admin()
            {
                Email = Email,
                Password = Hashing.ComputeSha256Hash(password),
            };

            bool isValid = await VerifyAdmin(model);

            if (isValid)
            {
                var result = await DOC.DeleteAsync();
                if (result != null)
                {
                    isAdminDeleted = true;
                }
                else
                {
                    isAdminDeleted = false;
                }
            }
            else
            {
                isAdminDeleted = false;
            }


            return isAdminDeleted;
        }

        public static async Task<bool> VerifyAdmin(Admin model)
        {
            Connect();
            DocumentReference DOC = database
                                   .Collection("Admin")
                                   .Document(model.Email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                FirestoreAdmin firestoreAdmin = snapshot.ConvertTo<FirestoreAdmin>();
                if (model.Password == firestoreAdmin.Password)
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


        public static async Task<bool> UpdateAdmin(Admin model)
        {
            Connect();
            DocumentReference DOC = database
                       .Collection("Admin")
                       .Document(model.Email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> data = new Dictionary<string, object>()
                {
                    {"Country", model.Country },
                    {"State", model.State },
                    {"City", model.City },
                    {"Pincode", model.Pincode },
                    {"Mobile", model.Mobile },
                    {"Name", model.Name }
                };

                await DOC.UpdateAsync(data);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> UpdatePassword(Admin model)
        {
            Connect();
            DocumentReference DOC = database
                       .Collection("Admin")
                       .Document(model.Email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                FirestoreAdmin firestoreAdmin = snapshot.ConvertTo<FirestoreAdmin>();

                if(firestoreAdmin.Password == Hashing.ComputeSha256Hash(model.Password))
                {
                    Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        {"Password", Hashing.ComputeSha256Hash(model.NewPassword) }
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

        public static string CreateAdmin(Admin model)
        {
            Connect();
            bool isValid = false;

            if (!isValid)
            {
                DocumentReference DOC = database
    .Collection("Admin")
    .Document(model.Email);
                Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Country", model.Country },
                {"State", model.State },
                {"City", model.City },
                {"Pincode", model.Pincode },
                {"Mobile", model.Mobile },
                {"Email", model.Email },
                {"Name", model.Name },
                {"Password", model.Password },
                {"Role", "Admin" },
            };
                DOC.SetAsync(data);

                return "Admin Created";
            }
            else
            {
                return "Admin Already Exsits";
            }

        }

        public static async Task<FirestoreAdmin> GetAdmin(string email)
        {
            Connect();
            bool isExsits = false;

            DocumentReference DOC = database
                                   .Collection("Admin")
                                   .Document(email);

            DocumentSnapshot snapshot = await DOC.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                FirestoreAdmin firestoreAdmin = snapshot.ConvertTo<FirestoreAdmin>();

                return firestoreAdmin;
            }
            else
            {
                return null;
            }
        }


    }
}
