using Assets.Code.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets.Code
{
    public static class Util
    {
        public static float NormalizedSin(float f) {
            return Mathf.Sin(f) * .5f + .5f;
        }

        public static List<T> Shuffle<T>(this List<T> list) {
            int n = list.Count;
            for (int i = 0; i < n; i++) {
                int r = i + Random.Range(0, n - i);
                T t = list[r];
                list[r] = list[i];
                list[i] = t;
            }
            return list;
        }
        public static T ChooseRandom<T>(IList<T> list) {
            return list[Random.Range(0, list.Count)];
        }

        public static string KeyCodeToString(KeyCode keyCode) {
            if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9) {
                return ((char)('0' + keyCode - KeyCode.Alpha0)).ToString();
            }
            return keyCode.ToString();
        }
        public static string ToRoman(int number) {
            // from Mosè Bottacini on StackOverflow
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException(nameof(number), "insert value between 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new Exception("Impossible state reached");
        }

        public static Color GetUnitUIColor(Unit unit) {
            if (unit.isSummoner) return Color.white;
            if (unit.playerControlled) return Constants.COLOR_ALLY;
            return Constants.COLOR_ENEMY;
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

        public static Vector3 GetChunksCenter(Scene scene) {
            int chunksLayer = LayerMask.NameToLayer("Chunks");
            Bounds bounds = new Bounds();
            foreach (GameObject go in scene.GetRootGameObjects()) {
                if (go.layer == chunksLayer) {
                    foreach (Collider collider in go.GetComponentsInChildren<Collider>()) {
                        Bounds chunkBounds = collider.bounds;
                        if (bounds.size.x == 0) {
                            bounds = chunkBounds;
                        }
                        else {
                            bounds.Encapsulate(chunkBounds.min);
                            bounds.Encapsulate(chunkBounds.max);
                        }
                    }
                    
                }
            }
            return bounds.center;
        }
    }
}
