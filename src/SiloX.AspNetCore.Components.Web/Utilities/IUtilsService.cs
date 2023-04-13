using JetBrains.Annotations;

namespace SiloX.AspNetCore.Components.Web.Utilities;

/// <summary>
///     A service that provides utility methods for the browser.
/// </summary>
[PublicAPI]
public interface IUtilsService
{
    /// <summary>
    ///     Adds a class to the tag with the given name.
    /// </summary>
    /// <param name="tagName">The name of the tag to add the class to.</param>
    /// <param name="className">The name of the class to add to the tag.</param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    ValueTask AddClassToTagAsync(string tagName, string className);

    /// <summary>
    ///     Removes a class from the tag with the given name.
    /// </summary>
    /// <param name="tagName">The name of the tag to add the class to.</param>
    /// <param name="className">The name of the class to add to the tag.</param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    ValueTask RemoveClassFromTagAsync(string tagName, string className);

    /// <summary>
    ///     Checks if the tag with the given name has the given class.
    /// </summary>
    /// <param name="tagName">The name of the tag to add the class to.</param>
    /// <param name="className">The name of the class to add to the tag.</param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    ValueTask<bool> HasClassOnTagAsync(string tagName, string className);

    /// <summary>
    ///     Replaces the href value of the link with the given id.
    /// </summary>
    /// <param name="linkId">The id of the link to replace the href value of.</param>
    /// <param name="hrefValue">The new href value to replace the old one with.</param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    ValueTask ReplaceLinkHrefByIdAsync(string linkId, string hrefValue);

    /// <summary>
    ///     Toggles the fullscreen mode of the browser.
    /// </summary>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    ValueTask ToggleFullscreenAsync();

    /// <summary>
    ///     Requests the browser to enter fullscreen mode.
    /// </summary>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    ValueTask RequestFullscreenAsync();

    /// <summary>
    ///     Requests the browser to exit fullscreen mode.
    /// </summary>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    ValueTask ExitFullscreenAsync();
}