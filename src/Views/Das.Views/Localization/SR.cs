using System;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Das.Views.Localization
{
    public static class SR
    {
        public static String Get(String name)
        {
            return GetResourceString(name, null);
        }

        public static String Get(String name,
                                 params Object[] args)
        {
            return Format(GetResourceString(name, null), args);
        }

        public static String GetResourceString(String resourceKey,
                                               String defaultString)
        {
            String? resourceString = null;
            try { resourceString = ResourceManager.GetString(resourceKey); }
            catch (MissingManifestResourceException) { }

            if (defaultString != null && resourceKey.Equals(resourceString, StringComparison.Ordinal))
            {
                return defaultString;
            }

            return resourceString;
        }

        public static String Format(String resourceFormat,
                                    params Object[] args)
        {
            if (args != null)
            {
                if (UsingResourceKeys())
                {
                    return resourceFormat + String.Join(", ", args);
                }

                return String.Format(resourceFormat, args);
            }

            return resourceFormat;
        }

        public static String Format(String resourceFormat,
                                    Object p1)
        {
            if (UsingResourceKeys())
            {
                return String.Join(", ", resourceFormat, p1);
            }

            return String.Format(resourceFormat, p1);
        }

        public static String Format(String resourceFormat,
                                    Object p1,
                                    Object p2)
        {
            if (UsingResourceKeys())
            {
                return String.Join(", ", resourceFormat, p1, p2);
            }

            return String.Format(resourceFormat, p1, p2);
        }

        public static String Format(String resourceFormat,
                                    Object p1,
                                    Object p2,
                                    Object p3)
        {
            if (UsingResourceKeys())
            {
                return String.Join(", ", resourceFormat, p1, p2, p3);
            }

            return String.Format(resourceFormat, p1, p2, p3);
        }

        // This method is used to decide if we need to append the exception message parameters to the message when calling SR.Format.
        // by default it returns false.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Boolean UsingResourceKeys()
        {
            return false;
        }

        private static ResourceManager ResourceManager => SRID.ResourceManager;
        //private static ResourceManager ResourceManager { get; } //=> ResourceManager;
    }
}
