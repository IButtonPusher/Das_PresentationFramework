using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;

namespace Das.Views.Text
{
    public static class TextElementCollectionHelper
    {
        // Invalidates any collection tracking parent's children.
        // Called by the TextContainer.
        public static void MarkDirty(IBindableElement parent)
        {
            if (parent == null)
            {
                return;
            }

            lock (_cleanParentList)
            {
                for (var i = 0; i < _cleanParentList.Length; i++)
                {
                    if (_cleanParentList[i] != null)
                    {
                        var pair = (ParentCollectionPair)_cleanParentList[i].Target;

                        if (pair == null || pair.Parent == parent)
                        {
                            _cleanParentList[i] = null;
                        }
                    }
                }
            }
        }

        public static void MarkClean(IBindableElement parent,
                                     Object collection)
        {
            lock (_cleanParentList)
            {
                Int32 firstFreeIndex;
                var index = GetCleanParentIndex(parent, collection, out firstFreeIndex);

                if (index == -1)
                {
                    index = firstFreeIndex >= 0 ? firstFreeIndex : _cleanParentList.Length - 1;
                    _cleanParentList[index] = new WeakReference(new ParentCollectionPair(parent, collection));
                }

                TouchCleanParent(index);
            }
        }

        private static Int32 GetCleanParentIndex(IBindableElement parent,
                                                 Object collection,
                                                 out Int32 firstFreeIndex)
        {
            var index = -1;

            firstFreeIndex = -1;

            for (var i = 0; i < _cleanParentList.Length; i++)
            {
                if (_cleanParentList[i] == null)
                {
                    if (firstFreeIndex == -1)
                    {
                        firstFreeIndex = i;
                    }
                }
                else
                {
                    var pair = (ParentCollectionPair)_cleanParentList[i].Target;

                    if (pair == null)
                    {
                        // WeakReference is dead, remove it.
                        _cleanParentList[i] = null;
                        if (firstFreeIndex == -1)
                        {
                            firstFreeIndex = i;
                        }
                    }
                    else if (pair.Parent == parent && pair.Collection == collection)
                    {
                        // Found a match.  Keep going to clean up any dead WeakReferences
                        // or set firstFreeIndex.
                        index = i;
                    }
                }
            }

            return index;
        }

        private static void TouchCleanParent(Int32 index)
        {
            var parentReference = _cleanParentList[index];
            // Shift preceding parents down, dropping the last parent.
            Array.Copy(_cleanParentList, 0, _cleanParentList, 1, index);
            // Put the mru parent at the head.
            _cleanParentList[0] = parentReference;
        }

        // Static list of clean parent/collection pairs.
        private static readonly WeakReference[] _cleanParentList = new WeakReference[10];

        private class ParentCollectionPair
        {
            internal ParentCollectionPair(IBindableElement parent,
                                          Object collection)
            {
                _parent = parent;
                _collection = collection;
            }

            internal IBindableElement Parent => _parent;

            internal Object Collection => _collection;

            private readonly Object _collection;

            private readonly IBindableElement _parent;
        }
    }
}
