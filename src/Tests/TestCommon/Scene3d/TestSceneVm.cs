using System;
using Das.ViewModels;
using Das.Views.Extended;
using Das.Views.Extended.Runtime;

namespace TestCommon.Scene3d
{
    public class TestSceneVm : BaseViewModel,
                               ISceneViewModel
    {
        private readonly IScene _scene;
        public ICamera Camera { get; }
        public void Update()
        {
            foreach (var element in _scene.VisualElements)
                element.Rotate(0.025f, 0.025f, 0);

            IsChanged = true;
        }

        public TestSceneVm(IScene scene)
        {
            _scene = scene;
            Camera = new WireframeCamera(new Vector3(0, 0, 10.0f), Vector3.Zero, Vector3.Zero, scene);
        }

        //public void AcceptChanges()
        //{
        //    IsChanged = false;
        //}

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Boolean IsChanged { get; private set; }
    }
}
