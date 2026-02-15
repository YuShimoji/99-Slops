using GlitchWorker.Camera;
using NUnit.Framework;
using UnityEngine;

namespace GlitchWorker.Tests.EditMode
{
    [TestFixture]
    public class CameraModeStateTests
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
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_playerGo);
            Object.DestroyImmediate(_managerGo);
        }

        [Test]
        public void DefaultMode_IsFirstPerson()
        {
            Assert.AreEqual(CameraViewMode.FirstPerson, _manager.ActiveMode);
        }

        [Test]
        public void Toggle_SwitchesToThirdPerson()
        {
            _manager.ToggleFirstThirdPerson();
            Assert.AreEqual(CameraViewMode.ThirdPerson, _manager.ActiveMode);
        }

        [Test]
        public void Toggle_Twice_ReturnsToFirstPerson()
        {
            _manager.ToggleFirstThirdPerson();
            _manager.ToggleFirstThirdPerson();
            Assert.AreEqual(CameraViewMode.FirstPerson, _manager.ActiveMode);
        }

        [Test]
        public void EnterCinematic_SetsCinematicMode()
        {
            var point = new GameObject("CinematicPoint").transform;
            _manager.EnterCinematic(point);
            Assert.AreEqual(CameraViewMode.Cinematic, _manager.ActiveMode);
            Object.DestroyImmediate(point.gameObject);
        }

        [Test]
        public void ExitCinematic_RestoresPreviousMode()
        {
            var point = new GameObject("CinematicPoint").transform;

            // Start in 3P, enter cinematic, exit → should return to 3P
            _manager.ToggleFirstThirdPerson();
            Assert.AreEqual(CameraViewMode.ThirdPerson, _manager.ActiveMode);

            _manager.EnterCinematic(point);
            Assert.AreEqual(CameraViewMode.Cinematic, _manager.ActiveMode);

            _manager.ExitCinematic();
            Assert.AreEqual(CameraViewMode.ThirdPerson, _manager.ActiveMode);

            Object.DestroyImmediate(point.gameObject);
        }

        [Test]
        public void Toggle_DuringCinematic_DoesNothing()
        {
            var point = new GameObject("CinematicPoint").transform;
            _manager.EnterCinematic(point);
            _manager.ToggleFirstThirdPerson();
            Assert.AreEqual(CameraViewMode.Cinematic, _manager.ActiveMode);
            Object.DestroyImmediate(point.gameObject);
        }

        [Test]
        public void ExitCinematic_WrongZone_DoesNotExit()
        {
            var point = new GameObject("CinematicPoint").transform;
            var zoneGo = new GameObject("Zone");
            zoneGo.AddComponent<BoxCollider>();
            var zone = zoneGo.AddComponent<CinematicCameraZone>();

            var otherZoneGo = new GameObject("OtherZone");
            otherZoneGo.AddComponent<BoxCollider>();
            var otherZone = otherZoneGo.AddComponent<CinematicCameraZone>();

            _manager.EnterCinematic(point, zone);
            _manager.ExitCinematic(otherZone);
            Assert.AreEqual(CameraViewMode.Cinematic, _manager.ActiveMode);

            Object.DestroyImmediate(point.gameObject);
            Object.DestroyImmediate(zoneGo);
            Object.DestroyImmediate(otherZoneGo);
        }

        [Test]
        public void EnterCinematic_NullPoint_DoesNothing()
        {
            _manager.EnterCinematic(null);
            Assert.AreEqual(CameraViewMode.FirstPerson, _manager.ActiveMode);
        }
    }
}
