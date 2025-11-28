using System.Diagnostics;
using System.Numerics;
using Leatha.WarOfTheElements.Common.Communication.Utilities;

namespace Leatha.WarOfTheElements.Server.Objects.Characters.Movement
{
    public interface IMovementProcessor
    {
        void Process(double delta);

        bool IsRunning();

        void SetRunning(bool isRunning);
    }

    public sealed class MotionMaster
    {
        public MotionMaster(NonPlayerState ownerState)
        {
            OwnerNonPlayer = ownerState;

            _idleMovementProcessor = new IdleMovementProcessor(this);
            _movementProcessor = new MotionMasterMovementProcessor(this);
            _waypointMovementProcessor = new WaypointMovementProcessor(this);
            _rotationProcessor = new MotionMasterRotationProcessor(this);

            // Set it to the idle.
            CurrentMovementProcessor = _idleMovementProcessor;
        }

        public NonPlayerState OwnerNonPlayer { get; }

        public IMovementProcessor CurrentMovementProcessor { get; private set; }

        /// <summary>
        /// Local input-style velocity: X = strafe, Y = forward, Z unused.
        /// This is consumed by NonPlayerState.ComputeDesiredVelocity.
        /// </summary>
        public Vector3 Velocity { get; set; }

        public bool IsMoving => Velocity.LengthSquared() > 0;

        private readonly IdleMovementProcessor _idleMovementProcessor;
        private readonly MotionMasterMovementProcessor _movementProcessor;
        private readonly WaypointMovementProcessor _waypointMovementProcessor;
        private readonly MotionMasterRotationProcessor _rotationProcessor;

        public void Update(double delta)
        {
            //_movementProcessor.Process(delta);

            CurrentMovementProcessor.Process(delta);
            _rotationProcessor.Process(delta);
        }

        public void MoveToSpawnPosition(bool isWalking = true)
        {
            _movementProcessor.SetWalking(isWalking);
            _movementProcessor.MoveTo(OwnerNonPlayer.SpawnPosition);
        }

        public void MoveTo(Vector3 position, bool isWalking = true)
        {
            _movementProcessor.SetWalking(isWalking);
            _movementProcessor.MoveTo(position);
        }

        public void MoveTo(Vector3 position, float rotationInDeg, bool isWalking = true)
        {
            _movementProcessor.SetWalking(isWalking);
            _movementProcessor.MoveTo(position);
            _rotationProcessor.Rotate(rotationInDeg);
        }

        public void MoveWaypoints(List<WaypointData> waypoints, bool repeat, bool isWalking = true)
        {
            //_movementProcessor.SetWalking(isWalking);
            //_movementProcessor.MoveWaypoints(waypoints, repeat);
            CurrentMovementProcessor = _waypointMovementProcessor;
            _waypointMovementProcessor.MoveWaypoints(waypoints, repeat);
        }

        public void RotateTo(Vector3 position)
        {
            _rotationProcessor.RotateTo(position);
        }

        public void RotateTo(float rotationInDeg)
        {
            _rotationProcessor.Rotate(rotationInDeg);
        }

        public void StopRotating()
        {
            _rotationProcessor.StopRotating();
        }

        public void StopMovement()
        {
            _movementProcessor.StopMovement();
        }

        public void SetPaused(bool paused)
        {
            _movementProcessor.SetPaused(paused);
            // Optionally also pause rotation.
        }

        public void SetWalking(bool isWalking)
        {
            _movementProcessor.SetWalking(isWalking);
        }

        public bool IsWalking() => _movementProcessor.IsWalking();
        public bool IsPaused() => _movementProcessor.IsPaused();
    }

    public sealed class WaypointData
    {
        public float PositionX { get; set; }

        public float PositionY { get; set; }

        public float PositionZ { get; set; }

        public float Rotation { get; set; }

        public int DelayMin { get; set; }

        public int DelayMax { get; set; }
    }

    public sealed class IdleMovementProcessor : IMovementProcessor
    {
        public IdleMovementProcessor(MotionMaster motionMaster)
        {
            _motionMaster = motionMaster;
        }

        private readonly MotionMaster _motionMaster;

        public void Process(double delta)
        {
            // Nothing to do, does not move at all.
        }

