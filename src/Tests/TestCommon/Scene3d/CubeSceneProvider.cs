using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Das.Views;
using Das.Views.Extended;
using Das.Views.Extended.Runtime;
using TestCommon.Scene3d;

namespace TestCommon
{
    public class CubeSceneProvider
    {
        private readonly IViewProvider _viewProvider;

        public CubeSceneProvider(IViewProvider viewProvider)
        {
            _viewProvider = viewProvider;
        }

        public ISceneViewModel BuildViewModel()
        {
            var cube = new TestCube();
            
            var scene = new CoreScene(new List<IMesh> { cube });
            var vm = new TestSceneVm(scene);
            
            return vm;
        }

        public async Task<IVisualElement> GetView()
        {
            var file = new FileInfo("Scene3d\\3dTest.json");
            var view = await _viewProvider.GetView(file);
            return view;
        }

    }
}
