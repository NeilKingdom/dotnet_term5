namespace Assignment2.Models.ViewModels
{
    /**
        ViewModel used for binding brokerages in the brokerages
        Razor pages.
     */
    public class BrokerageViewModel 
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Brokerage> Brokerages { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
