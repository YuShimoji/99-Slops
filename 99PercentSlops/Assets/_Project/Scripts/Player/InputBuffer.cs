using System;
using System.Collections.Generic;

namespace GlitchWorker.Player
{
    [Flags]
    public enum BufferableAction
    {
        None = 0,
        Jump = 1,
        Dash = 2
    }

    /// <summary>
    /// Frame-window input buffer. Records inputs and allows consuming them
    /// within a configurable frame window (default 8 frames).
    /// Prevents "I pressed jump but it didn't come out" by queuing inputs
    /// and releasing them when the player enters a state that accepts them.
    /// </summary>
    public class InputBuffer
    {
        private struct BufferedInput
        {
            public BufferableAction Action;
            public int FrameRecorded;
        }

        private readonly List<BufferedInput> _buffer = new(8);
        private readonly int _bufferWindowFrames;
        private int _currentFrame;

        public InputBuffer(int bufferWindowFrames = 8)
        {
            _bufferWindowFrames = bufferWindowFrames;
        }

        /// <summary>
        /// Advance frame counter and purge expired entries.
        /// Call at the start of each Update.
        /// </summary>
        public void Tick()
        {
            _currentFrame++;

            for (int i = _buffer.Count - 1; i >= 0; i--)
            {
                if (_currentFrame - _buffer[i].FrameRecorded > _bufferWindowFrames)
                {
                    _buffer.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Record an input action at the current frame.
        /// </summary>
        public void RecordInput(BufferableAction action)
        {
            for (int i = 0; i < _buffer.Count; i++)
            {
                if (_buffer[i].Action == action && _buffer[i].FrameRecorded == _currentFrame)
                    return;
            }

            _buffer.Add(new BufferedInput
            {
                Action = action,
                FrameRecorded = _currentFrame
            });
        }

        /// <summary>
        /// Try to consume a buffered input. Returns true and removes the entry
        /// if the action was found in the buffer.
        /// </summary>
        public bool ConsumeInput(BufferableAction action)
        {
            for (int i = 0; i < _buffer.Count; i++)
            {
                if (_buffer[i].Action == action)
                {
                    _buffer.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if a buffered input exists without consuming it.
        /// </summary>
        public bool HasInput(BufferableAction action)
        {
            for (int i = 0; i < _buffer.Count; i++)
            {
                if (_buffer[i].Action == action)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Clear all buffered inputs.
        /// </summary>
        public void Clear()
        {
            _buffer.Clear();
        }
    }
}
