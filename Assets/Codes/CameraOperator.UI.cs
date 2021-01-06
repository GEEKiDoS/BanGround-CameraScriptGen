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

public partial class CameraOperator : MonoBehaviour
{
    void DrawObjectList()
    {
        ImGui.SetNextWindowPos(new Vector2(10, 30), ImGuiCond.Once, new Vector2(0.0f, 0.0f));
        ImGui.SetNextWindowSize(new Vector2(150, Screen.height - 125), ImGuiCond.Once);
        ImGui.Begin("Objects", ImGuiWindowFlags.NoSavedSettings);
        {
            if(ImGui.Checkbox("Camera View", ref camView))
            {
                ToggleCameraView();
            }

            ImGui.BeginChild("ObjectList", new Vector2(0, -ImGui.GetFrameHeightWithSpacing() * 2), true);
            {
                foreach (var obj in scriptObjects)
                {
                    if (ImGui.Selectable(obj.name, activeObject == obj))
                    {
                        activeObject = obj;
                    }
                }
            }
            ImGui.EndChild();

            if(ImGui.Button("+"))
            {
                var sfd = new SelectFileDialog()
                    .SetFilter("Image file\0*.jpg;*.png\0")
                    .SetTitle("Select image")
                    .SetDefaultExt("png")
                    .Show();

                if(sfd.IsSucessful)
                {
                    var gameObJ = new GameObject();
                    var scriptObj = gameObJ.AddComponent<ScriptObject>();
                    scriptObj.TextureName = sfd.File;
                    
                    var newKeyframe = KeyFrame.Empty();
                    newKeyframe.Position = transform.position + transform.rotation * new Vector3(0, 0, 5);
                    newKeyframe.Rotation = transform.eulerAngles;

                    scriptObj.KeyFrames.Add(newKeyframe);
                    scriptObj.Position = newKeyframe.Position;
                    scriptObj.Rotation = newKeyframe.Rotation;

                    scriptObjects.Add(scriptObj);
                }
            }
        }
        ImGui.End();
    }

    void DrawMenu()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open keyframe data", "Ctrl+O"))
                {
                    LoadKeyFrames();
                }

                if (ImGui.MenuItem("Save keyframe data", "Ctrl+S"))
                {
                    SaveKeyFrames();
                }

                if (ImGui.MenuItem("Import Music", "Ctrl+I"))
                {
                    StartCoroutine(OpenMusic());
                }

                if (ImGui.MenuItem("Export BanGround Lua code", "Ctrl+E"))
                {
                    ExportLua();
                }

