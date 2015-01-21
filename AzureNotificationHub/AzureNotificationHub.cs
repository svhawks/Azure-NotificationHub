using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AzureNotificationHub
{
    public class AzureNotificationHub
    {
        private static AzureNotificationHub instance;
        private NotificationHubClient hub;
        public static AzureNotificationHub Instance(string endPoint, string hubName)
        {
            if (instance == null)
                instance = new AzureNotificationHub(endPoint, hubName);
            return instance;
        }

        
        public AzureNotificationHub(string endPoint, string hubName)
        {
            hub = NotificationHubClient.CreateClientFromConnectionString(endPoint, hubName);
        }

        public async Task<NotificationOutcome> Send(string data, string tag)
        {
            return await hub.SendAppleNativeNotificationAsync(data, tag);
        }

        public async Task<NotificationOutcome> SendBroadcast(string data)
        {
            return await hub.SendAppleNativeNotificationAsync(data);
        }

        public async Task DeleteRegistrationByDeviceId(string Handle)
        {
            await hub.DeleteRegistrationsByChannelAsync(Handle);
        }

        public async Task<string> GetRegistrationId()
        {
            return await hub.CreateRegistrationIdAsync();
        }

        public async Task CreateOrUpdateRegistration(string id, DeviceRegistration deviceRegistration)
        {
            RegistrationDescription registration = null;
            switch (deviceRegistration.Platform)
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceRegistration.Handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceRegistration.Handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceRegistration.Handle);
                    break;
                case "gcm":
                    registration = new GcmRegistrationDescription(deviceRegistration.Handle);
                    break;
                default:
                    throw new ArgumentException("Platform not supported.");
            }

            registration.RegistrationId = id;
            registration.Tags = new HashSet<string>(deviceRegistration.Tags);

            try
            {
                await hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                ReturnGoneIfHubResponseIsGone(e);
            }
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse)webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }

    }
}
