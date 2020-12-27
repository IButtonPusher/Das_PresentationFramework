using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles.Selectors
{
    public class OrStyleSelector
    {
        public OrStyleSelector(IEnumerable<IStyleSelector> selectors)
        {
            _selectors = new List<IStyleSelector>(selectors);
        }

        private readonly List<IStyleSelector> _selectors;
    }
}