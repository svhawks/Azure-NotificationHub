using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus.Notifications;

namespace AzureNotificationHub
{
    public class AzureNotificationHub
    {
        private static AzureNotificationHub _instance;
        private readonly NotificationHubClient _hub;

        public static AzureNotificationHub Instance(string endPoint, string hubName)
        {
            return _instance ?? (_instance = new AzureNotificationHub(endPoint, hubName));
        }


        public AzureNotificationHub(string endPoint, string hubName)
        {
            _hub = NotificationHubClient.CreateClientFromConnectionString(endPoint, hubName);
        }

        public async Task<NotificationOutcome> Send(string data, string tag)
        {
            return await _hub.SendAppleNativeNotificationAsync(data, tag);
        }

        public async Task<NotificationOutcome> SendBroadcast(string data)
        {
            return await _hub.SendAppleNativeNotificationAsync(data);
        }

        public async Task DeleteRegistrationByDeviceId(string handle)
        {
            await _hub.DeleteRegistrationsByChannelAsync(handle);
        }

        public async Task<string> GetRegistrationId()
        {
            return await _hub.CreateRegistrationIdAsync();
        }

        public async Task CreateOrUpdateRegistration(string id, DeviceRegistration deviceRegistration)
        {
            RegistrationDescription registration;
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
                await _hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                ReturnGoneIfHubResponseIsGone(e);
            }
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex != null && webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse) webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }
    }
}