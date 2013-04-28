using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

using MoshAppService.Service.Data;
using MoshAppService.Utils;

using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace MoshAppService.Service.Endpoints {
    [Route("/upload", "POST")]
    public class DataUpload {
        public string Name { get; set; }
        public string TextContents { get; set; }
    }

    public class DataUploadService : ServiceStack.ServiceInterface.Service {
        public object Post(DataUpload request) {
            if (RequestContext.Files.Length == 0) return new HttpResult(HttpStatusCode.BadRequest, "No file was uploaded.");
            var file = RequestContext.Files[0];
            string data;
            using (var reader = new StreamReader(file.InputStream)) {
                data = reader.ReadToEnd();
            }

            // Try and process the data as CSV
            List<User> users;
            try {
                users = JsonSerializer.DeserializeFromString<List<User>>(CsvToJson(data));
            } catch (InvalidDataException) {
                try {
                    // It's not a CSV file. Let's try to process it as JSON
                    users = JsonSerializer.DeserializeFromString<List<User>>(data);
                } catch (SerializationException) {
                    // This isn't a JSON file either. Let's bail out.
                    return new HttpError(HttpStatusCode.BadRequest, "");
                }
            }
            Global.Log.Debug(users.Dump());

            //TODO: Save the user data to the database

            var output = new Dictionary<string, string>(); // will contain generated Username/Password pairs
            foreach (var u in users) {
                string username, password;
                GenerateUserCredentials(u, out username, out password);
                output.Add(username, password);
            }

            return output;
        }

        private static void GenerateUserCredentials(User user, out string username, out string password, int length = 6) {
            username = "{0}{1}".Fmt(user.FirstName.Substring(0, 1), user.LastName).ToLower();
            
            // Generate random password
            var characters = "1234567890abcdefghjkmnpqrstuvwxyzACDEFGHJKLMNPQRSTUVWXYZ".ToCharArray();
            var pb = new StringBuilder(length);
            for (var i = 0; i < length; i++) {
                pb.Append(characters.GetRandom());
            }
            password = pb.ToString();
        }

        // slightly modified from
        // http://stackoverflow.com/questions/10824165/converting-a-csv-file-to-json-using-c-sharp
        private static string CsvToJson(string value) {
            if (value == null) return null;
            var lines = value.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) throw new InvalidDataException("Must have header line.");

            var headers = lines.First().Split(new[] { ',' }, StringSplitOptions.None);

            var sb = new StringBuilder();
            sb.Append("[");
            for (var i = 1; i < lines.Length; i++) {
                var fields = lines[i].Split(new[] { ',' }, StringSplitOptions.None);
                if(fields.Length!=headers.Length)throw new InvalidDataException("Field count must match header count.");
                var jsonElements = headers.Zip(fields, (header, field) => {
                    var h = header.Replace("\"", "\\\"");
                    var f = field.Replace("\"", "\\\"");
                    return "\"{0}\": \"{1}\"".Fmt(h, f);
                }).ToArray();
                var jsonObject = "{" + "{0}".Fmt(string.Join(",", jsonElements)) + "}";
                if (i < lines.Length - 1) jsonObject += ",";
                sb.AppendLine(jsonObject);
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
