namespace Engendro.Input
{
    /// <summary>
    /// GamePadVibrationSettings
    /// </summary>
    public sealed class GamePadVibrationSettings
    {
        // Constructor
        public GamePadVibrationSettings(int duration, float leftMotor, float rightMotor)
            : this(duration, leftMotor, rightMotor, 0, 0)
        {
        }

        // Constructor
        public GamePadVibrationSettings(int duration, float leftMotor, float rightMotor, float leftTrigger, float rightTrigger)
        {
            this.Duration = duration;
            this.LeftMotor = leftMotor;
            this.RightMotor = rightMotor;
            this.LeftTrigger = leftTrigger;
            this.RightTrigger = rightTrigger;
        }

        // Duration
        public int Duration { get; }

        // LeftMotor
        public float LeftMotor { get; }

        // LeftTrigger
        public float LeftTrigger { get; }

        // RightMotor
        public float RightMotor { get; }

        // RightTrigger
        public float RightTrigger { get; }
    }
}
