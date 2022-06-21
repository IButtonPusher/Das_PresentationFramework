using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Das.Views.Data
{
    public class NamedObject
    {
        public NamedObject(String name)
        {
            _name = !String.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(name);
        }

        public override String ToString()
        {
            if (_name[0] != '{')
                _name = String.Format(CultureInfo.InvariantCulture, "{{{0}}}", _name);
            return _name;
        }

        private String _name;
    }
}
