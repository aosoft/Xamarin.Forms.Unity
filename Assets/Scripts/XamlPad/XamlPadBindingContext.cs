using UnityEngine;
using UniRx;
using System.Windows.Input;
using System.ComponentModel;
using System;

namespace XamlPad
{
	public class XamlPadBindingContext
	{
		class InternalReactiveCommand : ReactiveCommand, ICommand
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

		class InternalReactiveProperty<T> : ReactiveProperty<T>, INotifyPropertyChanged
		{
			public InternalReactiveProperty(MonoBehaviour disposer)
			{
				this.Subscribe(_ => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null))).AddTo(disposer);
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}

		public XamlPadBindingContext(MonoBehaviour disposer)
		{
			XamlSource = new InternalReactiveProperty<string>(disposer);
			CompileResult = new InternalReactiveProperty<string>(disposer);
			RootPage = new InternalReactiveProperty<RootPage>(disposer);

			var cmd = new InternalReactiveCommand(disposer);
			cmd.Subscribe(_ =>
			{
				try
				{
					var page = new RootPage();
					Xamarin.Forms.Platform.Unity.XamlLoader.LoadXaml(page, XamlSource.Value);
					RootPage.Value = page;
				}
				catch (Exception e)
				{
					CompileResult.Value = e.Message;
				}

			});
			CompileCommand = cmd;
		}

		public ReactiveCommand CompileCommand
		{
			get;
		}

		public ReactiveProperty<string> XamlSource
		{
			get;
		}

		public ReactiveProperty<string> CompileResult
		{
			get;
		}

		public ReactiveProperty<RootPage> RootPage
		{
			get;
		}
	}

}