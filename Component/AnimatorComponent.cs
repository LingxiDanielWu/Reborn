using System.Collections.Generic;
using Cysharp.Text;
using EC.Manager;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace EC.Component
{
    public class AnimatorComponent : EComponent
    {
        private AnimationPlayer animPlayer;
        
        public AnimatorComponent(): base(ComponentType.Animator)
        {
        }

        public override void Attach(Entity e)
        {
            base.Attach(e);
            animPlayer = new AnimationPlayer(Parent);
        }

        public override void Tick(float deltaTime, params object[] paras)
        {
            animPlayer.Tick(deltaTime);
        }

        public override void LateTick(float deltaTime, params object[] paras)
        {
            animPlayer.LateTick(deltaTime);
        }

        public void Play(string key, float fadeInTime = 0.25f)
        {
            string clipName = ZString.Format("{0}-{1}", Parent.GetWeaponTypeName(false), key);
            animPlayer.Play(clipName, fadeInTime);
        }

        public void Stop(string key)
        {
            
        }

        public override void Init()
        {
            string bundleType = Utils.GetEnumName(typeof(EntityType), Parent.EType);
            string subBundleType = Parent.GetWeaponTypeName();
            animPlayer.Init($"{bundleType}/{subBundleType}");
        }

        public override void Dispose()
        {
            animPlayer.Dispose();
            animPlayer = null;
        }
    }
    
    public class ECPlayableInfo
    {
        public int Id;
        public int OccupiedIndexInMixer;
        public AnimationClipPlayable Playable;
        public float Weight;
        public float FadeInDuration;
        public string Name;
        public float Length;
    }

    public class AnimationPlayer
    {
        private PlayableGraph playableGraph = new PlayableGraph();
        private AnimationMixerPlayable mixerPb = new AnimationMixerPlayable();
        private Dictionary<string, AnimationClipPlayable> configPbs = new Dictionary<string, AnimationClipPlayable>();
        private Dictionary<int, ECPlayableInfo> playingPbs = new Dictionary<int, ECPlayableInfo>();
        private int lastPlayableId;
        private int curPlayableId;
        private Entity parent;
        private float timeSincePlay;
        private bool isCurPbDone;

        public AnimationPlayer(Entity entt)
        {
            parent = entt;
            playableGraph = PlayableGraph.Create();
        }

        public void Init(string animBundleName)
        {
            var weaponBundle = AssetBundle.LoadFromFile($"Assets/AssetBundles/animation/{animBundleName}");
            var clipNames = weaponBundle.GetAllAssetNames();
            mixerPb = AnimationMixerPlayable.Create(playableGraph, clipNames.Length);
            var output = AnimationPlayableOutput.Create(playableGraph, "Animation", parent.GetComponent<Animator>());
            output.SetSourcePlayable(mixerPb);
            playableGraph.Play();

            for (int i = 0; i < clipNames.Length; i++)
            {
                var clipAsset = weaponBundle.LoadAsset<AnimationClip>(clipNames[i]);
                var animPb = AnimationClipPlayable.Create(playableGraph, clipAsset);
                configPbs.Add(clipAsset.name, animPb);
            }
            
            GameEventManager.Instance.RegisterListener(EventType.AnimOver, parent);
        }

        public void Play(string clipName, float fadeInDuration = 0.25f)
        {
            // Debug.Log($"pppppppppppppp {clipName}");
            var animator = parent.GetComponent<Animator>();
            if (!animator.enabled)
            {
                animator.enabled = true;
            }

            ECPlayableInfo nextPbInfo = null;
            foreach (var pair in playingPbs)
            {
                if (pair.Value.Name == clipName)
                {
                    nextPbInfo = pair.Value;
                    if (nextPbInfo.Id == curPlayableId)
                    {
                        return;
                    }
                    break;
                }
            }

            if (nextPbInfo == null)
            {
                if (configPbs.TryGetValue(clipName, out AnimationClipPlayable pb))
                {
                    nextPbInfo = new ECPlayableInfo
                    {
                        Id = pb.GetHashCode(),
                        FadeInDuration = fadeInDuration,
                        Weight = 0,
                        Name = pb.GetAnimationClip().name,
                        OccupiedIndexInMixer = playingPbs.Count,
                        Playable = pb,
                        Length = pb.GetAnimationClip().length
                    };
                    playableGraph.Connect(pb, 0, mixerPb, playingPbs.Count);
                    playingPbs.Add(nextPbInfo.Id, nextPbInfo);
                }
            }

            if (nextPbInfo == null)
            {
                Debug.LogError($"Animation Clip {clipName} not exist.");
                return;
            }

            timeSincePlay = 0;
            isCurPbDone = false;
            
            //todo:临时混合方案，后续可优化为按队列中的pb优先级混合
            if (!IsInTransition() || nextPbInfo.Id != lastPlayableId)
            {
                nextPbInfo.FadeInDuration = fadeInDuration;
                nextPbInfo.Weight = 0;
                lastPlayableId = curPlayableId;
                curPlayableId = nextPbInfo.Id;
                nextPbInfo.Playable.SetTime(0f);
            }
            else
            {
                // 反复横跳的情况就保持现状做混合
                if (nextPbInfo.Id == lastPlayableId)
                {
                    nextPbInfo.FadeInDuration = fadeInDuration;
                    lastPlayableId = curPlayableId;
                    curPlayableId = nextPbInfo.Id;
                }
            }
        }

        public void Tick(float deltaTime)
        {
            if (playingPbs.Count == 0)
                return;

            timeSincePlay += Time.deltaTime;
            var curPlayingInfo = playingPbs[curPlayableId];
            if (playingPbs.Count == 1)
            {
                curPlayingInfo.Weight = 1f;
                mixerPb.SetInputWeight(curPlayingInfo.OccupiedIndexInMixer, curPlayingInfo.Weight);
            }
            else
            {
                float dt = Time.deltaTime;
                float curFramePassDurationPct = curPlayingInfo.FadeInDuration == 0f ? 1 : dt / curPlayingInfo.FadeInDuration;

                foreach (var pair in playingPbs)
                {
                    if (curPlayableId != 0 && pair.Value.Id == curPlayableId)
                    {
                        var curPb = playingPbs[curPlayableId];
                        curPb.Weight = Mathf.Clamp01(curPb.Weight + curFramePassDurationPct);
                        mixerPb.SetInputWeight(curPb.OccupiedIndexInMixer, curPb.Weight);
                    }
                    else
                    {
                        var otherPb = playingPbs[pair.Value.Id];
                        otherPb.Weight = Mathf.Clamp01(otherPb.Weight - curFramePassDurationPct);
                        mixerPb.SetInputWeight(otherPb.OccupiedIndexInMixer, otherPb.Weight);
                    }
                }
            }
        }

        public void LateTick(float deltaTime)
        {
            if (curPlayableId == 0 || isCurPbDone)
                return;

            var curPbInfo = playingPbs[curPlayableId];
            if (timeSincePlay >= curPbInfo.Length)
            {
                if (!curPbInfo.Playable.GetAnimationClip().isLooping)
                {
                    isCurPbDone = true;
                    SendAnimOver(curPbInfo.Name);
                }
            }
        }

        private bool IsInTransition()
        {
            if (!(lastPlayableId != 0 && curPlayableId != 0))
            {
                return false;
            }

            return playingPbs[lastPlayableId].Weight > 0f && playingPbs[curPlayableId].Weight < 1f;
        }

        // public void InitAnimEvent()
        // {
        //     foreach (var pair in configPbs)
        //     {
        //         var clip = pair.Value.GetAnimationClip();
        //         if (clip.isLooping)
        //         {
        //             continue;
        //         }
        //         
        //         AnimationEvent e = new AnimationEvent();
        //         e.stringParameter = clip.name;
        //         e.functionName = "AnimOverEvent";
        //         e.time = clip.length;
        //         clip.AddEvent(e);
        //     }
        //     GameEventManager.Instance.RegisterListener(EventType.AnimOver, parent);
        // }
        
        public void SendAnimOver(string clipName)
        {
            // Debug.LogWarning($"overrrr  {clipName} ");
            GameEventManager.Instance.PublishToEntity(EventType.AnimOver, parent, clipName);
        }

        public void Dispose()
        {
            playableGraph.Destroy();
        }
    }
}
