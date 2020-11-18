using SamuraiApp.Data;
using SamuraiApp.Domain;
using sys = System;
using System.Linq;

namespace SamuraiApp.Console
{
    class Program
    {
        private static SamuraiContext context = new SamuraiContext();
        static void Main(string[] args)
        {
            context.Database.EnsureCreated();
            GetSamurais("Before add");
            AddSamurai();
            GetSamurais("After add");
            sys.Console.Write("press any key...");
            sys.Console.ReadKey();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Michael" };
            context.Samurais.Add(samurai);
            context.SaveChanges();
        }

        private static void GetSamurais(string text)
        {
            var samurais = context.Samurais.ToList();
            sys.Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
            foreach (var samurai in samurais)
            {
                sys.Console.WriteLine(samurai.Name);
            }
        }
    }
}
