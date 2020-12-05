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

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote {Text = "I've come to save you."}
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            // When i retrieve the samurai its being tracked so it knows when i add a quote.
            var samuari = _context.Samurais.FirstOrDefault();
            samuari.Quotes.Add(new Quote { Text = "I bet you're happy that I've saved you!" });
            _context.SaveChanges();
        }

        private static void AddQuoteToExistingSamuraiNotTracked(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            samurai.Quotes.Add(new Quote
            {
                Text = "Now that I saved you, will you feed me dinnner?"
            });
            using (var newContext = new SamuraiContext())
            {
                // use attach here since we didn't update the samurai, just adding a quote
                // if you use update it will work, but it will run and unnescessary query to update the samurai
                newContext.Samurais.Attach(samurai);
                newContext.SaveChanges();
            }
        }

        private static void AddQuoteToExistingSamuraiNotTracked_SetForeignKey(int samuraiId)
        {
            var quote = new Quote
            {
                Text = "Now that I saved you, will you feed me dinnner?",
                SamuraiId = samuraiId
            };
            using (var newContext = new SamuraiContext())
            {
                // you don't have to retriee the whole samurai to add a quote to it, just set the foreign key
                // on the quote object and then save, next time the samurai is retrieved the quote will be theere
                newContext.Quotes.Add(quote);
                newContext.SaveChanges();
            }
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            var samuraiWithQuotes = _context.Samurais
                .Include(s => s.Quotes)
                .ToList();
            // to include further children chain a .ThenInclude to the quotes
            // i.e. Samurais.Include(s => s.Quotes).ThenInclude(q => q.Translations)
        }

        private static void EagerLoadSamuraiWithQuotesAndClans()
        {
            var samuraiWithQuotes = _context.Samurais
                .Include(s => s.Quotes)
                .Include(s => s.Clan)
                .ToList();
        }

        private static void RunQueryAndProjectSomeProperties()
        {
            //var someProperties = _context.Samurais
            //    .Select(s => new { s.Id, s.Name, s.Quotes.Count })
            //    .ToList();
            var someProperties = _context.Samurais
                .Select(s => new
                {
                    Samurai = s,
                    HappyQuotes = s.Quotes.Where(q => q.Text.Contains("happy"))
                })
                .ToList();
        }

        private static void ExplicitLoadQuotes()
        {
            var samurai = _context.Samurais
                .FirstOrDefault(s => s.Name.Contains("Julie"));
            
            _context
                .Entry(samurai)
                .Collection(s => s.Quotes)
                .Load();

            _context
                .Entry(samurai)
                .Reference(s => s.Horse)
                .Load();
        }

        private static void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais
                .Where(s => s.Quotes
                    .Any(q => q.Text.Contains("happy")))
                .ToList();
        }

        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais
                .Include(s => s.Quotes)
                .FirstOrDefault(s => s.Id == 2);
            samurai.Quotes[0].Text = " did you hear that?";
            _context.SaveChanges();
        }

        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais
                .Include(s => s.Quotes)
                .FirstOrDefault(s => s.Id == 2);
            var quote = samurai.Quotes[0];
            quote.Text += " Did you hear that again?";
            using(var newContext = new SamuraiContext())
            {
                /// this will update all of the quotes note just the changed one.
                /// because it will update everything in the graph related to the quote, 
                /// which is the samurai and the full list of quotes
                //newContext.Quotes.Update(quote);

                /// this fixes it so only the updated quote will be saved
                newContext.Entry(quote).State = EntityState.Modified;
                newContext.SaveChanges();
            }
        }

        private static void JoinBattleAndSamurai()
        {
            var sbJoin = new SamuraiBattle { SamuraiId = 1, Battleid = 3 };
            _context.Add(sbJoin);
            _context.SaveChanges();
        }

        private static void EnlistSamuraiIntoABattle()
        {
            var battle = _context.Battles.Find(1);
            battle.SamuraiBattles
                .Add(new SamuraiBattle { SamuraiId = 21 });
            _context.SaveChanges();
        }

        private static void GetSamuraiWithBattles()
        {
            var samuraiWithBattle = _context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(s => s.Battle)
                .FirstOrDefault(s => s.Id == 2);

            var samuraiBattlesCleaner = _context.Samurais
                .Where(s => s.Id == 2)
                .Select(s => new
                {
                    Samurai = s,
                    Battles = s.SamuraiBattles.Select(sb => sb.Battle)
                })
                .FirstOrDefault();

        }
    }
}
