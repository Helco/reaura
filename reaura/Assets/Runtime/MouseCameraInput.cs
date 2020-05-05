using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aura;
#if UNITY_EDITOR
using UnityEditor;

namespace Aura
{
    [CustomEditor(typeof(MouseCameraInput))]
    public class MouseCameraInputEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!EditorApplication.isPlaying)
                return;

            EditorGUILayout.Space();
            GUILayout.Label("Runtime", EditorStyles.boldLabel);
            bool prevEnabled = GUI.enabled;
            GUI.enabled = false;

            var scaledPos = (target as MouseCameraInput).AuraPosition;
            EditorGUILayout.Vector2Field("AuraPosition", scaledPos);

            GUI.enabled = prevEnabled;
        }
    }

#endif

    public class MouseCameraInput : MonoBehaviour
    {
        public float maxPitch = 1210.0f / 1440.0f * 90.0f;
        public float horizontalSpeed = 1.0f;
        public float verticalSpeed = 1.0f;

        public Vector2 AuraPosition => AuraMath.AngleToAura(euler);

        private Vector3 euler = Vector3.zero;

        void Start()
        {
            // cap framerate because f*** unity
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 240;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                var clickAuraPos = AuraMath.SphereToAura(ray.direction);
                Debug.Log("Clicked on aura: " + clickAuraPos);
            }

            bool isMoving = Input.GetMouseButton(1);
            Cursor.lockState = isMoving ? CursorLockMode.Locked : CursorLockMode.None;
            if (!isMoving)
                return;

            euler.x = Mathf.Clamp(euler.x + Input.GetAxis("Mouse Y") * verticalSpeed * Time.deltaTime, -90.0f, maxPitch);
            euler.y += Input.GetAxis("Mouse X") * horizontalSpeed * Time.deltaTime;
            while (euler.y < 0)
                euler.y += 360.0f;
            while (euler.y >= 360.0f)
                euler.y -= 360.0f;
            transform.localRotation = Quaternion.Euler(euler);
        }
    }
}
