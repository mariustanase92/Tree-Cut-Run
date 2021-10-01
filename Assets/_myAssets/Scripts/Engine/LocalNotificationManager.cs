using UnityEngine;

public class LocalNotificationManager : MonoBehaviour
{
    private string title = "Come Back!";
    private string subtitle = "New levels available!";
    private string body = "Sooo many trees to cut!";

    void Awake()
    {
#if UNITY_IOS
        var timeTrigger = new Unity.Notifications.iOS.iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(24,0,0),
            Repeats = false
        };

        var notification = new Unity.Notifications.iOS.iOSNotification()
        {
            Identifier = "_notification_01",
            Title = title,
            Body = body,
            Subtitle = subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = (Unity.Notifications.iOS.PresentationOption.Alert | Unity.Notifications.iOS.PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        Unity.Notifications.iOS.iOSNotificationCenter.ScheduleNotification(notification);
#elif UNITY_ANDROID
        var notification = new Unity.Notifications.Android.AndroidNotification();
        notification.Title = title;
        notification.Text = body;
        notification.FireTime = System.DateTime.Now.AddDays(1);

        Unity.Notifications.Android.AndroidNotificationCenter.SendNotification(notification, "channel_id");
#endif
    }

}