        public bool IsRunning()
        {
            return false;
        }

        public void SetRunning(bool isRunning)
        {
            // Nothing to do, does not move at all.
        }
    }

    public sealed class WaypointMovementProcessor : IMovementProcessor
    {
        public WaypointMovementProcessor(MotionMaster motionMaster)
        {
            _motionMaster = motionMaster;
        }

        private readonly MotionMaster _motionMaster;

        private Vector3? _moveTargetPosition;
        private bool _isWalking;

        private readonly List<WaypointData> _waypoints = new();
        private int _waypointIndex = -1;
        private bool _repeatWaypoints;

        private double? _waitDelay; // seconds

        // Be a little more generous; too small can cause “never reached”.
        private const float ArriveDistance = 0.5f;

        public void Process(double delta)
        {
            // Local intent each tick
            _motionMaster.Velocity = Vector3.Zero;

            if (CheckWaitDelay(delta))
                return;

            ProcessMoveToTarget(delta);
        }

        public bool IsRunning()
        {
            return false;
        }

        public void SetRunning(bool isRunning)
        {
            // Nothing to do, does not move at all.
        }

        public void MoveWaypoints(List<WaypointData> waypoints, bool repeat)
        {
            _waypoints.Clear();
            _waypoints.AddRange(waypoints);

            if (_waypoints.Count == 0)
            {
                Reset();
                return;
            }

            _repeatWaypoints = repeat;
            _waypointIndex = 0;
            _waitDelay = null;

            var wp = _waypoints[_waypointIndex];
            _moveTargetPosition = new Vector3(wp.PositionX, wp.PositionY, wp.PositionZ);
        }

        private void Reset()
        {
            _waypoints.Clear();
            _repeatWaypoints = false;
            _waypointIndex = -1;
            _waitDelay = null;
            _moveTargetPosition = null;
        }

        private void ProcessMoveToTarget(double delta)
        {
            if (!_moveTargetPosition.HasValue)
                return;

            var owner = _motionMaster.OwnerNonPlayer;
            var currentPos = owner.Position;

            var toTarget = _moveTargetPosition.Value - currentPos;
            //toTarget.Y = 0f; // #TODO

            var sqDist = toTarget.LengthSquared();
            if (sqDist <= ArriveDistance * ArriveDistance)
            {
                // We’ve arrived at this target
                AdvanceWaypoint();
                return;
            }

            // Move towards target
            var worldDir = Vector3.Normalize(toTarget);

            var yaw = owner.Yaw;
            var sin = MathF.Sin(yaw);
            var cos = MathF.Cos(yaw);

            var forward = new Vector3(-sin, 0f, -cos);
            var right = new Vector3(cos, 0f, -sin);

            var localForward = Vector3.Dot(worldDir, forward);
            var localRight = Vector3.Dot(worldDir, right);

            var localMove = new Vector3(localRight, localForward, 0f);
            if (localMove.LengthSquared() > 1f)
                localMove = Vector3.Normalize(localMove);

            _motionMaster.Velocity = localMove;
        }

        private bool CheckWaitDelay(double delta)
        {
            if (!_waitDelay.HasValue)
                return false;

            if (_waitDelay <= 0)
            {
                _waitDelay = null;
                return false;
            }

            _waitDelay -= delta;
            return true;
        }

        private void AdvanceWaypoint()
        {
            if (_waypointIndex < 0 || _waypointIndex >= _waypoints.Count)
            {
                _moveTargetPosition = null;
                return;
            }

            var waypoint = _waypoints[_waypointIndex];

            // Let the script know we reached this one
            _motionMaster.OwnerNonPlayer.Script?.OnWaypointReached(_waypointIndex);

            _waypointIndex++;

            var end = _waypointIndex >= _waypoints.Count;
            if (end)
            {
                if (!_repeatWaypoints)
                {
                    _waypointIndex = -1;
                    _moveTargetPosition = null;
                    return;
                }

                // Loop back
                _waypointIndex = 0;
            }

            var next = _waypoints[_waypointIndex];
            var delayMs = CommonExtensions.Random(next.DelayMin, next.DelayMax);

            if (delayMs <= 0)
            {
                _moveTargetPosition = new Vector3(next.PositionX, next.PositionY, next.PositionZ);
            }
            else
            {
                _waitDelay = delayMs / 1000.0; // ms -> seconds
                _moveTargetPosition = null;
            }

            _waitDelay = delayMs > 0 ? delayMs / 1000.0 : 0.0f;
            _moveTargetPosition = new Vector3(next.PositionX, next.PositionY, next.PositionZ);
        }
    }

