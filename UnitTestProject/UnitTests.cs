using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    using System.Diagnostics;
    using System.Net.Http;

    [TestClass]
    public class UnitTests
    {
        private static Process _iis1;
        private static Process _iis2;


        [TestInitialize]
        public void Initialize()
        {
            // start two instances of IIS Express pointing to the deployments

            var deployedPath = ResolveRelativePath(AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\..\WebMultiDeploy\Deployed\");

            _iis1 = StartIisExpress(deployedPath + "Debug", 8001);
            _iis2 = StartIisExpress(deployedPath + "Release", 8002);
        }


        [TestMethod]
        public void TestMethod()
        {
            var response = HTTPCall("http://localhost:8001/");
            Assert.AreEqual(true, response.Result.Contains("Debug"));

            response = HTTPCall("http://localhost:8002/");
            Assert.AreEqual(true, response.Result.Contains("Release"));
        }

        private static async Task<string> HTTPCall(string url)
        {
            var c = new HttpClient();
            return await c.GetStringAsync(url);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Stop our IIS Express instances
            if (!_iis1.HasExited)
            {
                _iis1.Kill();
            }
            _iis1.Dispose();

            if (!_iis2.HasExited)
            {
                _iis2.Kill();
            }
            _iis2.Dispose();
        }

        #region helpers
        private static Process StartIisExpress(string path, int port)
        {
            var iisExpressPath = string.Format("{0}\\IIS Express\\iisexpress.exe",
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

            var iisProcess = new Process
            {
                StartInfo =
                {
                    FileName = iisExpressPath,
                    Arguments = string.Format("/path:\"{0}\" /port:{1}", path, port),
                    UseShellExecute = true
                }
            };

            iisProcess.Start();
            return iisProcess;
        }
    
        public static string ResolveRelativePath(string referencePath, string relativePath)
        {
            var uri = new Uri(Path.Combine(referencePath, relativePath));
            return Path.GetFullPath(uri.AbsolutePath);
        }
        #endregion
    }
}
