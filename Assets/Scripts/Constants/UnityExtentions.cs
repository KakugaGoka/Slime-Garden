using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;



namespace UnityEngine {
    public static class GardenExtensions {

        public static void MoveTo(this Transform self, Transform goal, ref Vector3 posVelocity, ref Vector3 rotVelocity, ref Vector3 scaVelocity, float maxSpeed, ref bool isDone) {
            self.localPosition = Vector3.SmoothDamp(self.localPosition, goal.localPosition, ref posVelocity, Time.deltaTime, maxSpeed);
            self.localRotation = QuaternionSmoothDamp(self.localRotation, goal.localRotation, ref rotVelocity, Time.deltaTime, maxSpeed);
            self.localScale = Vector3.SmoothDamp(self.localScale, goal.localScale, ref scaVelocity, Time.deltaTime, maxSpeed);
            bool compare = self.CompareTo(goal);
            if (self.CompareTo(goal)) {
                isDone = !isDone;
            }
        }

        public static bool CompareTo(this Transform a, Transform b) {
            return
                Mathf.Approximately(a.localPosition.x, b.localPosition.x) &&
                Mathf.Approximately(a.localPosition.y, b.localPosition.y) &&
                Mathf.Approximately(a.localPosition.z, b.localPosition.z) &&
                Mathf.Approximately(a.localRotation.eulerAngles.x, b.localRotation.eulerAngles.x) &&
                Mathf.Approximately(a.localRotation.eulerAngles.y, b.localRotation.eulerAngles.y) &&
                Mathf.Approximately(a.localRotation.eulerAngles.z, b.localRotation.eulerAngles.z);
        }

        public static Quaternion QuaternionSmoothDamp(Quaternion self, Quaternion goal, ref Vector3 velocity, float smoothTime, float maxSpeed) {
            Vector3 eulerAngles = self.eulerAngles;

            eulerAngles.x = Mathf.SmoothDampAngle(eulerAngles.x, goal.eulerAngles.x, ref velocity.x, smoothTime, maxSpeed);
            eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, goal.eulerAngles.y, ref velocity.y, smoothTime, maxSpeed);
            eulerAngles.z = Mathf.SmoothDampAngle(eulerAngles.z, goal.eulerAngles.z, ref velocity.z, smoothTime, maxSpeed);

            return Quaternion.Euler(eulerAngles);
        }

        public static T[] Append<T>(this T[] array, T item) {
            if (array == null) {
                return new T[] { item };
            }
            T[] result = new T[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = item;
            return result;
        }

        public static void Set(this Text self, string textSting, Color color) {
            self.text = textSting;
            self.color = color;
        }

        public static void OrientTowards(this Transform self, Vector3 lookPosition, float orientationSpeed) {
            Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - self.position, Vector3.up).normalized;
            if (lookDirection.sqrMagnitude != 0f) {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                self.rotation = Quaternion.Slerp(self.rotation, targetRotation, Time.deltaTime * orientationSpeed);
            }
        }

        public static Vector3 RandomPointInRadius(Vector3 origin, float distance, int layermask) {
            Vector3 randomDirection = Random.insideUnitSphere * distance;
            randomDirection += origin;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
            if (Physics.Linecast(origin, navHit.position)) {
                return origin;
            }
            return navHit.position;
        }
    }
}
