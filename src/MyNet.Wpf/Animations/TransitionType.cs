// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

namespace MyNet.Wpf.Animations;

/// <summary>
/// Available types of transitions.
/// </summary>
public enum TransitionType
{
    /// <summary>
    /// None.
    /// </summary>
    None,

    /// <summary>
    /// Change opacity.
    /// </summary>
    FadeIn,

    FadeInWithSlide,

    /// <summary>
    /// Slide from bottom.
    /// </summary>
    SlideBottom,

    /// <summary>
    /// Slide to top.
    /// </summary>
    SlideTop,

    /// <summary>
    /// Slide to the right side.
    /// </summary>
    SlideRight,

    /// <summary>
    /// Slide to the left side.
    /// </summary>
    SlideLeft,
}
