using System.Diagnostics;
using System.IO;

namespace DebugServer.Properties
{
    internal class Debugger
    {
        public static void StartProcess(Debug entity)
        {
            string filename = entity._id;
            string workingDirectory = "/usr/bin/sudo";
            string argument = "";
            switch (entity.Language)
            {
                case "c#":
                    using (var sr = new StreamWriter(filename + ".cs"))
                    {
                        sr.Write(entity.Code);
                    } break;
                case "java":
                    using (var sr = new StreamWriter(filename + ".java"))
                    {
                        sr.Write(entity.Code);
                        argument = "javac " + filename + ".c";

                    }
                    break;
                 
                case "c":
                    using (var sr = new StreamWriter(filename + ".c"))
                    {
                        sr.Write(entity.Code);
                        argument = "gcc " + filename + ".c";

                    }

                    break;
                case "c++":
                    using (var sr = new StreamWriter(filename + ".cpp"))
                    {
                        sr.Write(entity.Code);
                        argument = "g++ " + filename + ".cpp";
                    }
                    

                    break;
                case "python":
                    using (var sr = new StreamWriter(filename + ".py"))
                    {
                        sr.Write(entity.Code);
                        argument = "python -m py_compile " + filename + ".py";
                    }
                    break;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filename ;
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.Arguments = argument;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            Process process = new Process();
            process.StartInfo = startInfo;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            File.Delete(filename);
        }
    }
}
