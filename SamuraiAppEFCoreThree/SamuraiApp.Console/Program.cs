using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SamuraiApp.CLI
{
    class Program
    {
        private static SamuraiContext _context;
        public static IConfigurationRoot Configuration;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true);

            Configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddDbContext<SamuraiContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("SamuraiConnex"))
                    .EnableSensitiveDataLogging()
            );
            var serviceProvider = services.BuildServiceProvider();
            _context = serviceProvider.GetService<SamuraiContext>();


            ////_context.Database.EnsureCreated(); // This was just for demonstration purposes, you wouldn't normally do this.
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
            //using (var newContextInstance = new SamuraiContext())
            //{
            //    // the update method marks the item as modified
            //    // this lets us know that we have made changes outside of what the context knows
            //    newContextInstance.Battles.Update(battle);
            //    newContextInstance.SaveChanges();
            //}
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
            //using (var newContext = new SamuraiContext())
            //{
            //    // use attach here since we didn't update the samurai, just adding a quote
            //    // if you use update it will work, but it will run and unnescessary query to update the samurai
            //    newContext.Samurais.Attach(samurai);
            //    newContext.SaveChanges();
            //}
        }

        private static void AddQuoteToExistingSamuraiNotTracked_SetForeignKey(int samuraiId)
        {
            var quote = new Quote
            {
                Text = "Now that I saved you, will you feed me dinnner?",
                SamuraiId = samuraiId
            };
            //using (var newContext = new SamuraiContext())
            //{
            //    // you don't have to retriee the whole samurai to add a quote to it, just set the foreign key
            //    // on the quote object and then save, next time the samurai is retrieved the quote will be theere
            //    newContext.Quotes.Add(quote);
            //    newContext.SaveChanges();
            //}
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
            //using(var newContext = new SamuraiContext())
            //{
            //    /// this will update all of the quotes note just the changed one.
            //    /// because it will update everything in the graph related to the quote, 
            //    /// which is the samurai and the full list of quotes
            //    //newContext.Quotes.Update(quote);

            //    /// this fixes it so only the updated quote will be saved
            //    newContext.Entry(quote).State = EntityState.Modified;
            //    newContext.SaveChanges();
            //}
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

        private static void AddNewHorseToSamuraiUsingId()
        {
            // add a horse to a samurai without retrieving the samurai
            var horse = new Horse { Name = "Scott", SamuraiId = 2 };
            _context.Add(horse);
            _context.SaveChanges();
        }

        private static void AddNewHorstToSamuraiObject()
        {
            var samurai = _context.Samurais.Find(3);
            samurai.Horse = new Horse { Name = "Mr. Ed" };
            _context.SaveChanges();
        }

        private static void AddNewHorseToSamuraiObject_Disconnected()
        {
            var samurai = _context.Samurais.AsNoTracking().FirstOrDefault(s => s.Id == 4);
            samurai.Horse = new Horse { Name = "Black Beauty" };
            //using (var newContext = new SamuraiContext())
            //{
            //    newContext.Attach(samurai);
            //    newContext.SaveChanges();
            //}
        }

        /// <summary>
        /// Getting data with a "clean" relationship
        /// i.e. samurai only has a navigation property and no clan id reference
        ///     and clan doesn't have a list of samurais
        /// </summary>
        private static void GetClanWithSamurais()
        {
            var clan = _context.Clans.Find(3);
            var samuraisForClan = _context.Samurais.Where(s => s.Clan.Id == clan.Id).ToList();
        }

        /// <summary>
        /// Example of Querying an entity from a View
        /// Querying data is the same as querying from a table
        /// obviosly the difference is the data is read only
        /// since this is keyless you can't use find!
        /// </summary>
        private static void QuerySamuraiBattleStats()
        {
            var stats = _context.SamuraiBattleStats.ToList();
        }

        /// <summary>
        /// Example of using the dbset to query with raw sql
        /// RULES:
        ///     1. Must return data for all properties of the entity type
        ///     2. Column names must match mapped column names
        ///     3. Query can't contain related data
        ///     4. Can only be used for entities known by dbcontext
        /// </summary>
        private static void QuerySamuraiUsingRawSql()
        {
            var samurais = _context.Samurais
                .FromSqlRaw("SELECT * FROM samurais")
                .ToList();

            string name = "Kikuchyo";
            samurais = _context.Samurais
                .FromSqlInterpolated($"SELECT * FROM Samurais where name = {name}")
                .ToList();
        }

        /// <summary>
        /// Example of querying data by calling a stored procedure
        ///     1. Example using raw sql
        ///     2. Example using interpolated
        /// </summary>
        private static void QueryUsingRawSqlStoredProcedure()
        {
            var text = "Happy";
            var samurais = _context.Samurais
                .FromSqlRaw(
                    "EXEC dbo.SamuraiWhoSaidAWord {0}", text)
                .ToList();

            samurais = _context.Samurais
                .FromSqlInterpolated($"EXEC dbo.SamuraiWhoSaidAWord {text}")
                .ToList();
        }

        /// <summary>
        /// Example of Execute a stored procedure not releated to the db context
        ///     1. Raw sql example
        ///     2. interpolated example
        /// </summary>
        /// <param name="samuraiId"></param>
        private static void RemoveAllQuotesFromSamurai(int samuraiId)
        {
            var numberOfRowsImpacted = _context.Database
                .ExecuteSqlRaw("EXEC DeleteQuotesForSamurai {0}", samuraiId);

            numberOfRowsImpacted = _context.Database
                .ExecuteSqlInterpolated($"EXEC DeleteQuotesForSamura {samuraiId}");
        }
    }
}
