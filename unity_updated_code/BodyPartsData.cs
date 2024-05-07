// using UnityEngine;

// public class BodyPartMovement : MonoBehaviour
// {
//     public Transform leftForearm;
//     public Transform rightForearm;
//     // Define other body parts as public Transform fields if needed

//     private Quaternion leftForearmSensorQuaternion;
//     private Quaternion rightForearmSensorQuaternion;
//     // Define sensor quaternion variables for other body parts if needed

//     private bool leftForearmIsOffsetSet = false;
//     private bool rightForearmIsOffsetSet = false;
//     // Define offset set flags for other body parts if needed

//     void OnEnable()
//     {
//         UnityServer.OnLeftForearmSensorDataReceived += HandleSensorDataReceived;
//         UnityServer.OnRightForearmSensorDataReceived += HandleSensorDataReceived;
//         // Subscribe to events for other body parts if needed
//     }

//     void OnDisable()
//     {
//         UnityServer.OnLeftForearmSensorDataReceived -= HandleSensorDataReceived;
//         UnityServer.OnRightForearmSensorDataReceived -= HandleSensorDataReceived;
//         // Unsubscribe from events for other body parts if needed
//     }

//     void Start()
//     {
//         // Initialize sensor quaternion variables to avoid null reference issues
//         leftForearmSensorQuaternion = Quaternion.identity;
//         rightForearmSensorQuaternion = Quaternion.identity;
//         // Initialize sensor quaternions for other body parts if needed
//     }

//     void Update()
//     {
//         // Apply the rotations in the Update method for smoother updates
//         if (leftForearmIsOffsetSet)
//         {
//             leftForearm.localRotation = leftForearmSensorQuaternion;
//         }
        
//         if (rightForearmIsOffsetSet)
//         {
//             rightForearm.localRotation = rightForearmSensorQuaternion;
//         }

//         // Apply rotations for other body parts if needed
//     }

//     private void HandleSensorDataReceived(string receivedBodyPart, Quaternion newSensorQuaternion)
//     {
//         switch (receivedBodyPart.ToLower())
//         {
//             case "leftforearm":
//                 Debug.Log("hello 1");
//                 UpdateBodyPartRotation(ref leftForearmIsOffsetSet, newSensorQuaternion, leftForearm);
//                 break;
//             case "rightforearm":
//                 Debug.Log("hello 2");
//                 UpdateBodyPartRotation(ref rightForearmIsOffsetSet, newSensorQuaternion, rightForearm);
//                 break;
//             // Add cases for other body parts if needed
//         }
//     }

//     private void UpdateBodyPartRotation(ref bool isOffsetSet, Quaternion newSensorQuaternion, Transform bodyPart)
//     {
//         if (!isOffsetSet)
//         {
//             // Assuming initial rotation should be preserved, calculate the offset based on the first sensor reading
//             Quaternion initialRotation = bodyPart.localRotation;
//             Quaternion offsetQuaternion = Quaternion.Inverse(newSensorQuaternion) * initialRotation;
//             newSensorQuaternion = offsetQuaternion * newSensorQuaternion;
//             isOffsetSet = true;
//         }

//         // Directly store the calculated rotation to be applied in Update
//         if(bodyPart == leftForearm)
//         {
//             leftForearmSensorQuaternion = newSensorQuaternion;
//         }
//         else if(bodyPart == rightForearm)
//         {
//             rightForearmSensorQuaternion = newSensorQuaternion;
//         }
//         // Add assignment logic for other body parts if needed
//     }
// }