    public sealed class MotionMasterMovementProcessor : IMovementProcessor
    {
        public MotionMasterMovementProcessor(MotionMaster motionMaster)
        {
            _motionMaster = motionMaster;
            _isWalking = !_motionMaster.OwnerNonPlayer.IsSprinting;
        }

        private readonly MotionMaster _motionMaster;

        private Vector3? _moveTargetPosition;
        private bool _isPaused;
        private bool _isWalking;

        private readonly List<WaypointData> _waypoints = new();
        private int _waypointIndex = -1;
        private bool _repeatWaypoints;
        private bool _isMovingOnWaypoints;

        private double? _waitDelay; // seconds

        // Be a little more generous; too small can cause “never reached”.
        private const float ArriveDistance = 0.5f;

        public void Process(double delta)
        {
            // Local intent each tick
            _motionMaster.Velocity = Vector3.Zero;

            if (_isPaused)
                return;

            if (CheckWaitDelay(delta))
                return;

            if (_moveTargetPosition.HasValue)
            {
                ProcessMoveToTarget(delta);
            }
        }

        public bool IsRunning()
        {
            throw new NotImplementedException();
        }

        public void SetRunning(bool isRunning)
        {
            throw new NotImplementedException();
        }

        private void ProcessMoveToTarget(double delta)
        {
            var owner = _motionMaster.OwnerNonPlayer;
            var currentPos = owner.Position;

            var toTarget = _moveTargetPosition.Value - currentPos;
            toTarget.Y = 0f;

            var sqDist = toTarget.LengthSquared();
            if (sqDist <= ArriveDistance * ArriveDistance)
            {
                // We’ve arrived at this target
                if (_isMovingOnWaypoints)
                    AdvanceWaypoint();
                else
                    _moveTargetPosition = null; // simple MoveTo

                return;
            }

            // Move towards target
            var worldDir = Vector3.Normalize(toTarget);

            var yaw = owner.Yaw;
            var sin = MathF.Sin(yaw);
            var cos = MathF.Cos(yaw);

            var forward = new Vector3(-sin, 0f, -cos);
            var right = new Vector3(cos, 0f, -sin);

            var localForward = Vector3.Dot(worldDir, forward);
            var localRight = Vector3.Dot(worldDir, right);

            var localMove = new Vector3(localRight, localForward, 0f);
            if (localMove.LengthSquared() > 1f)
                localMove = Vector3.Normalize(localMove);

            _motionMaster.Velocity = localMove;
        }

        public void MoveTo(Vector3 position)
        {
            _isMovingOnWaypoints = false;
            _moveTargetPosition = position;
            _waitDelay = null;
        }

        public void MoveWaypoints(List<WaypointData> waypoints, bool repeat)
        {
            _waypoints.Clear();
            _waypoints.AddRange(waypoints);

            if (_waypoints.Count == 0)
            {
                Reset();
                return;
            }

            _isMovingOnWaypoints = true;
            _repeatWaypoints = repeat;
            _waypointIndex = 0;
            _waitDelay = null;

            var wp = _waypoints[_waypointIndex];
            _moveTargetPosition = new Vector3(wp.PositionX, wp.PositionY, wp.PositionZ);
        }

        public void StopMovement()
        {
            Reset();
        }

        public void SetPaused(bool paused) => _isPaused = paused;
        public void SetWalking(bool isWalking) => _isWalking = isWalking;

        public bool IsPaused() => _isPaused;
        public bool IsWalking() => _isWalking;

        private void Reset()
        {
            _waypoints.Clear();
            _isMovingOnWaypoints = false;
            _repeatWaypoints = false;
            _waypointIndex = -1;
            _waitDelay = null;
            _isPaused = false;
            _moveTargetPosition = null;
        }

