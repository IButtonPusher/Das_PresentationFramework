using System;
using System.Collections.Generic;
using System.Text;
using Das.Views.Resources;

namespace Das.Views.Text
{
    public interface IResource
    {
        ResourceHandle AddRefOnChannel(Channel channel);

        int GetChannelCount();

        Channel GetChannel(int index);

        void ReleaseOnChannel(Channel channel);

        ResourceHandle GetHandle(Channel channel);

        /// <summary>
        /// Only Vieport3DVisual and Visual3D implement this.
        /// Vieport3DVisual has two handles. One stored in _proxy
        /// and the other one stored in _proxy3D. This function returns
        /// the handle stored in _proxy3D.
        /// </summary>
        ResourceHandle Get3DHandle(Channel channel);

        /// <summary>
        /// Sends a command to compositor to remove the child
        /// from its parent on the channel.
        /// </summary>
        void RemoveChildFromParent(
            IResource parent,
            Channel channel);
    }
}
