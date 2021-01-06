using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Editor
{
    public class SortedKeyFrames
    {
        List<KeyFrame> keyFrames = new List<KeyFrame>();

        public KeyFrame this[int index]
        {
            get => keyFrames[index];
        }

        public int Count => keyFrames.Count;
        public void Add(KeyFrame value)
        {
            foreach (var kf in keyFrames)
            {
                if (kf.Time == value.Time)
                {
                    Debug.LogError("You can't add same keyframe again");

                    return;
                }
            }

            keyFrames.Add(value);
            keyFrames.Sort();

            // sort on time changed
            value.OnTimeChanged += keyFrames.Sort;
        }

        public int IndexOf(KeyFrame kf) => keyFrames.IndexOf(kf);
        public void RemoveAt(int idx) => keyFrames.RemoveAt(idx);
    }

    public class ScriptObject : MonoBehaviour
    {
        public string Name => name;

        [JsonProperty(PropertyName = "IsCamera")]
        public bool IsCamera { get; set; }

        [JsonProperty(PropertyName = "textureName")]
        public string TextureName { get; set; }

        [JsonProperty(PropertyName = "KeyFrames")]
        public SortedKeyFrames KeyFrames { get; } = new SortedKeyFrames();

        public int SelectKeyFrameIndex { get; set; }
        public KeyFrame SelectedKeyFrame => KeyFrames[SelectKeyFrameIndex];

        private static Dictionary<string, int> texRefCount = new Dictionary<string, int>();
        private static Dictionary<string, Sprite> cachedSprs = new Dictionary<string, Sprite>();
        private int lastKeyframe = 0;

        // for camera
        public Vector3 Origin = Vector3.zero;
        public Vector3 ZeroRotation = Vector3.zero;

        private SpriteRenderer sprRenderer;

        public Vector3 Position
        {
            get => transform.position - Origin;
            set => transform.position = value + Origin;
        }

        public Vector3 Rotation
        {
            get => transform.eulerAngles - ZeroRotation;
            set => transform.eulerAngles = value + ZeroRotation;
        }

        public Vector3 Scale
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }

        public Vector4 Color
        {
            get => sprRenderer == null ? UnityEngine.Color.white : sprRenderer.color;
            set => _ = sprRenderer == null ? UnityEngine.Color.white : sprRenderer.color = value;
        }

        public void Start()
        {
            if (IsCamera)
            {
                Origin = transform.position;
                ZeroRotation = transform.eulerAngles;

                name = "Main Camera";

                return;
            }

            if (!cachedSprs.ContainsKey(TextureName))
            {
                var tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(TextureName));

                var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                cachedSprs.Add(TextureName, spr);
                texRefCount.Add(TextureName, 0);
            }
            else
            {
                texRefCount[TextureName]++;
            }

            name = $"{Path.GetFileNameWithoutExtension(TextureName)} {texRefCount[TextureName]}";

            sprRenderer = gameObject.AddComponent<SpriteRenderer>();
            sprRenderer.sprite = cachedSprs[TextureName];

            var col = gameObject.AddComponent<PolygonCollider2D>();
            col.points = sprRenderer.sprite.vertices;
        }

        public void OnUpdate(float currentTime)
        {
            bool reversed = false;

            if (KeyFrames.Count < lastKeyframe)
                lastKeyframe = 0;

            if (KeyFrames.Count == lastKeyframe)
                return;

            if (KeyFrames[lastKeyframe].Time > currentTime)
                reversed = true;

            int first_keyframe = -1;
            int next_keyframe = -1;

            for (int i = lastKeyframe; reversed ? i >= 0 : i < KeyFrames.Count; _ = reversed ? --i : ++i)
            {
                if (KeyFrames[i].Time == currentTime)
                {
                    first_keyframe = next_keyframe = i;
                    break;
                }

                if (KeyFrames[i].Time < currentTime)
                {
                    first_keyframe = i;

                    if (reversed)
                        break;
                }

                if (KeyFrames[i].Time > currentTime)
                {
                    next_keyframe = i;

                    if (!reversed)
                        break;
                }
            }

            if (next_keyframe == -1 || first_keyframe == next_keyframe)
            {
                var kf = KeyFrames[first_keyframe];
                lastKeyframe = first_keyframe;

                Position = kf.Position;
                Rotation = kf.Rotation;

                if(!IsCamera)
                {
                    Scale = kf.Scale;
                    Color = kf.Color;
                }
            }
            else
            {
                var kf1 = KeyFrames[first_keyframe];
                var kf2 = KeyFrames[next_keyframe];
                lastKeyframe = first_keyframe;

                float progress = (currentTime - kf1.Time) / (kf2.Time - kf1.Time);

                Position = KeyFrame.Interpolate(kf2.InterpolationMode, kf1.Position, kf2.Position, progress);
                Rotation = KeyFrame.Interpolate(kf2.InterpolationMode, kf1.Rotation, kf2.Rotation, progress);

                if (!IsCamera)
                {
                    Scale = KeyFrame.Interpolate(kf2.InterpolationMode, kf1.Scale, kf2.Scale, progress);
                    Color = KeyFrame.Interpolate(kf2.InterpolationMode, kf1.Color, kf2.Color, progress);
                }
            }
        }

        public GameObject GameObject => gameObject;
    }
}
