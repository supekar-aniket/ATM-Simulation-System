namespace ATM_Simulation_System.Helper
{
    public class MaskCardHelper
    {
        public static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return "XXXX XXXX XXXX XXXX";

            var last4 = cardNumber.Substring(cardNumber.Length - 4);
            return $"XXXX XXXX XXXX {last4}";
        }
    }
}