        private bool CheckWaitDelay(double delta)
        {
            if (!_waitDelay.HasValue)
                return false;

            if (_waitDelay <= 0)
            {
                _waitDelay = null;
                return false;
            }

            _waitDelay -= delta;
            return true;
        }

        private void AdvanceWaypoint()
        {
            if (!_isMovingOnWaypoints)
            {
                _moveTargetPosition = null;
                return;
            }

            if (_waypointIndex < 0 || _waypointIndex >= _waypoints.Count)
            {
                _moveTargetPosition = null;
                _isMovingOnWaypoints = false;
                return;
            }

            var waypoint = _waypoints[_waypointIndex];

            // Let the script know we reached this one
            _motionMaster.OwnerNonPlayer.Script?.OnWaypointReached(_waypointIndex);

            _waypointIndex++;

            var end = _waypointIndex >= _waypoints.Count;
            if (end)
            {
                if (!_repeatWaypoints)
                {
                    _waypointIndex = -1;
                    _isMovingOnWaypoints = false;
                    _moveTargetPosition = null;
                    return;
                }

                // Loop back
                _waypointIndex = 0;
            }

            var next = _waypoints[_waypointIndex];
            var delayMs = CommonExtensions.Random(next.DelayMin, next.DelayMax);

            if (delayMs <= 0)
            {
                _moveTargetPosition = new Vector3(next.PositionX, next.PositionY, next.PositionZ);
            }
            else
            {
                _waitDelay = delayMs / 1000.0; // ms -> seconds
                _moveTargetPosition = null;
            }
        }
    }

    public sealed class MotionMasterRotationProcessor : IMovementProcessor
    {
        public MotionMasterRotationProcessor(MotionMaster motionMaster)
        {
            _motionMaster = motionMaster;
        }

        private readonly MotionMaster _motionMaster;
        private bool _isRotating;
        private Vector3? _rotationTarget;
        private float? _finalRotation; // radians

        private const float RotationSpeed = 5f; // rad/s

        public void Process(double delta)
        {
            if (!_isRotating)
                return;

            var owner = _motionMaster.OwnerNonPlayer;
            var yaw = owner.Yaw;

            float targetYaw;

            if (_rotationTarget.HasValue)
            {
                var dir = _rotationTarget.Value - owner.Position;
                dir.Y = 0;
                if (dir.LengthSquared() < 1e-6f)
                    return;

                // Same convention as movement (yaw=0 -> forward=-Z)
                targetYaw = MathF.Atan2(-dir.X, -dir.Z);
            }
            else if (_finalRotation.HasValue)
            {
                targetYaw = _finalRotation.Value;
            }
            else
            {
                _isRotating = false;
                return;
            }

            var newYaw = LerpAngle(yaw, targetYaw, (float)(RotationSpeed * delta));

            owner.Yaw = newYaw;

            if (MathF.Abs(NormalizeAngle(targetYaw - newYaw)) < 0.01f)
                Reset();
        }

        public bool IsRunning()
        {
            throw new NotImplementedException();
        }

        public void SetRunning(bool isRunning)
        {
            throw new NotImplementedException();
        }

        public void RotateTo(Vector3 position)
        {
            _isRotating = true;
            _rotationTarget = position;
            _finalRotation = null;
        }

        public void Rotate(float finalRotationDeg)
        {
            _isRotating = true;
            _finalRotation = MathF.PI * finalRotationDeg / 180f;
            _rotationTarget = null;
        }

        public void StopRotating()
        {
            Reset();
        }

        private void Reset()
        {
            _finalRotation = null;
            _rotationTarget = null;
            _isRotating = false;
        }

        private static float NormalizeAngle(float a)
        {
            while (a > MathF.PI)
                a -= 2f * MathF.PI;

            while (a < -MathF.PI)
                a += 2f * MathF.PI;

            return a;
        }

        private static float LerpAngle(float from, float to, float t)
        {
            var diff = NormalizeAngle(to - from);
            return from + diff * Math.Clamp(t, 0f, 1f);
        }
    }








    public sealed class MoveFinishedEventArgs : EventArgs
    {
        public Vector3 Position { get; set; }
    }
}
