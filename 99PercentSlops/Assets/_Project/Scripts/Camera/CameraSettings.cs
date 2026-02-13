using UnityEngine;

namespace GlitchWorker.Camera
{
    /// <summary>
    /// Camera設定を集約したScriptableObject
    /// CameraManagerはこのアセットを参照して動作する
    /// </summary>
    [CreateAssetMenu(fileName = "CameraSettings", menuName = "GlitchWorker/Camera Settings")]
    public class CameraSettings : ScriptableObject
    {
        [Header("Look Settings")]
        [Tooltip("水平方向の感度")]
        [SerializeField, Min(0.01f)] private float _sensitivityX = 2f;

        [Tooltip("垂直方向の感度")]
        [SerializeField, Min(0.01f)] private float _sensitivityY = 2f;

        [Tooltip("垂直方向を反転するか")]
        [SerializeField] private bool _invertY;

        [Tooltip("最大仰角/俯角（度）")]
        [SerializeField, Range(10f, 89f)] private float _maxLookAngle = 80f;

        [Tooltip("回転のスムージング時間（秒）")]
        [SerializeField, Min(0f)] private float _rotationSmoothTime = 0.06f;

        [Header("First Person Settings")]
        [Tooltip("1人称時のカメラローカルオフセット（プレイヤー基準）")]
        [SerializeField] private Vector3 _firstPersonLocalOffset = new Vector3(0f, 0.8f, 0f);

        [Header("Third Person Settings")]
        [Tooltip("3人称時のカメラ距離")]
        [SerializeField, Min(0.1f)] private float _thirdPersonDistance = 3.2f;

        [Tooltip("3人称時の高さオフセット")]
        [SerializeField, Min(0f)] private float _thirdPersonHeightOffset = 1.6f;

        [Tooltip("カメラ衝突判定の球半径")]
        [SerializeField, Min(0.01f)] private float _thirdPersonCollisionRadius = 0.2f;

        [Tooltip("カメラ衝突判定の対象レイヤー")]
        [SerializeField] private LayerMask _cameraCollisionMask = ~0;

        [Header("Mode Transition Settings")]
        [Tooltip("視点モード切替時の遷移時間（秒）")]
        [SerializeField, Min(0.01f)] private float _modeTransitionTime = 0.22f;

        [Tooltip("ゲーム開始時のカメラモード")]
        [SerializeField] private CameraViewMode _startupMode = CameraViewMode.FirstPerson;

        // 公開プロパティ
        public float SensitivityX => _sensitivityX;
        public float SensitivityY => _sensitivityY;
        public bool InvertY => _invertY;
        public float MaxLookAngle => _maxLookAngle;
        public float RotationSmoothTime => _rotationSmoothTime;
        public Vector3 FirstPersonLocalOffset => _firstPersonLocalOffset;
        public float ThirdPersonDistance => _thirdPersonDistance;
        public float ThirdPersonHeightOffset => _thirdPersonHeightOffset;
        public float ThirdPersonCollisionRadius => _thirdPersonCollisionRadius;
        public LayerMask CameraCollisionMask => _cameraCollisionMask;
        public float ModeTransitionTime => _modeTransitionTime;
        public CameraViewMode StartupMode => _startupMode;

        /// <summary>
        /// フォールバック用のデフォルト設定を作成
        /// </summary>
        public static CameraSettings CreateDefaultSettings()
        {
            var settings = CreateInstance<CameraSettings>();
            // デフォルト値はフィールド初期化子で設定済み
            return settings;
        }

        /// <summary>
        /// 実行時に値を検証し、無効な場合はデフォルト値を適用
        /// </summary>
        public void Validate()
        {
            _sensitivityX = Mathf.Max(0.01f, _sensitivityX);
            _sensitivityY = Mathf.Max(0.01f, _sensitivityY);
            _maxLookAngle = Mathf.Clamp(_maxLookAngle, 10f, 89f);
            _rotationSmoothTime = Mathf.Max(0f, _rotationSmoothTime);
            _thirdPersonDistance = Mathf.Max(0.1f, _thirdPersonDistance);
            _thirdPersonHeightOffset = Mathf.Max(0f, _thirdPersonHeightOffset);
            _thirdPersonCollisionRadius = Mathf.Max(0.01f, _thirdPersonCollisionRadius);
            _modeTransitionTime = Mathf.Max(0.01f, _modeTransitionTime);
        }
    }
}
