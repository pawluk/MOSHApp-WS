// Project: MoshAppService
// Filename: DataUploadService.cs
// 
// Author: Jason Recillo

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

using JetBrains.Annotations;

using MoshAppService.Service.Data;
using MoshAppService.Service.Database;
using MoshAppService.Utils;

using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace MoshAppService.Service.Endpoints {
    [Route("/upload", "POST")]
    public class DataUpload { }

    /// <summary>
    /// Processes uploaded CSV and JSON files and adds the user information to the database.
    /// </summary>
    public class DataUploadService : ServiceStack.ServiceInterface.Service {
        private DateTime _start;

        public object Post(DataUpload unused) {
            //TODO: Only administrators should be able to do this

            _start = DateTime.Now;
            if (RequestContext.Files.Length == 0) return new HttpResult(HttpStatusCode.BadRequest, "No file was uploaded.");
            var file = RequestContext.Files[0];
            string data;
            using (var reader = new StreamReader(file.InputStream))
                data = reader.ReadToEnd();

            // Try and process the data as CSV
            List<User> users;
            try {
                Log("CSV Deserialize");
                var tmp = JsonSerializer.DeserializeFromString<List<Dictionary<string, string>>>(CsvToJson(data));

                // The above line executed successfully? Great! Now reassign the data variable
                // to contain the JSON version of its contents and then throw an InvalidDataException
                // to execute the validation code below
                Log("CSV Deserialize success");
                data = tmp.ToJson();
                throw new InvalidDataException();
            } catch (InvalidDataException) {
                try {
                    Log("JSON Deserialize");
                    // It's not a CSV file. Let's try to process it as JSON

                    // First of all, make sure that there aren't any unexpected fields
                    // Start by converting the data from JSON into a string Dictionary
                    var check = JsonSerializer.DeserializeFromString<List<Dictionary<string, string>>>(data);

                    // Don't hardcode any fields here, in case we change any fields in the User class.
                    // instead, convert a plain User object into a Dictionary<string, string>, and compare all keys
                    var testusr = JsonSerializer.DeserializeFromString<Dictionary<string, string>>(new User().ToJson());

                    // data will be invalid if any of the keys in `check` are not in the `testusr` keys.
                    // At the very least however, first name, last name, and student ID are required
                    Log("Check valid");
                    var valid = check.Aggregate(true, (current, ch) => current &
                                                                       ch.Keys.ContainsAny(false, testusr.Keys.ToArray()) &
                                                                       ch.Keys.ContainsAll(false, "firstName", "lastName", "studentNumber"));
                    Log("Check valid complete: {0}", valid);
                    if (!valid) throw new SerializationException();

                    Log("JSON Deserialize success");

                    users = JsonSerializer.DeserializeFromString<List<User>>(data);
                } catch (SerializationException) {
                    Log("Invalid input");
                    // This isn't a JSON file either. Let's bail out.
                    return new HttpError(HttpStatusCode.BadRequest, "");
                }
            }
            Log(users.Dump());

            //TODO: Save the user data to the database
            var output = new List<dynamic>();
            foreach (var u in users) {
                dynamic credentials;
                UserDbProvider.Instance.CreateUser(u, out credentials);
                output.Add(credentials);
            }
            Log(output.Dump());

            // Return a list of all of the login names and passwords
            return output;
        }

        // slightly modified from
        // http://stackoverflow.com/questions/10824165/converting-a-csv-file-to-json-using-c-sharp
        // ServiceStack doesn't have a working implementation for converting CSV to JSON
        // (CsvSerializer's Serialize/Deserialize methods throw NotImplementedException),
        // so we have to do it ourselves
        private static string CsvToJson(string value) {
            if (value == null) return null;
            var lines = value.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) throw new InvalidDataException("Must have header line.");

            var headers = lines.First().Split(new[] { ',' }, StringSplitOptions.None);

            var sb = new StringBuilder();
            sb.Append("[");
            for (var i = 1; i < lines.Length; i++) {
                var fields = lines[i].Split(new[] { ',' }, StringSplitOptions.None);
                if (fields.Length != headers.Length) throw new InvalidDataException("Field count must match header count.");
                var jsonElements = headers.Zip(fields, (header, field) => {
                    var h = header.Replace("\"", "\\\"");
                    var f = field.Replace("\"", "\\\"");
                    return "\"{0}\":\"{1}\"".Fmt(h, f);
                }).ToArray();
                var jsonObject = "{" + "{0}".Fmt(string.Join(",", jsonElements)) + "}";
                if (i < lines.Length - 1) jsonObject += ",";
                sb.AppendLine(jsonObject);
            }
            sb.Append("]");
            return sb.ToString();
        }

        [DebuggerHidden]
        [StringFormatMethod("message")]
        private void Log(string message, params object[] args) {
#if DEBUG
            var m = args != null && !args.Empty() ? message.Fmt(args) : message;
            Global.Log.Debug("[{0} ms]: {1}".Fmt((DateTime.Now - _start).TotalMilliseconds, m));
#endif
        }
    }
}