                if (ImGui.MenuItem("Quit"))
                {
                    Application.Quit(0);
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Playback"))
            {
                if (ImGui.MenuItem("Play/Pause", "Space"))
                {
                    TogglePlaystate();
                }

                if (ImGui.MenuItem("Reset time to 0"))
                {
                    ass.time = 0.00f;
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Keyframe"))
            {
                if (ImGui.MenuItem("Add keyframe/Edit current keyframe", "S"))
                {
                    UpdateOrNewKeyframe();
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

    void DrawKeyframesEditor()
    {
        ImGui.SetNextWindowPos(new Vector2(Screen.width - 10, 30), ImGuiCond.Once, new Vector2(1.0f, 0.0f));
        ImGui.SetNextWindowSize(new Vector2(350, Screen.height - 125), ImGuiCond.Once);
        ImGui.Begin("KeyFrames", ImGuiWindowFlags.NoSavedSettings);
        {
            ImGui.BeginGroup();
            ImGui.BeginChild("Left", new Vector2(100, -ImGui.GetFrameHeightWithSpacing()), true);
            {
                for (int i = 0; i < activeObject.KeyFrames.Count; i++)
                {
                    var kf = activeObject.KeyFrames[i];
                    if (ImGui.Selectable($"{i}: {kf.Time:0.00}s", activeObject.SelectKeyFrameIndex == i))
                    {
                        activeObject.SelectKeyFrameIndex = i;
                    }
                }

            }
            ImGui.EndChild();

            if (ImGui.Button("+"))
            {
                UpdateOrNewKeyframe();
            }

            if (activeObject.KeyFrames.Count > 0)
            {
                ImGui.SameLine();
                if (ImGui.Button("-"))
                {
                    activeObject.KeyFrames.RemoveAt(activeObject.SelectKeyFrameIndex);
                }
            }

            ImGui.EndGroup();
            ImGui.SameLine();

            if (activeObject.KeyFrames.Count > 0 && activeObject.SelectedKeyFrame != null)
            {
                var kf = activeObject.SelectedKeyFrame;

                ImGui.BeginGroup();
                ImGui.BeginChild("Keyframe prop", new Vector2(0, 0), true);

                float step = 0.1f;
                float step_fast = 0.5f;

                if (ImGuiExt.InputScalarFloat("Time", ImGuiDataType.Float, ref kf._time, ref step, ref step_fast, "%.2f", ImGuiInputTextFlags.None))
                {
                    // to raise the event then sort the keyframes
                    kf.Time = kf._time;
                    activeObject.SelectKeyFrameIndex = activeObject.KeyFrames.IndexOf(kf);
                }

                bool modified = false;

                if (ImGui.DragFloat3("Position", ref kf.Position))
                    modified = true;

                if (ImGui.DragFloat3("Rotation", ref kf.Rotation))
                    modified = true;

                if(!activeObject.IsCamera)
                {
                    if (ImGui.DragFloat3("Scale", ref kf.Scale))
                        modified = true;

                    if (ImGui.ColorPicker4("Color", ref kf.Color))
                        modified = true;
                }

                ImGui.Spacing();
                ImGui.Spacing();

                ImGui.Combo("Mode", ref kf.InterpolationMode, Easings.interpolationModes, Easings.interpolationModes.Length);

                if (ImGui.Button("Jump to"))
                {
                    ass.time = kf.Time;
                    currentTime = kf.Time;

                    modified = true;
                }

                if (currentTime != kf.Time)
                {
                    if (ImGui.Button("Duplicate"))
                    {
                        var newKey = new KeyFrame
                        {
                            Time = currentTime,
                            Position = kf.Position,
                            Rotation = kf.Rotation
                        };

                        if (!activeObject.IsCamera)
                        {
                            newKey.Scale = kf.Scale;
                            newKey.Color = kf.Color;
                        }

                        activeObject.KeyFrames.Add(newKey);
                        activeObject.SelectKeyFrameIndex = activeObject.KeyFrames.IndexOf(newKey);
                    }
                }

                if (modified)
                {
                    UpdateAll();
                }

                ImGui.EndChild();
                ImGui.EndGroup();
            }
        }
        ImGui.End();
    }

    void DrawPlaybackControl()
    {
        ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoMove;
        Vector2 window_pos = new Vector2(Screen.width / 2, Screen.height - 10.0f);
        Vector2 window_pos_pivot = new Vector2(0.5f, 1.0f);
        ImGui.SetNextWindowPos(window_pos, ImGuiCond.Always, window_pos_pivot);

        ImGui.SetNextWindowBgAlpha(0.35f);

        ImGui.SetNextWindowSize(new Vector2(Screen.width / 2, 0));
        ImGui.Begin("Playback Control", window_flags);
        {
            var len = 0.0f;
            
            if (ass.clip != null)
                len = ass.clip.length;

            ImGuiExt.TextCenter($"{currentTime:0.00}/{len:0.00}");
            if (ImGuiExt.ButtonCenter(ass.isPlaying ? "ll" : " > "))
            {
                TogglePlaystate();
            }

            ImGui.SetNextItemWidth(Screen.width / 2);
            if (ImGui.SliderFloat("", ref currentTime, 0, len))
            {
                ass.time = currentTime;
            }
        }
        ImGui.End();
    }

    void OnLayout()
    {
        DrawMenu();
        DrawObjectList();
        DrawKeyframesEditor();
        DrawPlaybackControl();
    }

    void OnEnable()
    {
        ImGuiUn.Layout += OnLayout;
    }

    void OnDisable()
    {
        ImGuiUn.Layout -= OnLayout;
    }
}