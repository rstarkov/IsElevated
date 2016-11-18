using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace IsElevated
{
    class Program
    {
        static void Main(string[] args)
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            Console.WriteLine($"Administrator: {(principal.IsInRole(WindowsBuiltInRole.Administrator) ? "YES" : "NO")}");
            Console.WriteLine($"Integrity: {ProcessIntegrity.GetCurrentProcessIntegrity()}");
        }
    }
}
