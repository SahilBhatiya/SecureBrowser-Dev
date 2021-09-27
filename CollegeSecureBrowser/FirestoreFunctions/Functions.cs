using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            var json = new WebClient().DownloadString("https://sahilbhatiya.me/PrivateData/key.json");

            Console.WriteLine(json);

            var jsonString = json.ToString();
            try
            {
                var builder = new FirestoreClientBuilder { JsonCredentials = jsonString };
                database = FirestoreDb.Create("exam-proctor-project", builder.Build());
            }
            catch(Exception e)
            {
                Console.WriteLine("\n\n\n\n\n\nError \n"+ e.ToString() + "\n\n"+ e.InnerException +"\n\n\n\n\n");
            }


/*            String path = AppDomain.CurrentDomain.BaseDirectory + @"FirestoreFunctions\exam-proctor-project.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            database = FirestoreDb.Create("exam-proctor-project");*/

            return "Sucess";
        }

        public static string CreateCollege(College college)
        {
            if (!CollegeExsits(college.Name))
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
                {"DefaultLink", college.DefaultLink },
            };
                DOC.SetAsync(data);

                return "College Created";
            }
            else
            {
                return "College Already Exsits";
            }

        }

        private static bool CollegeExsits(string name)
        {
            bool isExsits = false;
            return isExsits;
        }
    }
}
