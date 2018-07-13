using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnityiOSScreenCapture : MonoBehaviour {

    public CanvasGroup canvasGroup;

	public void Execute()
    {
#if !UNITY_EDITOR
		PHAuthorizationStatus phstatus = (PHAuthorizationStatus)Enum.ToObject(
			typeof(PHAuthorizationStatus), UnityiOS.HasCameraRollPermission());
		UnityiOS.PlaySystemShutterSound();
		if(phstatus == PHAuthorizationStatus.Authorized) {
			StartCoroutine(_CaptureScreenShot());
		} else {

		}
#endif
    }

    private IEnumerator _CaptureScreenShot()
    {
        canvasGroup.alpha = 0; //みたいな処理を入れておくと撮影時にUIを外すといった事が出来ます
        yield return new WaitForEndOfFrame();

        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        byte[] screenshot = tex.EncodeToPNG();

        UnityiOS.SaveTexture(screenshot, screenshot.Length);

        canvasGroup.alpha = 1;
    }
}
