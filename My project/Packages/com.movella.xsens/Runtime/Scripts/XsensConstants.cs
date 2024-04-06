using System.Collections.Generic;
using UnityEngine;

namespace Movella.Xsens
{
    static class XsensConstants
    {
        public static int MvnDefaultSegmentCount = 67;
        public static int MvnBodySegmentCount = 23;
        public static int MvnFingerSegmentCount = 40;
        public static int MvnPropSegmentCount = 4;

        public static int MaxCharacters = 4;
        public static int NumStreams = 5;

        public static Dictionary<XsBodyAnimationSegment, HumanBodyBones> BodyMecAnimBones = new()
        {
            { XsBodyAnimationSegment.Pelvis, HumanBodyBones.Hips },
            { XsBodyAnimationSegment.LeftUpperLeg, HumanBodyBones.LeftUpperLeg },
            { XsBodyAnimationSegment.LeftLowerLeg, HumanBodyBones.LeftLowerLeg },
            { XsBodyAnimationSegment.LeftFoot, HumanBodyBones.LeftFoot },
            { XsBodyAnimationSegment.LeftToe, HumanBodyBones.LeftToes },
            { XsBodyAnimationSegment.RightUpperLeg, HumanBodyBones.RightUpperLeg },
            { XsBodyAnimationSegment.RightLowerLeg, HumanBodyBones.RightLowerLeg },
            { XsBodyAnimationSegment.RightFoot, HumanBodyBones.RightFoot },
            { XsBodyAnimationSegment.RightToe, HumanBodyBones.RightToes },
            { XsBodyAnimationSegment.L5, HumanBodyBones.LastBone }, //not used
            { XsBodyAnimationSegment.L3, HumanBodyBones.Spine },
            { XsBodyAnimationSegment.T12, HumanBodyBones.LastBone },    //not used
            { XsBodyAnimationSegment.T8, HumanBodyBones.Chest },
            { XsBodyAnimationSegment.LeftShoulder, HumanBodyBones.LeftShoulder },
            { XsBodyAnimationSegment.LeftUpperArm, HumanBodyBones.LeftUpperArm },
            { XsBodyAnimationSegment.LeftLowerArm, HumanBodyBones.LeftLowerArm },
            { XsBodyAnimationSegment.LeftHand, HumanBodyBones.LeftHand },
            { XsBodyAnimationSegment.RightShoulder, HumanBodyBones.RightShoulder },
            { XsBodyAnimationSegment.RightUpperArm, HumanBodyBones.RightUpperArm },
            { XsBodyAnimationSegment.RightLowerArm, HumanBodyBones.RightLowerArm },
            { XsBodyAnimationSegment.RightHand, HumanBodyBones.RightHand },
            { XsBodyAnimationSegment.Neck, HumanBodyBones.Neck },
            { XsBodyAnimationSegment.Head, HumanBodyBones.Head }
        };

