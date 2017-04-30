namespace MALClient.Android.Aidl
{
    public static class BillingResult
    { 
        /// <summary>
        ///     Success
        /// </summary>
        /// <value>Success - 0</value>
        public const int OK = 0;

        /// <summary>
        ///     User pressed back or canceled a dialog
        /// </summary>
        /// <value>The user cancelled - 1</value>
        public const int UserCancelled = 1;

        /// <summary>
        ///     this billing API version is not supported for the type requested
        /// </summary>
        /// <value>The billing unavailable - 3</value>
        public const int BillingUnavailable = 3;

        /// <summary>
        ///     Requested SKU is not available for purchase
        /// </summary>
        /// <value>The item unavailable - 4</value>
        public const int ItemUnavailable = 4;

        /// <summary>
        ///     Invalid arguments provided to the API
        /// </summary>
        /// <value>The developer error - 5</value>
        public const int DeveloperError = 5;

        /// <summary>
        ///     Fatal error during the API action
        /// </summary>
        /// <value>The error - 6</value>
        public const int Error = 6;

        /// <summary>
        ///     Failure to purchase since item is already owned
        /// </summary>
        /// <value>The item already owned - 7</value>
        public const int ItemAlreadyOwned = 7;

        /// <summary>
        ///     Failure to consume since item is not owned
        /// </summary>
        /// <value>The item not owned - 8</value>
        public const int ItemNotOwned = 8;
    }

    /// <summary>
    ///     In app billing item types.
    /// </summary>
    public static class ItemType
    {
        /// <summary>
        ///     Gets the in app Item type
        /// </summary>
        /// <value>inapp</value>
        public const string InApp = "inapp";

        /// <summary>
        ///     Gets the subscription type
        /// </summary>
        /// <value>subs</value>
        public const string Subscription = "subs";
    }

    public static class Billing
    {

        public const int APIVersion = 3;
               
        public const string SkuDetailsList = "DETAILS_LIST";
               
        public const string ItemIdList = "ITEM_ID_LIST";
    }

    public static class Response
    {
        public const string Code = "RESPONSE_CODE";
               
        public const string BuyIntent = "BUY_INTENT";
               
        public const string InAppPurchaseData = "INAPP_PURCHASE_DATA";
               
        public const string InAppDataSignature = "INAPP_DATA_SIGNATURE";
               
        public const string InAppDataSignatureList = "INAPP_DATA_SIGNATURE_LIST";
               
        public const string InAppPurchaseItemList = "INAPP_PURCHASE_ITEM_LIST";
               
        public const string InAppPurchaseDataList = "INAPP_PURCHASE_DATA_LIST";
               
        public const string InAppContinuationToken = "INAPP_CONTINUATION_TOKEN";
    }
}