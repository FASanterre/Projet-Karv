using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro Dropdown

namespace KARV
{
    public class DeformationUIManager : MonoBehaviour
    {
        CaptureManager captureManager;

        [Header("Mesh Deformers")]
        public List<MeshDeformer> meshDeformers = new List<MeshDeformer>();
        public TMP_Dropdown objectDropdown;

        [Header("UI Elements")]
        public TMP_Dropdown deformationDropdown;
        public Transform jointsGridParent; // Parent object for grid layout
        public Toggle jointTogglePrefab; // Prefab for each joint toggle

        [Header("Impact Settings")]
        public Slider impactForceSlider;
        public TMP_InputField impactForceField;

        public Slider impactForceOffsetSlider;
        public TMP_InputField impactForceOffsetField;

        public Slider impactSpringForceSlider;
        public TMP_InputField impactSpringForceField;

        public Slider impactDampingSlider;
        public TMP_InputField impactDampingField;

        [Header("Twist Settings")]
        public Slider twistForceSlider;
        public TMP_InputField twistForceField;

        public TMP_InputField twistAxisXField;
        public TMP_InputField twistAxisYField;
        public TMP_InputField twistAxisZField;

        public Slider twistRadiusSlider;
        public TMP_InputField twistRadiusField;

        public Slider twistRatioSlider;
        public TMP_InputField twistRatioField;

        [Header("Rotation Settings")]
        public Slider rotationAngleXSlider;
        public TMP_InputField rotationAngleXField;

        public Slider rotationAngleYSlider;
        public TMP_InputField rotationAngleYField;

        public Slider epsilonSlider;
        public TMP_InputField epsilonField;

        public Slider acceptedDiffXSlider;
        public TMP_InputField acceptedDiffXField;

        public Slider acceptedDiffYSlider;
        public TMP_InputField acceptedDiffYField;


        private CaptureManager.DeformationTypes currentDeformation = CaptureManager.DeformationTypes.none;
        private List<KinectInterop.JointType> activeJoints = new List<KinectInterop.JointType>();
        private Dictionary<KinectInterop.JointType, Toggle> jointToggles = new Dictionary<KinectInterop.JointType, Toggle>();

