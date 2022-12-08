namespace Assignment2.Models
{
    public class Subscription
    {
        // Composite keys
        public int ClientId { get; set; }
        public string BrokerageId { get; set; }

        // Navigation props
        public virtual Brokerage Broker { get; set; }
        public virtual Client Client { get; set; }
    }
}
