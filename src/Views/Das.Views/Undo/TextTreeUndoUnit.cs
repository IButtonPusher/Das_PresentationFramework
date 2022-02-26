using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Das.Views.Collections;
using Das.Views.Data;
using Das.Views.DataBinding;
using Das.Views.DependencyProperties;
using Das.Views.Input.Text;
using Das.Views.Validation;

namespace Das.Views.Undo
{
    public abstract class TextTreeUndoUnit : IUndoUnit
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------


        // Create a new undo unit instance.
        public TextTreeUndoUnit(TextContainer tree,
                                Int32 symbolOffset)
        {
            _tree = tree;
            _symbolOffset = symbolOffset;
            _treeContentHashCode = _tree.GetContentHashCode();
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------


        // Called by the undo manager.  Restores tree state to its condition
        // when the unit was created.  Assumes the tree state matches conditions
        // just after the unit was created.
        public void Do()
        {
            _tree.BeginChange();
            try
            {
                DoCore();
            }
            finally
            {
                _tree.EndChange();
            }
        }

        // Called by the undo manager.  TextContainer undo units never merge.
        public Boolean Merge(IUndoUnit unit)
        {
            Invariant.Assert(unit != null);
            return false;
        }

        // Worker for Do method, implemented by derived class.
        public abstract void DoCore();


        //------------------------------------------------------
        //
        //  public Methods
        //
        //------------------------------------------------------


        // Explicitly sets this undo unit's content hash code to match the
        // current tree state.  This happens automatically in the ctor, but
        // some undo units (for delte operations) need to be initialized before
        // the content is modified, in which case they call this method
        // afterwards.
        public void SetTreeHashCode()
        {
            _treeContentHashCode = _tree.GetContentHashCode();
        }

        // Verifies the TextContainer state matches the original state when this undo
        // unit was created.  Because we use symbol offsets to track the
        // position of the content to modify, errors in the undo code or reentrant
        // document edits by random code could potentially
        // corrupt data rather than raise an immediate exception.
        //
        // This method uses Invariant.Assert to trigger a FailFast in the case an error
        // is detected, before we get a chance to corrupt data.
        public void VerifyTreeContentHashCode()
        {
            if (_tree.GetContentHashCode() != _treeContentHashCode)
            {
                // Data is irrecoverably corrupted, shut down!
                Invariant.Assert(false, "Undo unit is out of sync with TextContainer!");
            }
        }

        // Gets an array of PropertyValues from a IBindableElement's LocalValueEnumerator.
        // The array is safe to cache, LocalValueEnumerators are not.
        public static PropertyValue[] GetPropertyValueArray(IBindableElement d)
        {
            //var count = 0;
            //PropertyValue[] records = new PropertyValue[valuesEnumerator.Count];
            var records = new List<PropertyValue>();

            foreach (var current in d.GetLocalValues())
            {
                var dp = current.Property;
                if (!dp.IsReadOnly)
                {
                    // LocalValueEntry.Value can be an Expression, which we can't duplicate when we 
                    // undo, so we copy over the current value from IBindableElement.GetValue instead.

                    var record = new PropertyValue
                    {
                        Property = dp,
                        Value = d.GetValue(dp)
                    };

                    records.Add(record);
                    //records[count].Property = dp;
                    //records[count].Value = d.GetValue(dp);

                    //count++;
                }
            }

            return records.ToArray();


            //LocalValueEnumerator valuesEnumerator = d.GetLocalValueEnumerator();


            //valuesEnumerator.Reset();
            //while (valuesEnumerator.MoveNext())
            //{
            //    var dp = valuesEnumerator.Current.Property;
            //    if (!dp.IsReadOnly)
            //    {
            //        // LocalValueEntry.Value can be an Expression, which we can't duplicate when we 
            //        // undo, so we copy over the current value from IBindableElement.GetValue instead.
            //        records[count].Property = dp;
            //        records[count].Value = d.GetValue(dp);

            //        count++;
            //    }
            //}

            //PropertyValue[] trimmedResult;
            //if (valuesEnumerator.Count != count)
            //{
            //    trimmedResult = new PropertyValue[count];
            //    for (var i = 0; i < count; i++)
            //    {
            //        trimmedResult[i] = records[i];
            //    }
            //}
            //else
            //{
            //    trimmedResult = records;
            //}

            //return trimmedResult;
        }

        // Converts array of PropertyValues into a LocalValueEnumerator.
        // The array is safe to cache, LocalValueEnumerators are not.
        public static LocalValueEnumerator ArrayToLocalValueEnumerator(PropertyValue[] records)
        {
            var count = records.Length;
            var snapshot = new LocalValueEntry[count];

            for (var c = 0; c < records.Length; c++)
            {
                snapshot[c] = new LocalValueEntry(records[c].Property, records[c].Value);
            }

            return new LocalValueEnumerator(snapshot, count);


            //IBindableElement obj;
            //Int32 i;

            //obj = new BindableElement();

            //for (i = 0; i < records.Length; i++)
            //{
            //obj.SetValue(records[i].Property, records[i].Value);
            //}

            //return obj.GetLocalValueEnumerator();
        }


        //------------------------------------------------------
        //
        //  Protected Properties
        //
        //------------------------------------------------------


        // TextContainer associated with this undo unit.
        protected TextContainer TextContainer => _tree;

        // Offset in symbols of this undo unit within the TextContainer content.
        protected Int32 SymbolOffset => _symbolOffset;


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // The tree associated with this undo unit.
        private readonly TextContainer _tree;

        // Offset in the tree at which to undo.
        private readonly Int32 _symbolOffset;

        // Hash representing the state of the tree when the undo unit was
        // created.  If the hash doesn't match when Do is called, there's a bug
        // somewhere, and any TextContainer undo units on the stack are probably
        // corrupted.
        private Int32 _treeContentHashCode;

        #endregion Private Fields
    }
}
