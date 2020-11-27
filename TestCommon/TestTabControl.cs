using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.DataBinding;
using Das.Views.ItemsControls;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace TestCommon
{
    public sealed class TestTabControl : View<TestCompanyVm>
    {
        public TestTabControl(IVisualBootStrapper templateResolver)

        : base(templateResolver)
        {
            _tabControl= new TabControl(templateResolver);
            Content = _tabControl;
        }

        public override void Arrange(IRenderSize availableSpace, IRenderContext renderContext)
        {
            Debug.WriteLine("arrange test");
            base.Arrange(availableSpace, renderContext);
        }

        public override void SetDataContext(TestCompanyVm dataContext)
        {
            base.SetDataContext(dataContext);

            _tabControl.ItemsSource = dataContext.Employees;
            var selectionBinding = new TwoWayBinding(dataContext, nameof(TestCompanyVm.SelectedEmployee),
                _tabControl, nameof(TabControl.SelectedItem));
            _tabControl.AddBinding(selectionBinding);
            //UpdateSelection().ConfigureAwait(false);
        }

        private async Task UpdateSelection()
        {
            var idx = 0;

            while (true)
            {
                await Task.Delay(2000);

                idx++;
                if (idx >= DataContext.Employees.Count)
                    idx = 0;

                DataContext.SelectedEmployee = DataContext.Employees[idx];
            }
        }

        //public override Boolean IsChanged
        //{
        //    get => IsRequiresMeasure || IsRequiresArrange;
        //    //protected set => base.IsChanged = value;
        //}

        private readonly TabControl _tabControl;
    }
}
