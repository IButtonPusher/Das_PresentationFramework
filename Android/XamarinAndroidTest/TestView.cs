using System;
using Das.Views.Controls;
using Das.Views.DataBinding;

namespace XamarinAndroidTest
{
    public sealed class TestView: Das.Views.Panels.View<TestVm>
    {
        public TestView()
        {
            var streetIntro = typeof(TestVm);

            var playerCountBinding = new DeferredPropertyBinding<String>
                (streetIntro, nameof(TestVm.Name));
            var lblPlayersCount = new Label(playerCountBinding);
            Content = lblPlayersCount;
        }
    }
}