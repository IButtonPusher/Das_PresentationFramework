using System;
using Das.Views;
using Das.Views.Controls;
using Das.Views.DataBinding;

namespace XamarinAndroidTest
{
    public sealed class TestView: Das.Views.Panels.View
    {
        public TestView(IVisualBootstrapper visualBootstrapper)
        : base(visualBootstrapper)
        {
            var streetIntro = typeof(TestVm);

            var playerCountBinding = new DeferredPropertyBinding<String>
                (streetIntro, nameof(TestVm.Name));
            var lblPlayersCount = new Label(visualBootstrapper);
            lblPlayersCount.AddBinding(playerCountBinding);
            Content = lblPlayersCount;
        }
    }
}