using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
    public static class GardenExtensions
    {
        public static Vector3 lookAt( this GameObject myVector, GameObject targetVector )
        {
            return (targetVector.transform.position - myVector.transform.position).normalized;
        }

        public static Vector3 lookAt( this Vector3 myVector, GameObject targetVector )
        {
            return (targetVector.transform.position - myVector).normalized;
        }

        public static Vector3 lookAt( this GameObject myVector, Vector3 targetVector )
        {
            return (targetVector - myVector.transform.position).normalized;
        }

        public static Vector3 lookAt( this Vector3 myVector, Vector3 targetVector )
        {
            return (targetVector - myVector).normalized;
        }

        public static Vector3 Pos( this GameObject gameObject )
        {
            return gameObject.transform.position;
        }

        public static void MoveTo( this Transform self, Transform goal, ref Vector3 posVelocity, ref Vector3 rotVelocity, ref Vector3 scaVelocity, float maxSpeed, ref bool isDone )
        {
            self.localPosition = Vector3.SmoothDamp( self.localPosition, goal.localPosition, ref posVelocity, Time.deltaTime, maxSpeed );
            self.localRotation = QuaternionSmoothDamp( self.localRotation, goal.localRotation, ref rotVelocity, Time.deltaTime, maxSpeed );
            self.localScale = Vector3.SmoothDamp( self.localScale, goal.localScale, ref scaVelocity, Time.deltaTime, maxSpeed );
            bool compare = self.CompareTo( goal );
            if (self.CompareTo( goal )) {
                isDone = !isDone;
            }
        }

        public static void SetGlobalScale( this Transform transform, Vector3 globalScale )
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3( globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z );
        }

        public static int LastIndex<T>( this T[] array )
        {
            return array.Length - 1;
        }

        public static bool CompareTo( this Transform a, Transform b )
        {
            return
                Mathf.Approximately( a.localPosition.x, b.localPosition.x ) &&
                Mathf.Approximately( a.localPosition.y, b.localPosition.y ) &&
                Mathf.Approximately( a.localPosition.z, b.localPosition.z ) &&
                Mathf.Approximately( a.localRotation.eulerAngles.x, b.localRotation.eulerAngles.x ) &&
                Mathf.Approximately( a.localRotation.eulerAngles.y, b.localRotation.eulerAngles.y ) &&
                Mathf.Approximately( a.localRotation.eulerAngles.z, b.localRotation.eulerAngles.z );
        }

        public static Quaternion QuaternionSmoothDamp( Quaternion self, Quaternion goal, ref Vector3 velocity, float smoothTime, float maxSpeed )
        {
            Vector3 eulerAngles = self.eulerAngles;

            eulerAngles.x = Mathf.SmoothDampAngle( eulerAngles.x, goal.eulerAngles.x, ref velocity.x, smoothTime, maxSpeed );
            eulerAngles.y = Mathf.SmoothDampAngle( eulerAngles.y, goal.eulerAngles.y, ref velocity.y, smoothTime, maxSpeed );
            eulerAngles.z = Mathf.SmoothDampAngle( eulerAngles.z, goal.eulerAngles.z, ref velocity.z, smoothTime, maxSpeed );

            return Quaternion.Euler( eulerAngles );
        }

        public static T[] Append<T>( this T[] array, T item )
        {
            if (array == null) {
                return new T[] { item };
            }
            T[] result = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++) {
                result[i] = array[i];
            }
            result[array.Length] = item;
            return result;
        }

        public static void Set( this Text self, string textSting, Color color )
        {
            self.text = textSting;
            self.color = color;
        }

        public static void OrientTowards( this Transform self, Vector3 lookPosition, float orientationSpeed )
        {
            Vector3 lookDirection = Vector3.ProjectOnPlane( lookPosition - self.position, Vector3.up ).normalized;
            if (lookDirection.sqrMagnitude != 0f) {
                Quaternion targetRotation = Quaternion.LookRotation( lookDirection );
                self.rotation = Quaternion.Slerp( self.rotation, targetRotation, Time.deltaTime * orientationSpeed );
            }
        }

        public static Vector3 RandomPointInRadius( Vector3 origin, float distance, int layermask )
        {
            Vector3 randomDirection = Random.insideUnitSphere * distance;
            randomDirection += origin;
            NavMeshHit navHit;
            NavMesh.SamplePosition( randomDirection, out navHit, distance, layermask );
            if (Physics.Linecast( origin, navHit.position )) {
                return origin;
            }
            return navHit.position;
        }

        public static Vector3 GetRandomPointOnMesh( this Mesh mesh )
        {
            //if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
            float[] sizes = GetTriSizes( mesh.triangles, mesh.vertices );
            float[] cumulativeSizes = new float[sizes.Length];
            float total = 0;

            for (int i = 0; i < sizes.Length; i++) {
                total += sizes[i];
                cumulativeSizes[i] = total;
            }

            //so everything above this point wants to be factored out

            float randomsample = Random.value * total;

            int triIndex = -1;

            for (int i = 0; i < sizes.Length; i++) {
                if (randomsample <= cumulativeSizes[i]) {
                    triIndex = i;
                    break;
                }
            }

            if (triIndex == -1) Debug.LogError( "triIndex should never be -1" );

            Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
            Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
            Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

            //generate random barycentric coordinates

            float r = Random.value;
            float s = Random.value;

            if (r + s >= 1) {
                r = 1 - r;
                s = 1 - s;
            }
            //and then turn them back to a Vector3
            Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
            return pointOnMesh;
        }

        private static float[] GetTriSizes( int[] tris, Vector3[] verts )
        {
            int triCount = tris.Length / 3;
            float[] sizes = new float[triCount];
            for (int i = 0; i < triCount; i++) {
                sizes[i] = .5f * Vector3.Cross( verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]] ).magnitude;
            }
            return sizes;
        }

        public static bool VisibleFromCamera( this Camera camera, Renderer renderer )
        {
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes( camera );
            return GeometryUtility.TestPlanesAABB( frustumPlanes, renderer.bounds );
        }
    }
}