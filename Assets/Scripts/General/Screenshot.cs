using System;
using System.IO;
using UnityEngine;

public class Screenshot : PersistentSingleton<Screenshot>
{
    private const int MaxSuperRes = 8;
    [SerializeField] [Range(1, MaxSuperRes)] private int superSize = 1;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) == true)
        {
            // create screenshot folder if it doesn't exists
            string folderpath = Application.dataPath + "/../Screenshots";
            if (Directory.Exists(folderpath) == false)
            {
                Directory.CreateDirectory(folderpath);
            }

            var effectiveSuperSize = Mathf.Clamp(superSize, 1, MaxSuperRes);
            // save screenshot with date, time and resolution
            string resolution = Screen.width * effectiveSuperSize + "x" + Screen.height * effectiveSuperSize + "_";
            string dateAndTime = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = folderpath + "/screen_" + dateAndTime + resolution + ".png";
            ScreenCapture.CaptureScreenshot(filename, effectiveSuperSize);
            Debug.Log("Screenshot saved to: " + filename);
        }
    }
#endif
}