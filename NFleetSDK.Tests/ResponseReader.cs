using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NFleet.Tests
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
                        addHeader(line, resp.headers);
                    }
                    else
                    {
                        jsontmp += line;
                    }
                }
                resp.json = JsonConvert.DeserializeObject<JObject>(jsontmp);

                if (key != null)
                {
                    responses.Add(key, resp);
                }
            }
            return responses;
        }

        public static void addHeader(string headerLine, Dictionary<string, string> headerDict)
        {
            var splitIndex = headerLine.IndexOf(':');
            if (splitIndex > 0)
            {
                var key = headerLine.Substring(0, splitIndex);
                var value = headerLine.Substring(splitIndex).Trim();
                headerDict.Add(key, value);
            }
        }
    }

    class Response
    {
        public Dictionary<string, string> headers = new Dictionary<string, string>();
        public JObject json;
    }
}
