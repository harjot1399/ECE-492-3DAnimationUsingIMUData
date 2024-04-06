using System;
using Unity.LiveCapture;
using UnityEngine;

namespace Movella.Xsens
{
    /// <summary>
    /// Contains the segment numbers for the body animation
    /// </summary>
    public enum XsBodyAnimationSegment
    {
        Pelvis = 0,

        L5 = 1,//not used
        L3 = 2,//spine
        T12 = 3,//not used
        T8 = 4,//chest

        Neck = 5,
        Head = 6,

        RightShoulder = 7,
        RightUpperArm = 8,
        RightLowerArm = 9,
        RightHand = 10,

        LeftShoulder = 11,
        LeftUpperArm = 12,
        LeftLowerArm = 13,
        LeftHand = 14,

        RightUpperLeg = 15,
        RightLowerLeg = 16,
        RightFoot = 17,
        RightToe = 18,

        LeftUpperLeg = 19,
        LeftLowerLeg = 20,
        LeftFoot = 21,
        LeftToe = 22
    }

    /// <summary>
    /// Contains the segment numbers for the finger animation
    /// </summary>
    public enum XsFingerAnimationSegment
    {
        LeftCarpus = 0,

        LeftFirstMetacarpal = 1,
        LeftFirstProximalPhalange = 2,
        LeftFirstDistalPhalange = 3,

        LeftSecondMetacarpal = 4, //not used
        LeftSecondProximalPhalange = 5,
        LeftSecondMiddlePhalange = 6,
        LeftSecondDistalPhalange = 7,

        LeftThirdMetacarpal = 8, //not used
        LeftThirdProximalPhalange = 9,
        LeftThirdMiddlePhalange = 10,
        LeftThirdDistalPhalange = 11,

        LeftFourthMetacarpal = 12, //not used
        LeftFourthProximalPhalange = 13,
        LeftFourthMiddlePhalange = 14,
        LeftFourthDistalPhalange = 15,

        LeftFifthMetacarpal = 16, //not used
        LeftFifthProximalPhalange = 17,
        LeftFifthMiddlePhalange = 18,
        LeftFifthDistalPhalange = 19,

        RightCarpus = 20,

        RightFirstMetacarpal = 21,
        RightFirstProximalPhalange = 22,
        RightFirstDistalPhalange = 23,

        RightSecondMetacarpal = 24, //not used
        RightSecondProximalPhalange = 25,
        RightSecondMiddlePhalange = 26,
        RightSecondDistalPhalange = 27,

        RightThirdMetacarpal = 28, //not used
        RightThirdProximalPhalange = 29,
        RightThirdMiddlePhalange = 30,
        RightThirdDistalPhalange = 31,

        RightFourthMetacarpal = 32, //not used
        RightFourthProximalPhalange = 33,
        RightFourthMiddlePhalange = 34,
        RightFourthDistalPhalange = 35,

        RightFifthMetacarpal = 36, //not used
        RightFifthProximalPhalange = 37,
        RightFifthMiddlePhalange = 38,
        RightFifthDistalPhalange = 39
    }

    enum JointFlags
    {
        None = 0,
        Position = 1 << 0,
        Rotation = 1 << 1,
        Scale = 1 << 2,
        All = ~0
    }

    [Serializable]
    struct FrameData
    {
        public Timecode TC;
        public FrameRate FrameRate;
        public int SegmentCount;
        public Vector3[] Positions;
        public Quaternion[] Orientations;
        public int NumProps;
    }
}