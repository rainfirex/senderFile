using System;
using System.Threading;
using senderFile.Modules;

namespace senderFile
{
    class Program
    {
        private static int tickTime = 5;

        static void Main(string[] args)
        {            
            bool isTimer = true;
            while (isTimer)
            {
                Console.WriteLine($"Запуск рассылки через: { tickTime }");
                tickTime = tickTime - 1;
                Thread.Sleep(1000);

                if (tickTime <= 1)
                {
                    isTimer = false;
                    Console.WriteLine("Запуск рассылки..");
                    FileSender  fs = new FileSender();
                    fs.Run();
                }
            }         
        }
    }
}
