using System;

namespace Engendro
{
    // AnalogButtonSide
    public enum AnalogButtonSide { Left, Right }

    // AnimationDirection
    public enum AnimationDirection { Forward, Reverse }

    // CameraShakeState
    public enum CameraShakeState { None, X, Y, XY }

    // FadeMode
    public enum FadeMode { In, Out }

    // FadeState
    public enum FadeState { None, In, Out }

    // FramePosition
    public enum FramePosition { First, Next, Previous, Last, Random }

    // GamePadThumbStick
    public enum GamePadThumbStick { Left, Right }

    // GamePadStyle
    public enum GamePadStyle { Xbox, XBoxOne, XBoxSeries, PlayStation, PlayStation4, PlayStation5, NintendoSwitch }

    // HorizontalAlignment
    public enum HorizontalAlignment { Left, Center, Right }

    // HandleInputResult
    public enum HandleInputResult { Handled, Unhandled }

    // InputMethod
    public enum InputMethod { None, Mouse, GamePad, Keyboard }

    // LoadState
    public enum LoadState { Unloaded, Loading, Loaded }

    // ModifiersKey
    [Flags]
    public enum ModifiersKey { None = 0, Alt = 1, Control = 2, Shift = 4 }

    // MouseButton
    public enum MouseButton { None, Left, Right }

    // PlacementMode
    public enum PlacementMode { Relative, Absolute }

    // PolygonOrientation
    public enum PolygonOrientation { Clockwise, CounterClockwise }

    // RectanglePoint
    public enum RectanglePoint { LeftTop, Top, RightTop, Left, Center, Right, LeftBottom, Bottom, RightBottom }

    // RunningPlatform
    public enum RunningPlatform { Unknown, NintendoSwitch, PlayStation4, PlayStation5, XboxOne, XboxSeries, Windows }

    // RunningState
    public enum RunningState { Stopped, Running, Paused }

    // SceneSettings
    [Flags]
    public enum SceneSettings
    {
        // None
        None = 0,

        // Scene is full screen hiding previous scenes (optimization to draw topmost scene only)
        ExclusiveDraw = 1,

        // Pauses previous scenes
        PausePreviousScenes = 2
    }

    // ScrollLock
    public enum ScrollLock { None, Horizontal, Vertical, All }

    // SoundCategoryName
    public enum SoundCategoryName { FX, Ambience, Voice, Music }

    // SoundPopMode
    public enum SoundPopMode { Cyclic, Random }

    // StopBehavior
    public enum StopBehavior { AsIs, ForceComplete }

    // SwitchState
    public enum SwitchState { Off, On }

    // TransformChange
    public enum TransformChange { Altitude, Effects, PivotOrigin, Position, Rotation, Scale }

    // TransitionState
    public enum TransitionState { In, Out }

    // TweenStyle
    public enum TweenStyle { Linear, ElasticIn, ElasticOut, ElasticInOut, QuadraticIn, QuadraticOut, QuadraticInOut, CubicIn, CubicOut, CubicInOut, QuarticIn, QuarticOut, QuarticInOut, QuinticIn, QuinticOut, QuinticInOut, SineIn, SineOut, SineInOut }

    // VertexType
    public enum VertexType { Concave, Convex }

    // VerticalAlignment
    public enum VerticalAlignment { Top, Center, Bottom }

    // ZoomState
    public enum ZoomState { None, In, Out }
}