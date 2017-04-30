using System;
using Android.App;
using Android.Content;
using Android.OS;
using Com.Android.Vending.Billing;
using Object = Java.Lang.Object;

namespace MALClient.Android.Aidl
{
    public class InAppBillingServiceConnection : Object, IServiceConnection
    {
        private readonly Activity _activity;

        public InAppBillingServiceConnection(Activity activity)
        {
            _activity = activity;
        }

        public IInAppBillingService Service { get; private set; }

        public bool Connected { get; private set; }

        public void Connect()
        {
            var serviceIntent = new Intent("com.android.vending.billing.InAppBillingService.BIND");
            serviceIntent.SetPackage("com.android.vending");
            var intentServicesCount = _activity.PackageManager.QueryIntentServices(serviceIntent, 0).Count;
            if (intentServicesCount != 0)
                _activity.BindService(serviceIntent, this, Bind.AutoCreate);
        }

        public void Disconnected()
        {
            _activity.UnbindService(this);
        }

        protected virtual void RaiseOnConnected(bool connected)
        {
            if (!connected)
                return;

            OnConnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void RaiseOnDisconnected()
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;

        #region IServiceConnection implementation

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Service = IInAppBillingServiceStub.AsInterface(service);

            var packageName = _activity.PackageName;

            try
            {
                var response = Service.IsBillingSupported(Billing.APIVersion, packageName, ItemType.InApp);
                if (response != BillingResult.OK)
                    Connected = false;

                // check for v3 subscriptions support
                response = Service.IsBillingSupported(Billing.APIVersion, packageName, ItemType.Subscription);
                if (response == BillingResult.OK)
                {
                    Connected = true;
                    RaiseOnConnected(Connected);
                }
                else
                {
                    Connected = false;
                }
            }
            catch (Exception ex)
            {
                Connected = false;
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Connected = false;
            Service = null;

            RaiseOnDisconnected();
        }

        #endregion
    }
}