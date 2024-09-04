// MIT License
//
// Copyright (c) 2024 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace SirRandoo.ToolkitRaids.UX;

/// <summary>
///     A set of constants that are used throughout the mod's menus.
/// </summary>
public static class UiConstants
{
    /// <summary>
    ///     The line height of all content within the mod's menus.
    /// </summary>
    /// <remarks>
    ///     The mod intentionally uses a slightly higher line height than RimWorld's to create less
    ///     visually dense menus as they will often contain a lot of information that needs to be digested.
    /// </remarks>
    public const float LineHeight = 28f;

    /// <summary>
    ///     The line height of all tabs within the mod's menus.
    /// </summary>
    public const float TabHeight = 35f;
}
