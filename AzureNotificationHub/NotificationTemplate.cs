using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureNotificationHub
{
    public class NotificationTemplate
    {
        public string GetPushTemplate(string alert, string badge, Dictionary<string, string> data)
        {
            var pushRoot = new JObject();
            var pushBody = new JObject();
            var optionalFields = new List<JProperty>();

            if (!String.IsNullOrEmpty(alert))
                pushBody.Add(new JProperty("alert", alert));
            if (!String.IsNullOrEmpty(badge))
                pushBody.Add(new JProperty("badge", badge));

            pushRoot.Add("aps", pushBody);
            if (data == null) return pushRoot.ToString(Formatting.None);
            optionalFields.AddRange(data.Select(item => new JProperty(item.Key, item.Value)));
            pushRoot.Add(optionalFields);
            return pushRoot.ToString(Formatting.None);
        }
    }
}
