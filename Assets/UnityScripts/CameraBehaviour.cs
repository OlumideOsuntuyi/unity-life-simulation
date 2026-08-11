using UnityEngine;

namespace External
{
    [ExecuteInEditMode]
    public class CameraBehaviour : Singleton<CameraBehaviour>
    {
        [SerializeField] private Settings settings;
        [SerializeField] private Transform target, lookTarget;
        [SerializeField] private float minZoom, maxZoom;
        private Settings baseSettings;
        private Transform baseTarget;

        public Camera Camera
        {
            get; private set;
        }
        private void Awake()
        {
            if (Application.isPlaying)
            {
                baseSettings = settings;
                baseTarget = target;
            }
            Camera = GetComponent<Camera>();
        }
        public void OnReturn()
        {
            baseSettings = settings;
            settings.distance = 300;
            settings.smoothness = 60;
        }

        private void Update()
        {
            if (target && lookTarget)
            {
                Set(settings);
                //Zoom();
                Quaternion rotation = transform.rotation;
                transform.LookAt(lookTarget);
                transform.rotation = Quaternion.Lerp(rotation, transform.rotation, Time.deltaTime * settings.smoothness);
            }
        }

        [SerializeField] private float previousZoomDistance, deltaDistance;
        [SerializeField] private float zoomThreshold;
        void Zoom()
        {
            if (Input.touchCount == 2)
            {
                Touch one = Input.GetTouch(0);
                Touch two = Input.GetTouch(1);

                float distance;
                // previous distance will be preset before finger begins moving
                if (one.phase == two.phase && one.phase is TouchPhase.Moved)
                {
                    distance = Vector2.Distance(one.position, two.position);
                    deltaDistance = distance - previousZoomDistance;
                    float zoom = (deltaDistance) / Mathf.Clamp(zoomThreshold, 0.01f, 500f);
                    settings.distance = Mathf.Clamp(settings.distance + zoom, baseSettings.distance - minZoom, baseSettings.distance + maxZoom);
                }
                else // reset zoom distance on finger stationary 
                if (one.phase is TouchPhase.Stationary or TouchPhase.Began || two.phase is TouchPhase.Stationary or TouchPhase.Began)
                {
                    distance = Vector2.Distance(one.position, two.position);
                    deltaDistance = distance - previousZoomDistance;
                    previousZoomDistance = distance;
                }
            }
            else
            {
                previousZoomDistance = 0;
            }
        }
        void Set(Settings settings)
        {
            float radius = settings.distance;
            float alpha = settings.alpha;
            float tetha = settings.tetha;

            float x = radius * Mathf.Sin(alpha) * Mathf.Cos(tetha);
            float y = radius * Mathf.Cos(alpha);
            float z = radius * Mathf.Sin(alpha) * Mathf.Sin(tetha);

            Vector3 offset = new Vector3(x, y, z) + settings.offset;
            Vector3 newPosition = (target.position) + (target.rotation * offset);
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * settings.smoothness);
        }
        public void ResetSettings()
        {
            settings = baseSettings;
            target = baseTarget;
        }
        public void ChangeSettings(Settings settings)
        {
            this.settings = settings;
        }
        public void ChangeTarget(Transform target)
        {
            this.target = target;
        }
        [System.Serializable]
        public struct Settings
        {
            public Vector3 offset;
            [Range(-2, 2)] public float alpha;
            [Range(-2, 2)] public float tetha;
            public float distance;
            public float smoothness;
        }
    }

}