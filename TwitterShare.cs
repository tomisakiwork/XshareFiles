using System.Collections;
using System.IO;
using UnityEngine;

public class TwitterShare : MonoBehaviour
{
    public void ClickShare()
    {
        StartCoroutine(TakeScreenshotAndShare());
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        Destroy(ss);

        string textToShare = "シェアするテキスト";
        string encodedText = System.Uri.EscapeDataString(textToShare);
#if UNITY_ANDROID
        string shareURL = "AndroidアプリのURL";
        
#elif UNITY_IPHONE
        string shareURL = "iOSアプリのURL";

#endif

        string androidPackageName, androidClassName;
        if (NativeShare.FindTarget(out androidPackageName, out androidClassName, "com.twitter.android", ".*[cC]omposer.*"))
        {
            new NativeShare()
                .AddFile(filePath)
                .SetText(encodedText)
                .SetUrl(shareURL)
                .AddTarget(androidPackageName)
                .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                .Share();
        }
    }
}