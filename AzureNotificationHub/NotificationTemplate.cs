using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureNotificationHub
{
    public class NotificationTemplate
    {
        public string GetPushTemplate(string alert, string badge, Dictionary<string, string> data)
        {
            JObject pushRoot = new JObject();
            JObject pushBody = new JObject();
            List<JProperty> optionalFields = new List<JProperty>();

            if (!String.IsNullOrEmpty(alert))
                pushBody.Add(new JProperty("alert", alert));
            if (!String.IsNullOrEmpty(badge))
                pushBody.Add(new JProperty("badge", badge));
            if (data != null)
            {
                foreach (var item in data)
                {
                    optionalFields.Add(new JProperty(item.Key, item.Value));
                }
                pushBody.Add(optionalFields);
            }

            pushRoot.Add("aps", pushBody);
            return pushRoot.ToString(Formatting.None);
        }
    }
}
