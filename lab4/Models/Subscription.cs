namespace Lab4.Models
{
    public class Subscription
    {
        public int ClientID { get; set; }
        public string BrokerageID { get; set; }

        // Subsription has a 0:M relationship with Client and Broker. Null = 0 relations
        public Brokerage Broker { get; set; }
        public Client Client { get; set; }
    }
}
