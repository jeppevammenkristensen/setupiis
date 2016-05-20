using System;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.Web.Administration;

namespace SetupIisSite
{
    class Program
    {
        static void Main(string[] args)
        {
            var operations = new IOperation[] {new SetupIisSiteOperation(), new UpdatePasswordsOperation()};


            for (var i = 0; i < operations.Length; i++)
            {
                Console.WriteLine($"{i}:{operations[i].GetType().Name}");
            }

            // Ugly but we subtract the int value of the char 0
            var operationIndex = Console.ReadKey().KeyChar - '0';

            IOperation operation = operations[operationIndex];

            operation.Run(args);
            
        }
    }
}
