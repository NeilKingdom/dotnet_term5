using Assignment2.Models;

namespace Assignment2.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MarketDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Clients.Any())
            {
                return; // DB has been seeded
            }

            /*** Add clients to DB ***/
            var clients = new List<Client>
            {
                new Client { FirstName = "Carson",      LastName = "Alexander",     BirthDate = DateTime.Parse("1995-01-09") },
                new Client { FirstName = "Meredith",    LastName = "Alonso",        BirthDate = DateTime.Parse("1992-09-05") },
                new Client { FirstName = "Arturo",      LastName = "Anand",         BirthDate = DateTime.Parse("1993-10-09") },
                new Client { FirstName = "Gytis",       LastName = "Barzdukas",     BirthDate = DateTime.Parse("1992-01-09") }
            };
            clients.ForEach(client => context.Add(client));
            context.SaveChanges();

            /*** Add Brokerages to DB ***/
            var brokerages = new List<Brokerage>
            {
                new Brokerage { BrokerageId ="A1",    Title = "Alpha",      Fee = 300 },
                new Brokerage { BrokerageId ="B1",    Title = "Beta",       Fee = 130 },
                new Brokerage { BrokerageId ="O1",    Title = "Omega",      Fee = 390 }
            };
            brokerages.ForEach(brokerage => context.Add(brokerage));
            context.SaveChanges();

            /*** Add Subscriptions to DB ***/
            var subscriptions = new List<Subscription>
            {
                new Subscription { ClientId = 1,      BrokerageId = "A1" },
                new Subscription { ClientId = 1,      BrokerageId = "B1" },
                new Subscription { ClientId = 1,      BrokerageId = "O1" },
                new Subscription { ClientId = 2,      BrokerageId = "A1" },
                new Subscription { ClientId = 2,      BrokerageId = "B1" },
                new Subscription { ClientId = 3,      BrokerageId = "A1" }
            };
            subscriptions.ForEach(subscription => context.Add(subscription));
            context.SaveChanges();
        }
    }
}
