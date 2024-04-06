using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Movella.Xsens
{
    [Serializable]
    class AvatarPlayableAsset : PlayableAsset
    {
        public ExposedReference<Avatar> Avatar; 

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AvatarBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

            behaviour.avatar = Avatar.Resolve(graph.GetResolver());

            return playable;
        }
    }
}