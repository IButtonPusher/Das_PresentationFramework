using System;

namespace Das.ViewModels.Invocations
{
    public readonly struct PollyAction<TInput1, TInput2> : IPollyAction
    {
        private readonly TInput1 _input1;
        private readonly TInput2 _input2;
        private readonly Action<TInput1, TInput2> _action;

        public PollyAction(TInput1 input1, TInput2 input2, Action<TInput1, TInput2> action)
        {
            _input1 = input1;
            _input2 = input2;
            _action = action;
        }

        public void Execute()
        {
            _action(_input1, _input2);
        }
    }

    public readonly struct PollyAction<TInput1> : IPollyAction
    {
        private readonly TInput1 _input1;
        private readonly Action<TInput1> _action;

        public PollyAction(TInput1 input1, Action<TInput1> action)
        {
            _input1 = input1;
            _action = action;
        }

        public void Execute()
        {
            _action(_input1);
        }
    }

    public readonly struct PollyLateAction<TInput1, TInput2> : IPollyAction<TInput1>
    {
        private readonly TInput2 _input2;
        private readonly Action<TInput1, TInput2> _action;

        public PollyLateAction(TInput2 input2, Action<TInput1, TInput2> action)
        {
            _input2 = input2;
            _action = action;
        }

        public void Execute(TInput1 input1)
        {
            _action(input1, _input2);
        }
    }

    public readonly struct PollyLateAction<TInput1, TInput2, TInput3, TInput4> : IPollyAction<TInput1>
    {
        private readonly TInput2 _input2;
        private readonly TInput3 _input3;
        private readonly TInput4 _input4;
        private readonly Action<TInput1, TInput2, TInput3, TInput4> _action;

        public PollyLateAction(
            TInput2 input2,
            TInput3 input3,
            TInput4 input4,
            Action<TInput1, TInput2, TInput3, TInput4> action)
        {
            _input2 = input2;
            _input3 = input3;
            _input4 = input4;
            _action = action;
        }

        public void Execute(TInput1 input1)
        {
            _action(input1, _input2, _input3, _input4);
        }
    }

    public interface IPollyAction<in TInput>
    {
        void Execute(TInput input);
    }

    public interface IPollyAction
    {
        void Execute();
    }
}