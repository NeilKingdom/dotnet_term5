namespace Assignment2.Models.ViewModels
{
    /**
        ViewModel used for dynamic error messages
     */
    public class ErrorViewModel
    {
        public ErrorViewModel(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; }
    }
}
