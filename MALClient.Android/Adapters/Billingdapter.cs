// using Android.App;
// using Android.Content;
// using Android.OS;
// using Android.Runtime;
// using Android.Views;
// using Android.Widget;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;
// using Android.BillingClient.Api;
// using Java.Lang;
//
// namespace MALClient.Android.Adapters
// {
//     public class BillingAdapter : Java.Lang.Object, IPurchasesUpdatedListener, IBillingClientStateListener
//     {
//         public BillingClient BillingClient { get; private set; }
//         private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0);
//
//         public async Task Initialize(Activity activity)
//         {
//             BillingClient = BillingClient.NewBuilder(activity).EnablePendingPurchases().SetListener(this).Build();
//             BillingClient.StartConnection(this);
//             await _semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(10));
//         }
//
//         public void OnPurchasesUpdated(BillingResult p0, IList<Purchase> p1)
//         {
//
//         }
//
//         public void OnBillingServiceDisconnected()
//         {
//
//         }
//
//         public void OnBillingSetupFinished(BillingResult p0)
//         {
//             _semaphoreSlim.Release();
//         }
//     }
// }