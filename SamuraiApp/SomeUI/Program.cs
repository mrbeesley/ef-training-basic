using SamuraiApp.Domain;
using SamuraiApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SomeUI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {

            #region Module on Inserting and updating single source objects.
            //Action<Samurai> logSamurais = s => Console.WriteLine($"Samurai: {s.Id} - {s.Name}");
            //InsertSamurai();
            //InsertMultipleSamurais();
            //SimpleSamuraiQuery(logSamurais);
            //MoreQueries(s => s.Id > 2, logSamurais);
            //RetrieveAndUpdateSamurai(1);
            //RetrieveAndUpdateMultipleSamurais();
            //DeleteWhileTracked();
            //DeleteWhileNotTracked();
            #endregion

            #region Module on Inserting Updating and querying related objects
            //InsertNewPKFKGraph();
            //InsertNewPKFKGraphMultipleChildren();
            //AddChildToExistingObjectWhileTracked();
            //EagerLoadSamuraiWithQuotes();
            //ProjectSomeProperties();
            //ProjectSamuraisWithQuotes();
            //FilteringWithRelatedData();
            //ModifyingRelatedDataWhenTracked();
            //ModifyingRelatedDataWhenNotTracked();
            #endregion // End

            Console.WriteLine("press enter to continue....");
            Console.ReadLine();
        }

        #region Module on Inserting Updating and querying related objects
        private static void ModifyingRelatedDataWhenNotTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault();
            var quote = samurai.Quotes[0];
            quote.Text += " Did you hear that?";
            using (var newContext = new SamuraiContext())
            {
                //newContext.Quotes.Update(quote); // this updates everyting in the graph associated with the changed object
                newContext.Entry(quote).State = EntityState.Modified; // this only updates the speciifc object you are passing it
                newContext.SaveChanges();
            }
        }

        private static void ModifyingRelatedDataWhenTracked()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault();
            samurai.Quotes[0].Text += " Did you hear that?";
            _context.SaveChanges();
        }

        private static void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais
                .Include(s => s.Quotes)
                .Where(s => s.Quotes.Any(q => q.Text.Contains("save")))
                .ToList();
        }

        private static void ProjectSamuraisWithQuotes()
        {
            //var somePropertiesWithQuotes = _context.Samurais
            //    .Select(s => new { s.Id, s.Name, s.Quotes.Count })
            //    .ToList();

            //var somePropertiesWithQuotes = _context.Samurais
            //    .Select(s => new
            //    {
            //        s.Id,
            //        s.Name,
            //        HappyQuotes = s.Quotes.Where(q => q.Text.Contains("save"))
            //    })
            //    .ToList();

            var somePropertiesWithQuotes = _context.Samurais
                .Select(s => new { Samurai = s, Quotes = s.Quotes.Where(q => q.Text.Contains("save")) })
                .ToList();

            somePropertiesWithQuotes.ForEach(s => Console.WriteLine(s.ToString()));
        }

        private static void ProjectSomeProperties()
        {
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
            someProperties.ForEach(s => Console.WriteLine(s.ToString()));
        }

        private static void EagerLoadSamuraiWithQuotes()
        {
            var samuraiWithQuotes = _context.Samurais.Include(s => s.Quotes).ToList();
        }

        private static void AddChildToExistingObjectWhileTracked()
        {
            var samurai = _context.Samurais.First();
            samurai.Quotes.Add(new Quote { Text = "I bet you're happy that i saved you!" });
            _context.SaveChanges();
        }

        private static void InsertNewPKFKGraphMultipleChildren()
        {
            var samurai = new Samurai
            {
                Name = "Chris Beesley",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "I'm too old for this stuff." },
                    new Quote { Text = "This is not the drone ou are lokoing for." }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void InsertNewPKFKGraph()
        {
            var samurai = new Samurai
            {
                Name = "Michael Beesley",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "I've come to save you" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        #endregion 

        #region Module on Inserting and updating single source objects.
        private static void DeleteWhileNotTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "JulieSon");
            using (var context = new SamuraiContext())
            {
                context.Samurais.Remove(samurai);
                context.SaveChanges();
            }
        }

        private static void DeleteWhileTracked()
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == "JulieSon");
            _context.Samurais.Remove(samurai);
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateMultipleSamurais()
        {
            _context.Samurais.ToList().ForEach(s => s.Name += "Son");
            _context.SaveChanges();
        }

        private static void RetrieveAndUpdateSamurai(int id)
        {
            var samurai = _context.Samurais.FirstOrDefault(s => s.Id == id);
            samurai.Name += "Son";
            _context.SaveChanges();
        }

        private static void MoreQueries(Expression<Func<Samurai,bool>> expression, Action<Samurai> action)
        {
            _context.Samurais
                .Where(expression)
                .ToList()
                .ForEach(action);
        }

        private static void SimpleSamuraiQuery(Action<Samurai> action)
        {
            _context.Samurais
                .ToList()
                .ForEach(s => Console.WriteLine($"Samurai: {s.Id} - {s.Name}"));
        }

        private static void InsertMultipleSamurais()
        {
            var samurais = new List<Samurai>
            {
                new Samurai { Name = "Noah" },
                new Samurai { Name = "Emie" }
            };

            var samurai = new Samurai { Name = "Jessica" };
            var samuraiTwo = new Samurai { Name = "Stella" };

            using (var context = new SamuraiContext())
            {

                context.AddRange(samurais);
                context.Samurais.AddRange(samurai, samuraiTwo);   
                context.SaveChanges();
            }
        }

        private static void InsertSamurai()
        {
            var samurai = new Samurai { Name = "JulieSon" };
            using (var context = new SamuraiContext())
            {
                context.Samurais.Add(samurai);
                context.SaveChanges();
            }
        }
        #endregion
    }
}