        public static Dictionary<XsFingerAnimationSegment, HumanBodyBones> FingerMecAnimBones = new()
        {
            { XsFingerAnimationSegment.LeftCarpus, HumanBodyBones.LeftHand },

            { XsFingerAnimationSegment.LeftFirstMetacarpal, HumanBodyBones.LeftThumbProximal },
            { XsFingerAnimationSegment.LeftFirstProximalPhalange, HumanBodyBones.LeftThumbIntermediate },
            { XsFingerAnimationSegment.LeftFirstDistalPhalange, HumanBodyBones.LeftThumbDistal },

            { XsFingerAnimationSegment.LeftSecondMetacarpal, HumanBodyBones.LastBone }, //not used
            { XsFingerAnimationSegment.LeftSecondProximalPhalange, HumanBodyBones.LeftIndexProximal },
            { XsFingerAnimationSegment.LeftSecondMiddlePhalange, HumanBodyBones.LeftIndexIntermediate },
            { XsFingerAnimationSegment.LeftSecondDistalPhalange, HumanBodyBones.LeftIndexDistal },

            { XsFingerAnimationSegment.LeftThirdMetacarpal, HumanBodyBones.LastBone }, //not used
            { XsFingerAnimationSegment.LeftThirdProximalPhalange, HumanBodyBones.LeftMiddleProximal },
            { XsFingerAnimationSegment.LeftThirdMiddlePhalange, HumanBodyBones.LeftMiddleIntermediate },
            { XsFingerAnimationSegment.LeftThirdDistalPhalange, HumanBodyBones.LeftMiddleDistal },

            { XsFingerAnimationSegment.LeftFourthMetacarpal, HumanBodyBones.LastBone }, //not used
            { XsFingerAnimationSegment.LeftFourthProximalPhalange, HumanBodyBones.LeftRingProximal },
            { XsFingerAnimationSegment.LeftFourthMiddlePhalange, HumanBodyBones.LeftRingIntermediate },
            { XsFingerAnimationSegment.LeftFourthDistalPhalange, HumanBodyBones.LeftRingDistal },

            { XsFingerAnimationSegment.LeftFifthMetacarpal, HumanBodyBones.LastBone }, //not used
            { XsFingerAnimationSegment.LeftFifthProximalPhalange, HumanBodyBones.LeftLittleProximal },
            { XsFingerAnimationSegment.LeftFifthMiddlePhalange, HumanBodyBones.LeftLittleIntermediate },
            { XsFingerAnimationSegment.LeftFifthDistalPhalange, HumanBodyBones.LeftLittleDistal },

            { XsFingerAnimationSegment.RightCarpus, HumanBodyBones.RightHand },

            { XsFingerAnimationSegment.RightFirstMetacarpal, HumanBodyBones.RightThumbProximal },
            { XsFingerAnimationSegment.RightFirstProximalPhalange, HumanBodyBones.RightThumbIntermediate },
            { XsFingerAnimationSegment.RightFirstDistalPhalange, HumanBodyBones.RightThumbDistal },

            { XsFingerAnimationSegment.RightSecondMetacarpal, HumanBodyBones.LastBone }, //not used
            { XsFingerAnimationSegment.RightSecondProximalPhalange, HumanBodyBones.RightIndexProximal },
            { XsFingerAnimationSegment.RightSecondMiddlePhalange, HumanBodyBones.RightIndexIntermediate },
            { XsFingerAnimationSegment.RightSecondDistalPhalange, HumanBodyBones.RightIndexDistal },

            { XsFingerAnimationSegment.RightThirdMetacarpal, HumanBodyBones.LastBone }, //not used
            { XsFingerAnimationSegment.RightThirdProximalPhalange, HumanBodyBones.RightMiddleProximal },
            { XsFingerAnimationSegment.RightThirdMiddlePhalange, HumanBodyBones.RightMiddleIntermediate },
            { XsFingerAnimationSegment.RightThirdDistalPhalange, HumanBodyBones.RightMiddleDistal },

            { XsFingerAnimationSegment.RightFourthMetacarpal, HumanBodyBones.LastBone }, //not used
            { XsFingerAnimationSegment.RightFourthProximalPhalange, HumanBodyBones.RightRingProximal },
            { XsFingerAnimationSegment.RightFourthMiddlePhalange, HumanBodyBones.RightRingIntermediate },
            { XsFingerAnimationSegment.RightFourthDistalPhalange, HumanBodyBones.RightRingDistal },

            { XsFingerAnimationSegment.RightFifthMetacarpal, HumanBodyBones.LastBone }, //not used
            { XsFingerAnimationSegment.RightFifthProximalPhalange, HumanBodyBones.RightLittleProximal },
            { XsFingerAnimationSegment.RightFifthMiddlePhalange, HumanBodyBones.RightLittleIntermediate },
            { XsFingerAnimationSegment.RightFifthDistalPhalange, HumanBodyBones.RightLittleDistal }
        };
    }
}
