using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NFleetSDK.UnitTests
{
    class ResponseReader
    {
        public static Dictionary<string, Response> readResponses(string responsePath)
        {
            var rootdirinfo = new DirectoryInfo(responsePath);
            var responses = new Dictionary<string, Response>();
            var files = rootdirinfo.GetFiles("*.txt");
            foreach (System.IO.FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.FullName);
                bool json = false;
                var resp = new Response();
                string jsontmp = "";
                foreach (var line in System.IO.File.ReadAllLines(file.FullName))
                {
                    var trimmedline = line.Trim();
                    if (trimmedline.Length > 0 && trimmedline[0] == '{') json = true;
                    if (!json)
                    {
                        resp.Headers += line;
                    }
                    else
                    {
                        jsontmp += line;
                    }
                }
                resp.json = JsonConvert.DeserializeObject<JObject>(jsontmp);
                responses.Add(key, resp);
            }
            return responses;
        }

    }

    class Response
    {
        public string Headers = "";
        public JObject json;
    }
}
