using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;
using Editor;
using System;
using System.Runtime.InteropServices;
using BanGround.Utils;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using RuntimeGizmos;

public partial class CameraOperator : MonoBehaviour
{
    // BECAUSE UNITY AUDIO IS ASS
    public AudioSource ass;
    public TransformGizmo gizmo;
    public Camera cam;

    public bool AutoKeyFrame { get; set; } = false;

    string currentFile = null;

    ScriptObject activeObject;
    List<ScriptObject> scriptObjects = new List<ScriptObject>();

    float currentTime = 0;

    Vector3 savedCameraPos = new Vector3(7.5f, 12.5f, 0);
    Quaternion savedCameraAngle = Quaternion.Euler(45, -45, 0);

    bool camView = true;
    Transform camTransform;

    public const string CAMERA_NAME = "Main Camera";

    public static Vector3 CamOrigin { get; private set; }
    public static Vector3 CamZeroRot { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        var cameraObj = GameObject.Find(CAMERA_NAME);
        camTransform = cameraObj.transform;

        var scriptObject = cameraObj.AddComponent<ScriptObject>();
        scriptObject.IsCamera = true;
        scriptObject.KeyFrames.Add(KeyFrame.Empty());

        scriptObject.Origin = cameraObj.transform.position;
        scriptObject.ZeroRotation = cameraObj.transform.eulerAngles;

        CamOrigin = cameraObj.transform.position;
        CamZeroRot = cameraObj.transform.eulerAngles;

        gizmo.onSelectObject = trsfrm =>
        {
            activeObject = trsfrm.gameObject.GetComponent<ScriptObject>();
        };

        activeObject = scriptObject;
        scriptObjects.Add(scriptObject);
    }

    Dictionary<GameObject, int> last_keyframes = new Dictionary<GameObject, int>();

    // Update is called once per frame
    void Update()
    {
        if(ass.isPlaying)
        {
            currentTime = ass.time;
            UpdateAll();
        }
    }

    void UpdateAll()
    {
        foreach (var scriptObj in scriptObjects)
        {
            scriptObj.OnUpdate(currentTime);
        }
    }

    float sensitivity = 15f;

