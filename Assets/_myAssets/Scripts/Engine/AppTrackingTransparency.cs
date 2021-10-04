using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

public class AppTrackingTransparency : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_IOS

        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
        ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }

}
