namespace CryptoExperiments
{
    using System;
    using System.Runtime.ExceptionServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    internal partial class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

                var t = new Task(Run);
                t.Start();
                t.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // This is a solution for CSE not to break your app.
        private static void ExecInAnotherDomain()
        {
            AppDomain dom = null;

            try
            {
                dom = AppDomain.CreateDomain("newDomain");
                var p = new MethodParams() { BeenThere = false };
                var o = (BoundaryLessExecHelper)dom.CreateInstanceAndUnwrap(typeof(BoundaryLessExecHelper).Assembly.FullName, typeof(BoundaryLessExecHelper).FullName);
                Console.WriteLine("Before call");

                o.DoSomething(p, CausesAccessViolation);
                Console.WriteLine("After call. param been there? : " + p.BeenThere.ToString()); // never gets to here
            }
            catch (Exception exc)
            {
                Console.WriteLine($"CSE: {exc.ToString()}");
            }
            finally
            {
                AppDomain.Unload(dom);
            }

            Console.ReadLine();
        }


        private static void Run()
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            Console.WriteLine($"Is linux: {isLinux}");

            var api = new LinuxCryptoApi();

            var testData = Encoding.UTF8.GetBytes("Hello world!");

            Console.WriteLine("================= Hashing ======================");
            var hash = api.ComputeHash(string.Empty, testData);
            Console.WriteLine($"Hash: {Convert.ToHexString(hash)}");
            Console.WriteLine();

            Console.WriteLine("================= Certs store reading ======================");
            var c = api.FindByThumbprint(Convert.FromHexString("68da674f6c7c1eb57a2ec53becb0892a9247d632"));
            Console.WriteLine(
                $"Subject: {c?.SubjectName.Name}, Private key:{c?.HasPrivateKey}, " +
                $"Thumbprint: {c?.Thumbprint}, Serial: {c?.SerialNumber}");
            Console.WriteLine();

            Console.WriteLine("================= Encrypt ======================");
            var encryptedTestData = api.Encrypt(c!, testData);
            Console.WriteLine($"Encrypted test data: {Convert.ToHexString(encryptedTestData)}");
            Console.WriteLine();
        }

        // but it's important that this method is marked
        [SecurityCritical]
        [HandleProcessCorruptedStateExceptions]
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // this will catch all unhandled exceptions, including CSEs
            Console.WriteLine(e.ExceptionObject as Exception);
        }
    }
}
