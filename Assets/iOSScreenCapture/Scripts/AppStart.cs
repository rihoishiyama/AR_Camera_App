using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppStart : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
#if !UNITY_EDITOR && UNITY_IOS
        UnityiOS.RequestPermissions();
#endif
	}
}
