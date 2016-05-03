using System;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using Microsoft.Web.Administration;

namespace SetupIisSite
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Get path. Empty will use current");
            var current = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(current))
            {
                current = Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(current))
                    throw new InvalidOperationException("The directory does not exist");

            Console.WriteLine(current);

            var userName = GetUserName();
            var password = getPassword();
            
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                if (!context.ValidateCredentials(userName, password.ToString()))
                {
                    throw new InvalidOperationException("Failed to initialize");
                }
            }

            var name = GetSiteName();


            ServerManager manager = new ServerManager();

            var localVeluxDev = name;
            var appPool = manager.ApplicationPools.FirstOrDefault(x => x.Name == localVeluxDev) ??  manager.ApplicationPools.Add(localVeluxDev);
            appPool.ManagedRuntimeVersion = "v4.0";
            appPool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
            appPool.ProcessModel.UserName = userName;
            appPool.ProcessModel.Password = password;
            manager.CommitChanges();
       
            var site = manager.Sites.Add(localVeluxDev,"http",$"*:80:{localVeluxDev}",current);
            site.ApplicationDefaults.ApplicationPoolName = appPool.Name;

            manager.CommitChanges();
            
        }

        private static string GetSiteName()
        {
            Console.WriteLine("Sitenavn (vil også være hostnavn");
            return Console.ReadLine();
        }

        public static string GetUserName()
        {
            Console.WriteLine("Indtast brugernavn");
            var userName = Console.ReadLine();

            return userName;
        }

        public static string getPassword()
        {
            Console.WriteLine("Indtast password");
            string pwd = string.Empty;
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Remove(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd = pwd + i.KeyChar;
                    Console.Write("*");
                }
            }
            return pwd;
        }
    }

}
