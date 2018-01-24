using System;
using System.ComponentModel;
using System.Windows.Input;
using UnityEngine;

public class SampleBindingContext
{
    private class InternalReactiveCommand : ReactiveCommand, ICommand
    {
        public InternalReactiveCommand(MonoBehaviour disposer)
        {
            CanExecute.Subscribe(_ => CanExecuteChanged?.Invoke(this, EventArgs.Empty)).AddTo(disposer);
        }

        public event EventHandler CanExecuteChanged;

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute.Value;
        }
    }

    private class InternalReactiveProperty<T> : ReactiveProperty<T>, INotifyPropertyChanged
    {
        public InternalReactiveProperty(MonoBehaviour disposer)
        {
            this.Subscribe(_ => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null))).AddTo(disposer);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    private System.Random _rnd = new System.Random();

    public SampleBindingContext(MonoBehaviour disposer)
    {
        var cmd = new InternalReactiveCommand(disposer);
        cmd.Subscribe(_ =>
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UnityEngine.Object.Destroy(go, 5.0f);

            go.transform.position = new Vector3((float)(_rnd.NextDouble() * 2.0 - 1.0), 5.0f, (float)(_rnd.NextDouble() * 2.0 - 1.0));

            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = true;

            Counter.Value++;

            DoubleValue.Value = _rnd.NextDouble();
        }).AddTo(disposer);
        InstantiateCommand = cmd;

        Counter = new InternalReactiveProperty<int>(disposer);
        DoubleValue = new InternalReactiveProperty<double>(disposer);
    }

    public ReactiveCommand InstantiateCommand
    {
        get;
    }

    public ReactiveProperty<int> Counter
    {
        get;
    }

    public ReactiveProperty<double> DoubleValue
    {
        get;
    }
}