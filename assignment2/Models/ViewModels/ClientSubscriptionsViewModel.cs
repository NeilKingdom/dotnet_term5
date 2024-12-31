namespace Assignment2.Models.ViewModels
{
    /**
        ViewModel used for binding the client : subscription relationships
        when registering/unregistering client subscriptions.
     */
    public class ClientSubscriptionsViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<BrokerageSubscriptionsViewModel> Subscriptions { get; set; }
    }
}
