using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Movella.Xsens
{
    [Serializable]
    [TrackClipType(typeof(AvatarPlayableAsset))]
    [TrackBindingType(typeof(Animator))]
    [ExcludeFromPreset]
    class AvatarTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = go.GetComponent<PlayableDirector>(); 
            var playable = ScriptPlayable<AvatarMixerBehaviour>.Create(graph, inputCount);
            var mixer = playable.GetBehaviour();

            var clip = GetClips().FirstOrDefault(); 

            if (clip != null)
            {
                var asset = clip.asset as AvatarPlayableAsset;
                var avatar = asset.Avatar.Resolve(director);
                mixer.Avatar = avatar;
                mixer.Animator = director.GetGenericBinding(this) as Animator;
            }

            return playable; 
        }
    }
}