    // Camera control
    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButton(1))
        {
            float rotationX = camTransform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity;
            float rotationY = camTransform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
            float rotationZ = camTransform.localEulerAngles.z;

            while (rotationX > 180)
                rotationX -= 360;

            rotationX = Mathf.Clamp(rotationX, -90, 90);

            camTransform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);

            bool isFast = false;

            if (Input.GetKey(KeyCode.LeftShift))
                isFast = true;

            var velocity = Vector3.zero;

            const float slowSpeed = 0.02f;
            const float fastSpeed = 0.1f;

            if (Input.GetKey(KeyCode.W))
            {
                velocity.z += isFast ? fastSpeed : slowSpeed;
            }

            if (Input.GetKey(KeyCode.S))
            {
                velocity.z -= isFast ? fastSpeed : slowSpeed;
            }

            if (Input.GetKey(KeyCode.A))
            {
                velocity.x -= isFast ? fastSpeed : slowSpeed;
            }

            if (Input.GetKey(KeyCode.D))
            {
                velocity.x += isFast ? fastSpeed : slowSpeed;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                velocity.y += isFast ? fastSpeed : slowSpeed;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                velocity.y -= isFast ? fastSpeed : slowSpeed;
            }

            if (velocity == Vector3.zero)
                return;

            var rotatedVelocity = camTransform.localRotation * velocity;
            camTransform.localPosition = camTransform.localPosition + rotatedVelocity;
        }
        else
        {
            bool controlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (Input.GetKeyDown(KeyCode.S))
            {
                UpdateOrNewKeyframe();
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                TogglePlaystate();
            }

            if(controlDown && Input.GetKeyDown(KeyCode.O))
            {
                LoadKeyFrames();
            }

            if (controlDown && Input.GetKeyDown(KeyCode.S))
            {
                SaveKeyFrames();
            }

            if (controlDown && Input.GetKeyDown(KeyCode.I))
            {
                StartCoroutine(OpenMusic());
            }

            if (controlDown && Input.GetKeyDown(KeyCode.E))
            {
                ExportLua();
            }
        }
    }

    void UpdateOrNewKeyframe()
    {
        int targetToModify = -1;
        var keyframes = activeObject.KeyFrames;

        for (int i = 0; i < keyframes.Count; i++)
        {
            if (keyframes[i].Time == currentTime)
            {
                targetToModify = i;
                break;
            }
        }

        if (targetToModify != -1)
        {
            activeObject.SelectKeyFrameIndex = targetToModify;

            keyframes[targetToModify].Position = activeObject.Position;
            keyframes[targetToModify].Rotation = activeObject.Rotation;

            if(!activeObject.IsCamera)
            {
                keyframes[targetToModify].Scale = activeObject.Scale;
                keyframes[targetToModify].Color = activeObject.Color;
            }
        }
        else
        {
            var newKey = new KeyFrame
            {
                Time = currentTime,
                Position = activeObject.Position,
                Rotation = activeObject.Rotation
            };

            if (!activeObject.IsCamera)
            {
                newKey.Scale = activeObject.Scale;
                newKey.Color = activeObject.Color;
            }

            keyframes.Add(newKey);
        }
    }

    IEnumerator AfterLoadKeyFrames()
    {
        yield return new WaitForFixedUpdate();
        UpdateAll();
    }

    void LoadKeyFrames()
    {
        var sfd = new SelectFileDialog()
                     .SetFilter("KeyFrame data\0*.json\0")
                     .SetTitle("Select KeyFrame data")
                     .SetDefaultExt("json")
                     .Show();

        if (sfd.IsSucessful)
        {
            foreach(var obj in scriptObjects)
            {
                if(obj.IsCamera)
                    Destroy(obj);
                else
                    Destroy(obj.gameObject);
            }

            currentFile = sfd.File;
            scriptObjects = ScriptObject.FromJson(File.ReadAllText(currentFile));

            currentTime = 0;
            ass.time = 0;

            StartCoroutine(AfterLoadKeyFrames());
        }
    }

    void SaveKeyFrames()
    {
        if (currentFile == null)
        {
            var sfd = new SaveFileDialog()
                         .SetFilter("KeyFrame data\0*.json\0")
                         .SetTitle("Save KeyFrame data")
                         .SetDefaultExt("json")
                         .Show();

            if (sfd.IsSucessful)
            {
                currentFile = sfd.File;
            }
        }

        if (currentFile == null)
            return;

        var json = ScriptObject.ToJson(scriptObjects);
        File.WriteAllText(currentFile, json);
    }

    IEnumerator OpenMusic()
    {
        var sfd = new SelectFileDialog()
                     .SetFilter("Music file\0*.ogg\0")
                     .SetTitle("Select music")
                     .SetDefaultExt("ogg")
                     .Show();

        if(sfd.IsSucessful)
        {
            var type = AudioType.OGGVORBIS;

            using (var uwrm = UnityWebRequestMultimedia.GetAudioClip("file:///" + sfd.File.Replace('\\', '/'), type))
            {
                yield return uwrm.SendWebRequest();

                if (!uwrm.isNetworkError)
                {
                    ass.clip = DownloadHandlerAudioClip.GetContent(uwrm);
                }
            }
        }
    }

    void TogglePlaystate()
    {
        if (ass.isPlaying)
            ass.Pause();
        else
            ass.Play();
    }

    void ExportLua()
    {
        var sfd = new SaveFileDialog()
                     .SetFilter("Lua script\0*.lua\0")
                     .SetTitle("Save script")
                     .SetDefaultExt("lua")
                     .Show();

        if (sfd.IsSucessful)
        {
            var targetFiles = new List<string>();

            var code = LuaCodes.GetLuaCode(scriptObjects);
            File.WriteAllText(sfd.File, code);

            targetFiles.Add(sfd.File);

            var dir = Path.GetDirectoryName(sfd.File);

            var animatorPath = Path.Combine(dir, "animator.lua");
            File.WriteAllText(animatorPath, LuaCodes.AnimatorCode);

            targetFiles.Add(animatorPath);

            foreach (var obj in scriptObjects)
            {
                if (obj.IsCamera)
                    continue;

                var texPath = obj.TextureName.Replace(Path.GetDirectoryName(obj.TextureName), dir);
                File.Copy(obj.TextureName, texPath, true);

                targetFiles.Add(texPath);
            }

            Win32.OpenFolderAndSelectFiles(dir, targetFiles.ToArray());
        }
    }

    void ToggleCameraView()
    {
        if(camView)
        {
            savedCameraPos = transform.position;
            savedCameraAngle = transform.rotation;

            camTransform = GameObject.Find(CAMERA_NAME).transform;

            transform.parent = camTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            cam.cullingMask &= ~(1 << 8);
            gizmo.selectionMask &= ~(1 << 8);
        }
        else
        {
            transform.parent = GameObject.Find("Cameras").transform;

            camTransform = transform;

            transform.position = savedCameraPos;
            transform.rotation = savedCameraAngle;

            cam.cullingMask |= (1 << 8);
            gizmo.selectionMask |= (1 << 8);
        }
    }
}
