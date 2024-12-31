using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;

namespace Assignment2.Controllers
{
    public class ClientsController : Controller
    {
        private readonly MarketDbContext _context;

        /**
            Default constructor
         */
        public ClientsController(MarketDbContext context)
        {
            _context = context;
        }

        /**
            Binds a brokerage ViewModel to the Client Index.cshtml page.
            This can be used to detect if the brokerage contains any
            relationships with clients through a subscription.
         */
        public async Task<IActionResult> Index(int? id)
        {
            BrokerageViewModel brokerageVM = new()
            {
                // SELECT table Subscriptions JOINed with Broker ORDERED by client's last name
                Clients = await _context.Clients
                    .Include(i => i.Subscriptions)
                    .ThenInclude(b => b.Broker)
                    .OrderBy(c => c.LastName)
                    .ToListAsync(),

                // SELECT table Broker WHERE client ID == id in URL
                Subscriptions = await _context.Subscriptions
                    .Include(s => s.Broker)
                    .Where(c => c.ClientId == id)
                    .ToListAsync()
            };

            return View(brokerageVM);
        }

        /**
            If the client with ClientId == id is found, return View
            with the appropriate model bound, otherwise, return 404.
         */
        public async Task<IActionResult> Details(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        /**
            Display the Client Create.cshtml page when the
            Create New button is pressed.
         */
        public IActionResult Create() => View();

        /**
            Creates and stores a new client in the DB upon form
            submission in Client Create.cshtml page. If model is
            bound correctly, it is added to the DB, otherwise, we
            display an error.
         */
        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        /**
            Displays the Client Edit.cshtml page with client
            who's clientId matches id in the URL.
         */
        public async Task<IActionResult> Edit(int id)
        {
            Client client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            return View(client); 
        }

        /**
            Edit a client and update their properties in the
            database. If id is not found, returns a 404.
         */
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (ModelState.IsValid && _context.Clients.Any(c => c.ClientId == id))
            {
                client.ClientId = id;
                _context.Attach(client).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        /**
            Returns the Delete.cshtml page for Clients if
            a client with ClientId matching id in the URL
            was found, otherwise, returns 404.
         */
        public async Task<ActionResult> Delete(int id)
        {
            Client client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        /**
            Removes a client from the DB. Returns a 404
            if the client with matching clientId to the 
            requested id does not exist.
         */
        [HttpPost]
        public async Task<IActionResult> Delete(int id, BrokerageViewModel brokerageVM)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            if (!ModelState.IsValid)
                return View("Error", new ErrorViewModel("Something went wrong deleting the client."));

            // Remove the client from DB
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /**
            Register or unregister client subscriptions to
            brokerages.
         */
        public async Task<IActionResult> EditSubscriptions(int id)
        {
            ClientSubscriptionsViewModel clientSubscriptions = new();
            List<BrokerageSubscriptionsViewModel> brokerSubsriptions = new();

            clientSubscriptions.Client = await _context.Clients.FindAsync(id);
            if (clientSubscriptions.Client == null) return NotFound();

            // For each brokerage, add a new BrokerageSubscriptionViewModel to brokerSubscriptions
            await _context.Brokerages.ForEachAsync(b => brokerSubsriptions.Add(new() {
                BrokerageId = b.BrokerageId,
                Title = b.Title,
                IsMember = false 
            }));

            // SELECT from table Subscription client with ClientId == id
            // Foreach client, SELECT brokerage with brokerageId == b.BrokerageId
            // Assert that there is only one such brokerage and set its IsMember property to true
            await _context.Subscriptions
                .Where(c => c.ClientId == id)
                .ForEachAsync(b => brokerSubsriptions
                .Where(c => c.BrokerageId == b.BrokerageId)
                .Single().IsMember = true);

            // Order client subscriptions by brokerage Title
            clientSubscriptions.Subscriptions = brokerSubsriptions
                .OrderBy(b => b.Title)
                .OrderBy(m=>m.IsMember==true);

            return View(clientSubscriptions);
        }

        /**
            Register a brokerage to a client by linking them 
            through a subscription.
         */
        public async Task<IActionResult> AddSubscription(int clientId, string brokerageId)
        {
            var subscription = await _context.Subscriptions.FindAsync(clientId, brokerageId);
            if (subscription == null)
            {
                Subscription newSubscription = new()
                {
                    BrokerageId = brokerageId,
                    ClientId = clientId
                };

                // Save subscription to DB
                _context.Subscriptions.Add(newSubscription);
                await _context.SaveChangesAsync();
            }
            else // Should not reach
            { 
                return View("Error",
                    new ErrorViewModel($"Subscription between client {clientId} and brokerage {brokerageId} already exists!"));
            }

            return Redirect($"/Clients/EditSubscriptions/{clientId}");
        }

        /**
            Remove a subscription if it exists between a client
            and brokerage.
         */
        public async Task<IActionResult> RemoveSubscription(int clientId, string brokerageId)
        {
            var subscription = await _context.Subscriptions.FindAsync(clientId, brokerageId);
            if (subscription == null) // Should not reach
            {
                return View("Error",
                    new ErrorViewModel($"The subscription between client {clientId} and brokerage {brokerageId} does not exist in the database..."));
            }

            // Remove subscription from DB
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return Redirect($"/Clients/EditSubscriptions/{clientId}");
        }
    }
}
