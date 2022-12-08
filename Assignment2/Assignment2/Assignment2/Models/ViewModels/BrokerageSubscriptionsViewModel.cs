namespace Assignment2.Models.ViewModels
{
    /**
        ViewModel used for binding the state of the brokerage subscription
        for a given client in the EditSubscriptions.cshtml View. If a client
        is subscribed to the brokerage, IsMember will be set to true, 
        otherwise, it will be set to false.
     */
    public class BrokerageSubscriptionsViewModel
    {
        public string BrokerageId { get; set; }
        public string Title { get; set; }
        public bool IsMember { get; set; }
    }
}
