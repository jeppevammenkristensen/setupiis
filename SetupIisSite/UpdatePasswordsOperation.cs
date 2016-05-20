using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Microsoft.Web.Administration;

namespace SetupIisSite
{
    public class UpdatePasswordsOperation : IOperation
    {
        public void Run(string[] args)
        {
            var userName = GetUserName();
            var password = getPassword();

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                if (!context.ValidateCredentials(userName, password.ToString()))
                {
                    throw new InvalidOperationException("Error validating credentials");
                }
            }

            SetupPasswords(userName,password);
        }

        private void SetupPasswords(string username, string password)
        {
            var manager = new ServerManager();
            foreach (var source in manager.ApplicationPools.Where(x => x.ProcessModel.UserName == username))
            {
                var pool = manager.ApplicationPools.FirstOrDefault(x => x.Name == source.Name);
                Console.WriteLine($"Updating ${pool?.Name}");
                pool.ProcessModel.Password = password;
                
            }

            manager.CommitChanges();
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