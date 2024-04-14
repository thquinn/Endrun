using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public static class Util
    {
        public static string KeyCodeToString(KeyCode keyCode) {
            if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9) {
                return ((char)('0' + keyCode - KeyCode.Alpha0)).ToString();
            }
            return keyCode.ToString();
        }

        static Camera mainCamera;
        public static Collider GetMouseCollider(LayerMask layerMask) {
            if (mainCamera == null) mainCamera = Camera.main;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                return null;
            }
            return hit.collider;
        }
        public static Vector3 GetMouseCollisionPoint(LayerMask layerMask) {
            if (mainCamera == null) mainCamera = Camera.main;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                return Vector3.zero;
            }
            return hit.point;
        }
    }
}
