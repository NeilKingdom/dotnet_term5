using Lab4.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Lab4.Data
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

            foreach (Client client in clients)
            {
                context.Clients.Add(client);
            }

            context.SaveChanges();

            /*** Add Brokerages to DB ***/
            var brokerages = new List<Brokerage>
            {
                new Brokerage { ID="A1",    Title="Alpha",      Fee=300 },
                new Brokerage { ID="B1",    Title="Beta",       Fee=130 },
                new Brokerage { ID="O1",    Title="Omega",      Fee=390 }
            };

            foreach (Brokerage brokerage in brokerages)
            {
                context.Brokerages.Add(brokerage);
            }

            context.SaveChanges();

            /*** Add Subscriptions to DB ***/
            var subscriptions = new List<Subscription>
            {
                new Subscription { ClientID=1,      BrokerageID="A1" },
                new Subscription { ClientID=1,      BrokerageID="B1" },
                new Subscription { ClientID=1,      BrokerageID="O1" },
                new Subscription { ClientID=2,      BrokerageID="A1" },
                new Subscription { ClientID=2,      BrokerageID="B1" },
                new Subscription { ClientID=3,      BrokerageID="A1" }
            };

            foreach (Subscription subscription in subscriptions)
            {
                context.Subscriptions.Add(subscription);
            }

            context.SaveChanges();
        }
    }
}
