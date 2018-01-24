using System;
using System.ComponentModel;
using System.Windows.Input;
using UnityEngine;

namespace XamlPad
{
    public class XamlPadBindingContext
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

        private class InternalReadOnlyReactiveProperty<T> : ReadOnlyReactiveProperty<T>, INotifyPropertyChanged
        {
            public InternalReadOnlyReactiveProperty(UniRx.IObservable<T> source, MonoBehaviour disposer) : base(source)
            {
                this.Subscribe(_ => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null))).AddTo(disposer);
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public XamlPadBindingContext(MonoBehaviour disposer)
        {
            AutoParse = new InternalReactiveProperty<bool>(disposer);
            XamlSource = new InternalReactiveProperty<string>(disposer);
            CompileResult = new InternalReactiveProperty<string>(disposer);
            FontSizeSelectedIndex = new InternalReactiveProperty<int>(disposer);
            _fontSize = new InternalReactiveProperty<double>(disposer);
            FontSize = new InternalReadOnlyReactiveProperty<double>(_fontSize, disposer);
            RootPage = new InternalReactiveProperty<Xamarin.Forms.View>(disposer);

            var cmd = new InternalReactiveCommand(disposer);
            cmd.Subscribe(_ =>
            {
                try
                {
                    var page = new Xamarin.Forms.Grid();
                    Xamarin.Forms.Platform.Unity.XamlLoader.LoadXaml(page, XamlSource.Value);
                    RootPage.Value = page;
                    CompileResult.Value = "Success!";
                }
                catch (Exception e)
                {
                    while (e.InnerException != null)
                    {
                        e = e.InnerException;
                    }
                    CompileResult.Value = e.Message;
                }
            });
            CompileCommand = cmd;

            FontSizeSelectedIndex.Subscribe(value =>
            {
                value = Math.Max(Math.Min(value, FontSizeList.Length - 1), 0);
                _fontSize.Value = double.Parse(FontSizeList[value]);
            });

            IDisposable o = null;
            AutoParse.Subscribe(value =>
            {
                o?.Dispose();
                if (value)
                {
                    o = XamlSource
                        .Throttle(new TimeSpan(TimeSpan.TicksPerSecond))
                        .ObserveOnMainThread()
                        .Subscribe(_ => cmd.Execute());
                }
            });
        }

        /// <summary>
        /// プロパティ値を初期化する。
        /// BindingContext に設定後に呼び出す。
        /// </summary>
        public void InitializePropertyValues()
        {
            XamlSource.Value = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<Grid\n  xmlns=\"http://xamarin.com/schemas/2014/forms\"\n  xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\">\n</Grid>";
            FontSizeSelectedIndex.Value = 3;
            AutoParse.Value = false;
        }

        public ReactiveCommand CompileCommand
        {
            get;
        }

        public ReactiveProperty<bool> AutoParse
        {
            get;
        }

        public ReactiveProperty<int> FontSizeSelectedIndex
        {
            get;
        }

        public ReadOnlyReactiveProperty<double> FontSize
        {
            get;
        }

        private ReactiveProperty<double> _fontSize;

        public string[] FontSizeList
        {
            get;
        } = new string[] { "12", "14", "16", "18", "20", "22" };

        public ReactiveProperty<string> XamlSource
        {
            get;
        }

        public ReactiveProperty<string> CompileResult
        {
            get;
        }

        public ReactiveProperty<Xamarin.Forms.View> RootPage
        {
            get;
        }
    }
}