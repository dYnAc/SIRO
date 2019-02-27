
namespace SIRO.Core.Helpers
{
    using System.Xml;

    /// <summary>
    /// The xml utility.
    /// </summary>
    internal static class XmlUtil
    {
        /// <summary>
        /// The get attribute.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetAttribute(this XmlElement node, string name)
        {
            return node.GetAttribute(name);
        }
    }
}