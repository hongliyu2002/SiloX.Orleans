using JetBrains.Annotations;

namespace SiloX.AspNetCore.UI.LayoutHooks;

/// <summary>
/// </summary>
[PublicAPI]
public static class LayoutHooks
{
    /// <summary>
    /// </summary>
    public static class Header
    {
        /// <summary>
        /// </summary>
        public const string First = "Header.First";
        
        /// <summary>
        /// </summary>
        public const string Last = "Header.Last";
    }

    /// <summary>
    /// </summary>
    public static class Body
    {
        /// <summary>
        /// </summary>
        public const string First = "Body.First";
        
        /// <summary>
        /// </summary>
        public const string Last = "Body.Last";
    }

    /// <summary>
    /// </summary>
    public static class PageContent
    {
        /// <summary>
        /// </summary>
        public const string First = "PageContent.First";
        
        /// <summary>
        /// </summary>
        public const string Last = "PageContent.Last";
    }
}