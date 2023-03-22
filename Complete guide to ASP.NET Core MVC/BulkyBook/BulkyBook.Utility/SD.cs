namespace BulkyBook.Utility
{
    public static class  SD
    {
        // I do wonder why all of these couldnt be enums
        // But that is a phylosophical question
        // I mean I guess the SD is technically used as an Enum :D
        // TODO make these as enums

        public const string Role_User_Indi = "Individual";
        public const string Role_User_Comp = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string StatussPending = "Pending";
        public const string StatussApproved = "Approved";
        public const string StatussInProcess = "Processing";
        public const string StatussShipped = "Shipped";
        public const string StatussCancelled = "Cancelled";
        public const string Statuss_name = "Refunded";

        public const string PaymentStatussPending = "Pending";
        public const string PaymentStatussApproved = "Approved";
        public const string PaymentStatussDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatussRejected = "rejected";
    }
}