using System.IO;

namespace NFleet.Tests
{
    class ResponseWriter
    {
        public static void Write(string example, string exampleName, string destination)
        {
            string beginExampleTag = "\n//##BEGIN EXAMPLE " + exampleName + "##\n";
            string endExampleTag = "\n//##END EXAMPLE##\n";

            File.WriteAllText(destination, beginExampleTag + example + endExampleTag);
        }

        public static void WriteAll(string directoryPath, string destination, string postfix)
        {
            string contents = "";
            foreach (string file in Directory.EnumerateFiles(directoryPath, "*" + postfix))
            {
                contents += "\n"+File.ReadAllText(file);
            }

            File.WriteAllText(destination, contents);
        }
    }
}
