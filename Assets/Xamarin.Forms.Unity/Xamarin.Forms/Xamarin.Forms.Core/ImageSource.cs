using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	[TypeConverter(typeof(ImageSourceConverter))]
	public abstract class ImageSource : Element
	{
		private readonly object _synchandle = new object();
		private CancellationTokenSource _cancellationTokenSource;

		private TaskCompletionSource<bool> _completionSource;

		private readonly WeakEventManager _weakEventManager = new WeakEventManager();

		protected ImageSource()
		{
		}

		protected CancellationTokenSource CancellationTokenSource
		{
			get { return _cancellationTokenSource; }
			private set
			{
				if (_cancellationTokenSource == value)
					return;
				if (_cancellationTokenSource != null)
					_cancellationTokenSource.Cancel();
				_cancellationTokenSource = value;
			}
		}

		private bool IsLoading
		{
			get { return _cancellationTokenSource != null; }
		}

		public virtual Task<bool> Cancel()
		{
			if (!IsLoading)
				return Task.FromResult(false);

			var tcs = new TaskCompletionSource<bool>();
			TaskCompletionSource<bool> original = Interlocked.CompareExchange(ref _completionSource, tcs, null);
			if (original == null)
			{
				_cancellationTokenSource.Cancel();
			}
			else
				tcs = original;

			return tcs.Task;
		}

		public static ImageSource FromFile(string file)
		{
			return new FileImageSource { File = file };
		}

		public static ImageSource FromResource(string resource, Type resolvingType)
		{
			return FromResource(resource, resolvingType.GetTypeInfo().Assembly);
		}

		public static ImageSource FromResource(string resource, Assembly sourceAssembly = null)
		{
			if (sourceAssembly == null)
			{
				MethodInfo callingAssemblyMethod = typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly");
				if (callingAssemblyMethod != null)
				{
					sourceAssembly = (Assembly)callingAssemblyMethod.Invoke(null, new object[0]);
				}
				else
				{
					Log.Warning("Warning", "Can not find CallingAssembly, pass resolvingType to FromResource to ensure proper resolution");
					return null;
				}
			}

			return FromStream(() => sourceAssembly.GetManifestResourceStream(resource));
		}

		public static ImageSource FromStream(Func<Stream> stream)
		{
			return new StreamImageSource { Stream = token => Task.Run(stream, token) };
		}

		public static ImageSource FromUri(Uri uri)
		{
			if (!uri.IsAbsoluteUri)
				throw new ArgumentException("uri is relative");
			return new UriImageSource { Uri = uri };
		}

		public static implicit operator ImageSource(string source)
		{
			Uri uri;
			return Uri.TryCreate(source, UriKind.Absolute, out uri) && uri.Scheme != "file" ? FromUri(uri) : FromFile(source);
		}

		public static implicit operator ImageSource(Uri uri)
		{
			if (!uri.IsAbsoluteUri)
				throw new ArgumentException("uri is relative");
			return FromUri(uri);
		}

		protected void OnLoadingCompleted(bool cancelled)
		{
			if (!IsLoading || _completionSource == null)
				return;

			TaskCompletionSource<bool> tcs = Interlocked.Exchange(ref _completionSource, null);
			if (tcs != null)
				tcs.SetResult(cancelled);

			lock (_synchandle)
			{
				CancellationTokenSource = null;
			}
		}

		protected void OnLoadingStarted()
		{
			lock (_synchandle)
			{
				CancellationTokenSource = new CancellationTokenSource();
			}
		}

		protected void OnSourceChanged()
		{
			_weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(SourceChanged));
		}

		internal event EventHandler SourceChanged
		{
			add { _weakEventManager.AddEventHandler(nameof(SourceChanged), value); }
			remove { _weakEventManager.RemoveEventHandler(nameof(SourceChanged), value); }
		}
	}
}