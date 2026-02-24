using System.Collections;
using GlitchWorker.Camera;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GlitchWorker.Tests.PlayMode
{
    [TestFixture]
    public class CameraRuntimeSmokeTests
    {
        private GameObject _managerGo;
        private CameraManager _manager;
        private GameObject _playerGo;

        [SetUp]
        public void SetUp()
        {
            _managerGo = new GameObject("CameraManager");
            var cam = new GameObject("Camera").AddComponent<UnityEngine.Camera>();
            cam.transform.SetParent(_managerGo.transform);
            _manager = _managerGo.AddComponent<CameraManager>();

            _playerGo = new GameObject("Player");
            _playerGo.transform.position = Vector3.zero;
        }

        [TearDown]
        public void TearDown()
        {
            // Use DestroyImmediate to ensure the singleton Instance is cleared
            // before the next test's SetUp runs in the same frame.
            Object.DestroyImmediate(_playerGo);
            Object.DestroyImmediate(_managerGo);
        }

        [UnityTest]
        public IEnumerator HandleLook_DoesNotThrow_AfterAwake()
        {
            yield return null; // Allow Awake to run

            Assert.DoesNotThrow(() =>
            {
                _manager.HandleLook(new Vector2(1f, 0.5f), _playerGo.transform);
            });
        }

        [UnityTest]
        public IEnumerator Toggle_RuntimeSwitch_1P_To_3P()
        {
            yield return null;

            Assert.AreEqual(CameraViewMode.FirstPerson, _manager.ActiveMode);
            _manager.ToggleFirstThirdPerson();
            Assert.AreEqual(CameraViewMode.ThirdPerson, _manager.ActiveMode);

            // Apply a few frames of look to let blending progress
            for (int i = 0; i < 10; i++)
            {
                _manager.HandleLook(Vector2.zero, _playerGo.transform);
                yield return null;
            }

            // Camera should have moved away from player (3P offset)
            var camTransform = _manager.ActiveCameraTransform;
            if (camTransform != null)
            {
                float dist = Vector3.Distance(camTransform.position, _playerGo.transform.position);
                Assert.Greater(dist, 0.1f, "Camera should be offset from player in 3P");
            }
        }

        [UnityTest]
        public IEnumerator Cinematic_EnterAndExit_RestoresMode()
        {
            yield return null;

            var pointGo = new GameObject("CinematicPoint");
            pointGo.transform.position = new Vector3(10f, 5f, 10f);

            _manager.EnterCinematic(pointGo.transform);
            Assert.AreEqual(CameraViewMode.Cinematic, _manager.ActiveMode);

            // Run a few frames of cinematic
            for (int i = 0; i < 5; i++)
            {
                _manager.HandleLook(Vector2.zero, _playerGo.transform);
                yield return null;
            }

            _manager.ExitCinematic();
            Assert.AreEqual(CameraViewMode.FirstPerson, _manager.ActiveMode);

            Object.Destroy(pointGo);
        }

        [UnityTest]
        public IEnumerator Forward_ReturnsHorizontalVector()
        {
            yield return null;

            // Apply some look input to create a non-trivial rotation
            for (int i = 0; i < 5; i++)
            {
                _manager.HandleLook(new Vector2(2f, 1f), _playerGo.transform);
                yield return null;
            }

            Vector3 forward = _manager.Forward;
            Assert.AreEqual(0f, forward.y, 0.001f, "Forward.y should be zero (horizontal)");
            Assert.Greater(forward.sqrMagnitude, 0.9f, "Forward should be approximately normalized");
        }
    }
}
