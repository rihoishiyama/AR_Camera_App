using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class UnityAndroid : MonoBehaviour
{

    //ここから
    [SerializeField] private GameObject canvas;
    [SerializeField] private Text testText;
    private string imageName = "";

	public void ScreenCapture_Android()
    {
        canvas.SetActive(false);
        StartCoroutine("Captcha");
    }

    IEnumerator Captcha() {

        //ファイル名 ※カスタムDateTimeじゃないとファイル名に/と:が入って保存失敗する
        imageName = System.DateTime.Now.ToString ("gyyyyMMddtthhmmssfff") + ".png";

        //ios,Android時パスを追加
#if !UNITY_EDITOR && UNITY_ANDROID && !UNITY_IOS
        Scan(imageName);
        //testText.text = imageName;
#endif
        yield return new WaitForEndOfFrame();
        //スクリーンショット
        ScreenCapture.CaptureScreenshot(imageName);
        canvas.SetActive(true);

#if !UNITY_EDITOR && UNITY_ANDROID && !UNITY_IOS
        //↑モバイルではうまく動かないようなのでFileStreamで保存する
        var tex = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
        using (FileStream BinaryFile = new FileStream (imageName, FileMode.Create, FileAccess.Write)) 
        {
            using (BinaryWriter Writer = new BinaryWriter (BinaryFile)) 
            {
                Writer.Write (tex.EncodeToPNG ());
            }
        }

#endif
        //待つ
        yield return new WaitForEndOfFrame ();
        //保存まで待機
        float latency = 0, latencyLimit=2;
        while (latency < latencyLimit) {
            //ファイルが存在していればループ終了
            if (System.IO.File.Exists (imageName)) {
                break;
            }
            latency += Time.deltaTime;
            yield return null;
        }

        ScanMedia (imageName);

        //待機時間を超えた
        if (latency >= latencyLimit) {
            testText.text = "ファイル保存失敗";
        }

    }

    void ScanMedia(string fileName)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        //メディアスキャン
#if !UNITY_EDITOR && UNITY_ANDROID && !UNITY_IOS
        using (AndroidJavaClass jcUnityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject joActivity = jcUnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity"))
        using (AndroidJavaObject joContext = joActivity.Call<AndroidJavaObject> ("getApplicationContext"))
        using (AndroidJavaObject jcMediaScannerConnection = new AndroidJavaClass ("android.media.MediaScannerConnection"))
        using (AndroidJavaClass jcEnvironment = new AndroidJavaClass ("android.os.Environment"))
        using (AndroidJavaObject joExDir = jcEnvironment.CallStatic<AndroidJavaObject> ("getExternalStorageDirectory")) 
        {
            jcMediaScannerConnection.CallStatic ("scanFile", joContext, new string[] { fileName }, new string[] { "image/png" }, null);
        }
        Handheld.StopActivityIndicator();
#endif
    }

    void Scan(string fileName)
    {
#if !UNITY_EDITOR && UNITY_ANDROID && !UNITY_IOS
        using (AndroidJavaClass jcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject joActivity = jcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject joContext = joActivity.Call<AndroidJavaObject>("getApplicationContext"))
        using (AndroidJavaObject jcMediaScannerConnection = new AndroidJavaClass("android.media.MediaScannerConnection"))
        using (AndroidJavaClass jcEnvironment = new AndroidJavaClass("android.os.Environment"))
        using (AndroidJavaObject joExDir = jcEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
        string imageNameDirectory = joExDir.Call<string>("toString") + "/DCIM/Unipro/";
            if (!Directory.Exists(imageNameDirectory))
            {
                testText.text = "search path2 : " + imageNameDirectory;
                System.IO.Directory.CreateDirectory(imageNameDirectory);
                testText.text = "search path3 : " + imageNameDirectory;
                return;
            }
            imageName = joExDir.Call<string>("toString") + "/DCIM/Unipro/" + fileName;
            //testText.text = "search path : " + imageName;
        }
#endif   
    }



}