using UnityEngine;
using UniRx;
using System.Windows.Input;
using System.ComponentModel;
using System;

public class InternalReactiveCommand : ReactiveCommand, ICommand
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

public class InternalReactiveProperty<T> : ReactiveProperty<T>, INotifyPropertyChanged
{
	public InternalReactiveProperty(MonoBehaviour disposer)
	{
		this.Subscribe(_ => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null))).AddTo(disposer);
	}

	public event PropertyChangedEventHandler PropertyChanged;
}

public class SampleBindingContext
{
	public SampleBindingContext(MonoBehaviour disposer)
	{
		var cmd = new InternalReactiveCommand(disposer);
		cmd.Subscribe(_ =>
		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			UnityEngine.Object.Destroy(go, 5.0f);

			Counter.Value++;
		}).AddTo(disposer);
		InstantiateCommand = cmd;

		Counter = new InternalReactiveProperty<int>(disposer);
	}


	public ReactiveCommand InstantiateCommand
	{
		get;
	}

	public ReactiveProperty<int> Counter
	{
		get;
	}
}

