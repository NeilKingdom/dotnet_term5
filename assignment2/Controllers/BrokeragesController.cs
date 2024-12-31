using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;

namespace Assignment2.Controllers
{
    public class BrokeragesController : Controller
    {
        private readonly MarketDbContext _context;

        /**
            Default constructor
         */
        public BrokeragesController(MarketDbContext context)
        {
            _context = context;
        }

        /**
            Displayed as the default page for the brokerages
            page, but also used when the select link is pressed.
            If the select link is pressed, the View is updated 
            to display brokerage subscribers.
         */
        public async Task<IActionResult> Index(string? id)
        {
            BrokerageViewModel brokerageViewModel = new()
            {
                // SELECT FROM table brokerages and JOIN subscriptions. ORDER BY title
                Brokerages = await _context.Brokerages
                    .Include(i => i.Subscriptions)
                    .OrderBy(i => i.Title)
                    .ToListAsync()
            };

            if (id != null)
            {
                var client = await _context.Clients.ToListAsync();
                var subscriptions = await _context.Subscriptions
                    .Where(b => b.BrokerageId == id)
                    .ToListAsync();

                // FROM s IN subscription JOIN c in client ON s.ClientID == c.ClientId
                // SELECT new Client{ FirstName = c.FirstName, LastName = c.LastName};
                var brokerClients = subscriptions.Join(client, s => s.ClientId, c => c.ClientId,
                    (s, c) => new Client { FirstName = c.FirstName, LastName = c.LastName });

                brokerageViewModel.Clients = brokerClients;
                brokerageViewModel.Subscriptions = subscriptions;
            };

            return View(brokerageViewModel);
        }

        /**
            Returns a list of details about the client or 
            a 404 if no clients with ClientId matching the
            ID in the URL are found.
         */
        public async Task<IActionResult> Details(string id)
        {
            var brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null) return NotFound();
            return View(brokerage);
        }

        /**
            Returns the Brokerage Create.cshtml page to
            allow creation of a new brokerage.
         */
        public IActionResult Create() => View();

        /**
            Creates a new brokerage when the Create
            button is pressed and the form data is 
            sent via HTTP POST.
         */
        [HttpPost]
        public async Task<IActionResult> Create(Brokerage brokerage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(brokerage);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException) // Unique to brokerages since we manually enter the ID
                {
                    return View("Error", new ErrorViewModel($"Brokerage with key {brokerage.BrokerageId} already exists."));
                }
            }

            return View();
        }

        /**
            Return the Edit.cshtml View for Brokerages if 
            at least one brokerage with BrokerageId == id
            are found. Return 404 otherwise.
         */
        public async Task<IActionResult> Edit(string id)
        {
            Brokerage brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null) return NotFound();
            return View(brokerage);
        }

        /**
            Persist edits from the form to a specific brokerage
            and save those changes in the database. View() is 
            returned if no such brokerage exists and .NET handles
            the form field errors.
         */
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Brokerage brokerage)
        {
            if (ModelState.IsValid && _context.Brokerages.Any(b => b.BrokerageId == id))
            {
                brokerage.BrokerageId = id;
                _context.Attach(brokerage).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        /**
            Returns the Delete.cshtml page for Brokerages if
            a brokerage with BrokerageId matching id in the URL
            was found, otherwise, returns 404.
         */
        public async Task<ActionResult> Delete(string id)
        {
            Brokerage brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null) return NotFound();
            return View(brokerage);
        }

        /**
            Delete a brokerage from the data store.
            Returns a 404 if the brokerage with 
            brokerageId matching id in the URL is 
            not found.
         */
        [HttpPost]
        public async Task<IActionResult> Delete(string id, BrokerageViewModel brokerageVM)
        {
            var brokerage = await _context.Brokerages.FindAsync(id);
            if (brokerage == null) return NotFound();

            if (!ModelState.IsValid)
                return View("Error", new ErrorViewModel("Something went wrong deleting the brokerage."));

            var brokerAds = await _context.Advertisements
                .Where(b => b.BrokerageId == id)
                .ToListAsync();

            // Brokerage cannot be deleted if it contains advertisements
            if(brokerAds.Any()) 
                return View("Error", new ErrorViewModel("Broker has advertisements. Cannot be deleted."));

            // Remove the client from DB
            _context.Brokerages.Remove(brokerage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
