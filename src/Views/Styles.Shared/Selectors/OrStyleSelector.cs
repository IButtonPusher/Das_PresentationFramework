using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    // ReSharper disable once UnusedType.Global
    public class OrStyleSelector
    {
        public OrStyleSelector(IEnumerable<IStyleSelector> selectors)
        {
            _selectors = new List<IStyleSelector>(selectors);
        }

        // ReSharper disable once NotAccessedField.Local
        private readonly List<IStyleSelector> _selectors;
    }
}