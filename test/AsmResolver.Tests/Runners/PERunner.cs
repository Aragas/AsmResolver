using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using AsmResolver.IO;
using AsmResolver.PE.File;
using Xunit;
using Xunit.Abstractions;

namespace AsmResolver.Tests.Runners
{
    public abstract class PERunner
    {
        protected PERunner(string basePath)
        {
            BasePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }

        public string BasePath
        {
            get;
        }

        protected abstract string ExecutableExtension
        {
            get;
        }

        public void RebuildAndRun(PEFile peFile, string fileName, string expectedOutput, int timeout = 30000,
            [CallerFilePath] string testClass = "File",
            [CallerMemberName] string testMethod = "Test")
        {
            string fullPath = Rebuild(peFile, fileName, testClass, testMethod);
            string actualOutput = RunAndCaptureOutput(fullPath, null, timeout);
            Assert.Equal(expectedOutput.Replace("\r\n", "\n"), actualOutput);
        }

        public string GetTestDirectory(string testClass, string testName)
        {
            string path = Path.Combine(BasePath, testClass, testName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public string GetTestExecutablePath(string testClass, string testMethod, string fileName)
        {
            return Path.ChangeExtension(Path.Combine(GetTestDirectory(testClass, testMethod), fileName), ExecutableExtension);
        }

        public string Rebuild(PEFile peFile, string fileName, string testClass, string testMethod)
        {
            testClass = Path.GetFileNameWithoutExtension(testClass);
            string fullPath = GetTestExecutablePath(testClass, testMethod, fileName);

            using var fileStream = File.Create(fullPath);
            peFile.Write(new BinaryStreamWriter(fileStream));

            return fullPath;
        }

        public string RunAndCaptureOutput(string fileName, byte[] contents, string[]? arguments = null,
            int timeout = 30000,
            [CallerFilePath] string testClass = "File",
            [CallerMemberName] string testMethod = "Test",
            ITestOutputHelper? outputHelper = null)
        {
            testClass = Path.GetFileNameWithoutExtension(testClass);
            string testExecutablePath = GetTestExecutablePath(testClass, testMethod, fileName);
            File.WriteAllBytes(testExecutablePath, contents);
            return RunAndCaptureOutput(testExecutablePath, arguments, timeout, outputHelper);
        }

        public string RunAndCaptureOutput(string filePath, string[]? arguments = null, int timeout = 30000, ITestOutputHelper? outputHelper = null)
        {
            var info = GetStartInfo(filePath, arguments);
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.WorkingDirectory = Path.GetDirectoryName(filePath);

            using var process = new Process();

            process.StartInfo = info;
            process.OutputDataReceived += (sender, args) => outputHelper?.WriteLine("received output: {0}", args.Data);
            process.ErrorDataReceived += (sender, args) => outputHelper?.WriteLine("received error: {0}", args.Data);
            process.Start();

            if (!process.WaitForExit(timeout))
            {
                try
                {
                    process.Kill();
                }
                catch (InvalidOperationException)
                {
                    // Process has already exited.
                }

                string outputString = process.StandardOutput.ReadToEnd();
                string errorString = process.StandardError.ReadToEnd();
                string nl = Environment.NewLine;
                throw new TimeoutException($"Failed to WaitForExit({timeout})!{nl}Output:{nl}{outputString}", new RunnerException(process.ExitCode, errorString));
            }

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string errorString = process.StandardError.ReadToEnd();
                throw new RunnerException(process.ExitCode, errorString);
            }

            return process.StandardOutput.ReadToEnd().Replace("\r\n", "\n");
        }

        protected abstract ProcessStartInfo GetStartInfo(string filePath, string[]? arguments);
    }
}
