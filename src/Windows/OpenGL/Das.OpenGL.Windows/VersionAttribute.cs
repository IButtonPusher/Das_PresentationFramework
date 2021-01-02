using System;
using System.Linq;

namespace Das.OpenGL.Windows
{
    [AttributeUsage(AttributeTargets.Field)]
    public class VersionAttribute : Attribute
    {
        private readonly Int32 _major;
        private readonly Int32 _minor;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionAttribute"/> class.
        /// </summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        public VersionAttribute(Int32 major, Int32 minor)
        {
            _major = major;
            _minor = minor;
        }

        /// <summary>
        /// Determines whether this version is at least as high as the version specified in the parameters.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <returns>True if this version object is at least as high as the version specified by <paramref name="major"/> and <paramref name="minor"/>.</returns>
        public Boolean IsAtLeastVersion(Int32 major, Int32 minor)
        {
            //  If major versions match, we care about minor. Otherwise, we only care about major.
            if (_major == major)
                return _major >= major && _minor >= minor;
            return _major >= major;
        }

        /// <summary>
        /// Gets the version attribute of an enumeration value <paramref name="enumeration"/>.
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>The <see cref="VersionAttribute"/> defined on <paramref name="enumeration "/>, or null of none exists.</returns>
        public static VersionAttribute GetVersionAttribute(Enum enumeration) =>
            // ReSharper disable once AssignNullToNotNullAttribute
            enumeration
                .GetType()
                .GetMember(enumeration.ToString())
                .Single()
                .GetCustomAttributes(typeof(VersionAttribute), false)
                .OfType<VersionAttribute>()
                .FirstOrDefault();

        /// <summary>
        /// Gets the major version number.
        /// </summary>
        public Int32 Major => _major;

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        public Int32 Minor => _minor;
    }
}
