namespace Assignment2.Models.ViewModels
{
    /**
        ViewModel used for binding the brokerage and its associated 
        advertisements to the Index.cshtml page of the Advertisements
        View.
     */
    public class AdsViewModel
    {
        public Brokerage Brokerage { get; set; }
        public IEnumerable<Advertisement> Advertisements { get; set; }
    }
}