        // Map default joints for each deformation
        private readonly Dictionary<CaptureManager.DeformationTypes, List<KinectInterop.JointType>> defaultJointMappings = new Dictionary<CaptureManager.DeformationTypes, List<KinectInterop.JointType>>()
    {
        { CaptureManager.DeformationTypes.none, new List<KinectInterop.JointType>() },
        { CaptureManager.DeformationTypes.impact, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } },
        { CaptureManager.DeformationTypes.impactFPS, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } },
        { CaptureManager.DeformationTypes.twistingImpact, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } },
        { CaptureManager.DeformationTypes.twisterStarRail, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } },
        { CaptureManager.DeformationTypes.rotationPush, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } },
        { CaptureManager.DeformationTypes.rotationRoast, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } },
        { CaptureManager.DeformationTypes.zoomIn, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } },
        { CaptureManager.DeformationTypes.zoomOut, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } },
        { CaptureManager.DeformationTypes.zoomInOut, new List<KinectInterop.JointType> { KinectInterop.JointType.HandLeft, KinectInterop.JointType.HandRight } }
    };

        void Start()
        {
            captureManager = CaptureManager.Instance;

            PopulateObjectDropdown();
            objectDropdown.onValueChanged.AddListener(OnObjectSelected);

            // Populate the dropdown
            PopulateDropdown();

            // Populate the grid
            PopulateJointsGrid();

            // Attach listeners to the dropdown
            deformationDropdown.onValueChanged.AddListener(OnDeformationChanged);

            InitializeSliders();

            // Set the default deformation
            OnDeformationChanged(0);
        }
        private void PopulateObjectDropdown()
        {
            objectDropdown.ClearOptions();

            if (meshDeformers == null || meshDeformers.Count == 0)
            {
                Debug.LogWarning("No Mesh Deformers found.");
                return;
            }

            var options = new List<string>();
            foreach (var deformer in meshDeformers)
            {
                if (deformer != null)
                {
                    options.Add(deformer.name);
                }
            }
            objectDropdown.AddOptions(options);

            // Set the first object as active by default
            if (meshDeformers.Count > 0 && meshDeformers[0] != null)
            {
                SetActiveMeshDeformer(0);
            }
        }

        private void OnObjectSelected(int index)
        {
            if (index >= 0 && index < meshDeformers.Count && meshDeformers[index] != null)
            {
                SetActiveMeshDeformer(index);
            }
        }

        private void SetActiveMeshDeformer(int index)
        {
            MeshDeformer selectedDeformer = meshDeformers[index];
            if (selectedDeformer != null)
            {
                CaptureManager.Instance.SetActiveObject(selectedDeformer);
                Debug.Log($"Active MeshDeformer set to: {selectedDeformer.name}");
            }
        }
        private void PopulateDropdown()
        {
            deformationDropdown.ClearOptions();
            var options = new List<string>();

            foreach (CaptureManager.DeformationTypes deformation in System.Enum.GetValues(typeof(CaptureManager.DeformationTypes)))
            {
                options.Add(deformation.ToString());
            }

            deformationDropdown.AddOptions(options);
        }

        private void InitializeSliders()
        {
            // Impact Settings
            InitializeSlider(impactForceSlider, impactForceField,
                CaptureManager.Instance.ImpactForce,
                0f, 100f,
                (value) => CaptureManager.Instance.ImpactForce = value);

            InitializeSlider(impactForceOffsetSlider, impactForceOffsetField,
                CaptureManager.Instance.ImpactForceOffset,
                0f, 10f,
                (value) => CaptureManager.Instance.ImpactForceOffset = value);

            InitializeSlider(impactSpringForceSlider, impactSpringForceField,
                CaptureManager.Instance.ImpactSpringForce,
                0f, 100f,
                (value) => CaptureManager.Instance.ImpactSpringForce = value);

            InitializeSlider(impactDampingSlider, impactDampingField,
                CaptureManager.Instance.ImpactDamping,
                0f, 50f,
                (value) => CaptureManager.Instance.ImpactDamping = value);

            // Twist Settings
            InitializeSlider(twistForceSlider, twistForceField,
                CaptureManager.Instance.twistForce,
                0f, 50f,
                (value) => CaptureManager.Instance.twistForce = value);

            InitializeVectorField(twistAxisXField,
                CaptureManager.Instance.twistAxis.x,
                (value) => CaptureManager.Instance.twistAxis = new Vector3(value, CaptureManager.Instance.twistAxis.y, CaptureManager.Instance.twistAxis.z));
            InitializeVectorField(twistAxisYField,
                CaptureManager.Instance.twistAxis.y,
                (value) => CaptureManager.Instance.twistAxis = new Vector3(CaptureManager.Instance.twistAxis.x, value, CaptureManager.Instance.twistAxis.z));
            InitializeVectorField(twistAxisZField,
                CaptureManager.Instance.twistAxis.z,
                (value) => CaptureManager.Instance.twistAxis = new Vector3(CaptureManager.Instance.twistAxis.x, CaptureManager.Instance.twistAxis.y, value));

            InitializeSlider(twistRadiusSlider, twistRadiusField,
                CaptureManager.Instance.twistRadius,
                0f, 1f,
                (value) => CaptureManager.Instance.twistRadius = value);

            InitializeSlider(twistRatioSlider, twistRatioField,
                CaptureManager.Instance.twistRatio,
                0f, 10f,
                (value) => CaptureManager.Instance.twistRatio = value);

            // Rotation Settings
            InitializeSlider(rotationAngleXSlider, rotationAngleXField,
                CaptureManager.Instance.rotationAngleX,
                -180f, 180f,
                (value) => CaptureManager.Instance.rotationAngleX = value);

            InitializeSlider(rotationAngleYSlider, rotationAngleYField,
                CaptureManager.Instance.rotationAngleY,
                -180f, 180f,
                (value) => CaptureManager.Instance.rotationAngleY = value);

            // Collision Tolerance Settings
            InitializeSlider(epsilonSlider, epsilonField,
                CaptureManager.Instance.epsilon,
                0f, 5f,
                (value) => CaptureManager.Instance.epsilon = value);

            InitializeSlider(acceptedDiffXSlider, acceptedDiffXField,
                CaptureManager.Instance.acceptedDiffX,
                0f, 5f,
                (value) => CaptureManager.Instance.acceptedDiffX = value);

            InitializeSlider(acceptedDiffYSlider, acceptedDiffYField,
                CaptureManager.Instance.acceptedDiffY,
                0f, 5f,
                (value) => CaptureManager.Instance.acceptedDiffY = value);
        }

        // Helper function to initialize sliders and input fields
        private void InitializeSlider(Slider slider, TMP_InputField field, float initialValue, float min, float max, System.Action<float> onValueChanged)
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = initialValue;
            field.text = initialValue.ToString("F2");

            // Synchronize slider and input field
            slider.onValueChanged.AddListener((value) =>
            {
                field.text = value.ToString("F2");
                onValueChanged(value);
            });

            field.onEndEdit.AddListener((text) =>
            {
                if (float.TryParse(text, out float value))
                {
                    value = Mathf.Clamp(value, min, max); // Ensure within range
                    slider.value = value;
                    onValueChanged(value);
                }
            });
        }

        // Helper function to initialize vector field input
        private void InitializeVectorField(TMP_InputField field, float initialValue, System.Action<float> onValueChanged)
        {
            field.text = initialValue.ToString("F2");

            field.onEndEdit.AddListener((text) =>
            {
                if (float.TryParse(text, out float value))
                {
                    onValueChanged(value);
                }
            });
        }

        private void PopulateJointsGrid()
        {
            foreach (KinectInterop.JointType joint in System.Enum.GetValues(typeof(KinectInterop.JointType)))
            {
                Toggle toggle = Instantiate(jointTogglePrefab, jointsGridParent);
                toggle.isOn = false; // Default to unchecked
                toggle.GetComponentInChildren<Text>().text = joint.ToString(); // Set label
                jointToggles[joint] = toggle;

                // Add listener to toggle
                toggle.onValueChanged.AddListener((isChecked) => OnJointToggleChanged(joint, isChecked));
            }
        }

        private void OnDeformationChanged(int index)
        {
            currentDeformation = (CaptureManager.DeformationTypes)index;

            // Update active joints to match the default for the selected deformation
            activeJoints = new List<KinectInterop.JointType>(defaultJointMappings[currentDeformation]);

            // Update the grid toggles to reflect the active joints
            foreach (var kvp in jointToggles)
            {
                kvp.Value.isOn = activeJoints.Contains(kvp.Key);
            }

            // Update CaptureManager with current deformation and active joints
            UpdateCaptureManager();
        }

        private void OnJointToggleChanged(KinectInterop.JointType joint, bool isChecked)
        {
            if (isChecked)
            {
                if (!activeJoints.Contains(joint))
                {
                    activeJoints.Add(joint);
                }
            }
            else
            {
                activeJoints.Remove(joint);
            }

            // Update CaptureManager with current deformation and active joints
            UpdateCaptureManager();
        }

        private void UpdateCaptureManager()
        {
            captureManager.ChangeActiveDeformation(currentDeformation, activeJoints);
            if (currentDeformation == CaptureManager.DeformationTypes.none)
            {
                captureManager.HandleInput();
            }
        }
    }
}