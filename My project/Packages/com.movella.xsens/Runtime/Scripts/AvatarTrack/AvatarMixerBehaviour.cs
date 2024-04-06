using UnityEngine;
using UnityEngine.Playables;

namespace Movella.Xsens
{
    class AvatarMixerBehaviour : PlayableBehaviour
    {
        public Avatar Avatar;
        public Animator Animator;

        public override void ProcessFrame(Playable playable, UnityEngine.Playables.FrameData info, object playerData)
        {
            // Note: There's a bug in nested timelines; playerData doesn't seem to be updated properly and may be pointing at the wrong instance. 
            // Until that bug is fixed, don't use playerData. Use the instance set by CreateTrackMixer in AvatarTrack.

            //if (Animator == null)
            //    Animator = playerData as Animator; 

            if (Animator != null)
            {
                var inputCount = playable.GetInputCount();
                bool hasInput = false; 

                for (int i = 0; i < inputCount; i++)
                {
                    if (playable.GetInputWeight(i) > 0)
                    {
                        hasInput = true;
                        break;
                    }
                }

                if (hasInput)
                {
                    if (Animator.avatar != null)
                    {
                        Avatar = Animator.avatar;
                        Animator.avatar = null;
                    }
                }
                else
                {
                    RestoreAvatar();
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            RestoreAvatar();
        }

        void RestoreAvatar()
        {
            if (Animator != null && Avatar != null)
                Animator.avatar = Avatar;
        }
    }
}