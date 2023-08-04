using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Android;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Director : MonoBehaviour
{
    #region V
    public TextAsset RBB_0001;
    public RectTransform content_recttransform;
    public GameObject eachText;
    public string testTxt;
    public string[] sentences;
    Transform contentTF;
    Text[] preparedTextArray = new Text[100];
    #endregion
    void Start()
    {
        CheckAndroidPermissionAndDo(MyStart);
    }

    private void MyStart()
    {
#if UNITY_EDITOR
        Co_printEverySec = StartCoroutine(IE_PrintEverySec());
#endif
        ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
        contentTF = content_recttransform.transform;

        sentences = RBB_0001.text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        if (sentences.Length > preparedTextArray.Length)
        {
            preparedTextArray = new Text[sentences.Length];
        }

        for (int i = 0; i < preparedTextArray.Length; i++)
        {
            GameObject obj = Instantiate(eachText, contentTF);
#if UNITY_EDITOR
            obj.name = "Text_" + i.ToString();
#endif
            preparedTextArray[i] = obj.GetComponent<Text>();
        }

        for (int i = 0; i < sentences.Length; i++)
        {
            preparedTextArray[i].text = sentences[i];

        }

        StartCoroutine(IE_AfterFrameStart(waitFrame: 1));
    }

    void Update()
    {
        if (content_recttransform.localPosition.y < 0f)
        {
            content_recttransform.localPosition = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            
        }

    }
#if UNITY_EDITOR
    private void OnDisable()
    {
        StopCoroutine(Co_printEverySec);
    }

    Coroutine Co_printEverySec;
    IEnumerator IE_PrintEverySec()
    {
        while (true)
        {
            //print("world " + content_recttransform.position.y);
            //print("local " + content_recttransform.localPosition.y);
            yield return new WaitForSeconds(1f);
        }

    }

#endif

    IEnumerator IE_AfterFrameStart(int waitFrame)
    {
        if (waitFrame <= 1)
        {
            waitFrame = 1;
        }
        int countFrame = 0;
        while (true)
        {
            ++countFrame;
            yield return null;

            if (countFrame == waitFrame)
            {
                contentTF.GetComponent<VerticalLayoutGroup>().enabled = true;
                yield break;
            }
        }


    }
    public static void WriteString()
    {
        string path = Application.persistentDataPath + "/test.txt";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        writer.Close();
        StreamReader reader = new StreamReader(path);
        //Print the text from the file
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
    public static void ReadString()
    {
        
        string path = Application.persistentDataPath + "/test.txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

#if UNITY_ANDROID
    /// <summary> 안드로이드 - 권한 확인하고, 승인시 동작 수행하기 </summary>
    private void CheckAndroidPermissionAndDo(Action actionIfPermissionGranted)
    {
        // 권한 승인이 안된 상태
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) == false)
        {
            // 권한 요청 응답에 따른 동작 콜백
            PermissionCallbacks pCallbacks = new PermissionCallbacks();
            pCallbacks.PermissionGranted += str => Debug.Log($"{str} 승인");
            pCallbacks.PermissionGranted += _ => actionIfPermissionGranted(); // 승인 시 기능 실행

            pCallbacks.PermissionDenied += str => Debug.Log($"{str} 거절");

            pCallbacks.PermissionDeniedAndDontAskAgain += str => Debug.Log($"{str} 격하게 거절(다시 보기 싫음)");

            // 권한 요청
            Permission.RequestUserPermission(Permission.ExternalStorageRead, pCallbacks);
        }
        // 권한이 승인 되어 있는 경우
        else
        {
            actionIfPermissionGranted(); // 바로 기능 실행
        }
    }
#endif
}
