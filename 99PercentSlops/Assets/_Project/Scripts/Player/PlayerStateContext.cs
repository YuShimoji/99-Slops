using UnityEngine;
using GlitchWorker.Drone;

namespace GlitchWorker.Player
{
    public class PlayerStateContext
    {
        // Component references
        public Rigidbody Rb;
        public PlayerMovement Movement;
        public PlayerJump Jump;
        public PlayerStats Stats;
        public PlayerStateMachine StateMachine;
        public InputBuffer Buffer;
        public DroneBeam DroneBeam;

        // Frame input (updated each frame by PlayerController)
        public Vector2 MoveInput;
        public bool JumpRequested;
        public bool DashRequested;
        public bool FastFallRequested;

        // Derived (read from components)
        public bool IsGrounded => Jump != null && Jump.IsGrounded;
        public float SlopeAngle => Jump != null ? Jump.SlopeAngle : 0f;
        public Vector3 GroundNormal => Jump != null ? Jump.GroundNormal : Vector3.up;

        // State timing
        public float StateTimer;

        // Dash charges (managed by PlayerController)
        public int DashChargesRemaining;
        public float DashCooldownTimer;
    }
}
