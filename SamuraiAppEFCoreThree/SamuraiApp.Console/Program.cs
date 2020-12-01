using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.CLI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            _context.Database.EnsureCreated();
            //GetSamurais("Before add");
            //AddSamurai();
            InsertMultipleSamurais();
            //QueryFilters();
            //RemoveSamurai(2);
            GetSamurais();
            Console.Write("press any key...");
            Console.ReadKey();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Michael" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertMultipleSamurais()
        {
            var michael = new Samurai { Name = "Michael" };
            var jessica = new Samurai { Name = "Jessica" };
            var noah = new Samurai { Name = "Noah" };
            var em = new Samurai { Name = "Emelia" };
            var stella = new Samurai { Name = "Stella Bella" };
            _context.Samurais.AddRange(michael, jessica, noah, em, stella);
            _context.SaveChanges();
        }

        private static void GetSamurais()
        {
            var samurais = _context.Samurais.ToList();
            WriteSamurais(samurais);
        }

        private static void QueryFilters()
        {
            var name = "Noah";
            Console.WriteLine("Query Filter: First or Default on name");
            WriteSamurais(_context.Samurais.FirstOrDefault(s => s.Name == name));

            Console.WriteLine("Query Filter: Find(2)");
            WriteSamurais(_context.Samurais.Find(2));

            Console.WriteLine("Query Filter: Like %N%");
            WriteSamurais(_context.Samurais.Where(s => EF.Functions.Like(s.Name, "N%")));
        }

        private static void RemoveSamurai(int id)
        {
            var samuari = _context.Samurais.Find(id);
            _context.Samurais.Remove(samuari);
            _context.SaveChanges();
        }

        private static void WriteSamurais(IEnumerable<Samurai> samurais)
        {
            Console.WriteLine($"Samurai count is {samurais.Count()}");
            foreach (var samurai in samurais)
            {
                WriteSamurais(samurai);
            }
        }

        private static void WriteSamurais(Samurai samurai)
        {
            Console.WriteLine($"Id: {samurai.Id} Name: {samurai.Name}");
        }

        private static void QueryAndUpdateBattle_Disconnected()
        {
            // tells the context not to track the results of this query
            var battle = _context.Battles.AsNoTracking().FirstOrDefault();
            battle.EndDate = new DateTime(1560, 06, 30);
            using(var newContextInstance = new SamuraiContext())
            {
                // the update method marks the item as modified
                // this lets us know that we have made changes outside of what the context knows
                newContextInstance.Battles.Update(battle);
                newContextInstance.SaveChanges();
            }
        }
    }
}
