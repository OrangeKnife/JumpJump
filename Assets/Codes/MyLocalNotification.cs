﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class MyLocalNotification
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";
    private static string unityClass = "com.unity3d.player.UnityPlayerNativeActivity";
#endif
    
    public static void SendNotification(int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", id, unityClass, delay * 1000L, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b);
        }
#endif
#if UNITY_IOS 
		UnityEngine.iOS.LocalNotification iOSlocalNotification = new UnityEngine.iOS.LocalNotification();
		iOSlocalNotification.fireDate = System.DateTime.Now.AddSeconds(delay);
		iOSlocalNotification.alertBody = message;
		iOSlocalNotification.applicationIconBadgeNumber = 1;
		iOSlocalNotification.hasAction = true;
	

		iOSlocalNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(iOSlocalNotification);

#endif


    }

    public static void SendRepeatingNotification(int id, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetRepeatingNotification", id, unityClass, delay * 1000L, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b);
        }
#endif
	
#if UNITY_IOS  

		UnityEngine.iOS.LocalNotification iOSlocalNotification = new UnityEngine.iOS.LocalNotification();
		iOSlocalNotification.fireDate = System.DateTime.Now.AddSeconds(delay);	//first push here
		iOSlocalNotification.alertBody = message;
		iOSlocalNotification.applicationIconBadgeNumber = 1;
		iOSlocalNotification.hasAction = true;

		iOSlocalNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.GregorianCalendar;
		iOSlocalNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;//hack here, do daily notification

		iOSlocalNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(iOSlocalNotification);
#endif


    }

    public static void CancelNotification(int id)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("CancelNotification", id);
        }
#endif

#if UNITY_IOS && !UNITY_EDITOR	
		UnityEngine.iOS.LocalNotification l = new UnityEngine.iOS.LocalNotification (); 
		l.applicationIconBadgeNumber = -1; 
		UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow (l);
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications (); 
		UnityEngine.iOS.NotificationServices.ClearLocalNotifications (); 
#endif
    }
}
