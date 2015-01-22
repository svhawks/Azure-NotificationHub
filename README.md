# Azure-NotificationHub

Azure Notification Hub Helper

##Usage
```var instance = AzureNotificationHub.Instance("endpoint", "hubName");```

```NotificationTemplate nt = new NotificationTemplate();```

```var dictionary = new Dictionary<string, string>();```

```dictionary.Add("key", "value");```

```var data = nt.GetPushTemplate("Test", "1", dictionary);```

#####For single user
```var response = await instance.Send(data, "tag");```

#####For broadcast
```var response = await instance.SendBroadcast(data);```
