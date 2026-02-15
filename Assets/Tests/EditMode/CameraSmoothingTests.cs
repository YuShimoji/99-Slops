using GlitchWorker.Camera;
using NUnit.Framework;
using UnityEngine;

namespace GlitchWorker.Tests.EditMode
{
    [TestFixture]
    public class CameraSmoothingTests
    {
        [Test]
        public void SmoothYawPitch_ConvergesToTarget()
        {
            var state = new SmoothedRotationState();
            float targetYaw = 90f;
            float targetPitch = 45f;

            // Simulate 2 seconds of smoothing at 60fps
            for (int i = 0; i < 120; i++)
            {
                CameraSmoother.SmoothYawPitch(targetYaw, targetPitch, ref state, 0.06f, 1f / 60f);
            }

            Assert.AreEqual(targetYaw, state.SmoothedYaw, 0.5f,
                "Yaw should converge to target within tolerance");
            Assert.AreEqual(targetPitch, state.SmoothedPitch, 0.5f,
                "Pitch should converge to target within tolerance");
        }

        [Test]
        public void SmoothYawPitch_ZeroDeltaTime_DoesNotCrash()
        {
            var state = new SmoothedRotationState();
            Assert.DoesNotThrow(() =>
            {
                CameraSmoother.SmoothYawPitch(90f, 45f, ref state, 0.06f, 0f);
            });
        }

        [Test]
        public void SmoothYawPitch_ZeroSmoothTime_UsesMinimum()
        {
            var state = new SmoothedRotationState();
            // Should not throw with smoothTime = 0 (clamped internally)
            Assert.DoesNotThrow(() =>
            {
                CameraSmoother.SmoothYawPitch(90f, 45f, ref state, 0f, 1f / 60f);
            });
        }

        [Test]
        public void AccumulateLookInput_ClampsMaxPitch()
        {
            float yaw = 0f;
            float pitch = 0f;
            float maxPitch = 80f;

            // Large upward input
            CameraSmoother.AccumulateLookInput(
                new Vector2(0f, 1000f), 2f, 2f, false, maxPitch, 1f,
                ref yaw, ref pitch);

            Assert.AreEqual(-maxPitch, pitch, 0.01f,
                "Pitch should be clamped to -maxPitch (invertY=false means negative pitch is up)");
        }

        [Test]
        public void AccumulateLookInput_InvertY_ReversesDirection()
        {
            float yaw = 0f;
            float pitchNormal = 0f;
            float pitchInverted = 0f;

            CameraSmoother.AccumulateLookInput(
                new Vector2(0f, 1f), 2f, 2f, false, 80f, 1f,
                ref yaw, ref pitchNormal);

            yaw = 0f;
            CameraSmoother.AccumulateLookInput(
                new Vector2(0f, 1f), 2f, 2f, true, 80f, 1f,
                ref yaw, ref pitchInverted);

            Assert.AreEqual(-pitchNormal, pitchInverted, 0.001f,
                "Inverted Y should produce opposite pitch");
        }

        [Test]
        public void AccumulateLookInput_AccumulatesYaw()
        {
            float yaw = 0f;
            float pitch = 0f;

            CameraSmoother.AccumulateLookInput(
                new Vector2(5f, 0f), 2f, 2f, false, 80f, 1f,
                ref yaw, ref pitch);

            Assert.AreEqual(10f, yaw, 0.001f, "Yaw should accumulate as input.x * sensitivityX");
        }
    }
}
