using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.WinForms
{
	internal class WinFormsIsolatedStorage : IIsolatedStorageFile
	{
		public WinFormsIsolatedStorage()
		{
		}

		public Task CreateDirectoryAsync(string path)
		{
			return Task.Run(() => Directory.CreateDirectory(path));
		}

		public Task<bool> GetDirectoryExistsAsync(string path)
		{
			return Task.Run<bool>(() => Directory.Exists(path));
		}

		public Task<bool> GetFileExistsAsync(string path)
		{
			return Task.Run<bool>(() => File.Exists(path));
		}

		public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
		{
			return Task.Run<DateTimeOffset>(() => new DateTimeOffset(File.GetLastWriteTime(path)));
		}

		public Task<Stream> OpenFileAsync(string path, Internals.FileMode mode, Internals.FileAccess access)
		{
			return Task.Run<Stream>(() =>
			{
				System.IO.FileMode mode2;
				System.IO.FileAccess access2;

				switch (mode)
				{
					case Internals.FileMode.CreateNew:
						mode2 = System.IO.FileMode.CreateNew;
						break;

					case Internals.FileMode.Create:
						mode2 = System.IO.FileMode.Create;
						break;

					case Internals.FileMode.Truncate:
						mode2 = System.IO.FileMode.Truncate;
						break;

					case Internals.FileMode.OpenOrCreate:
						mode2 = System.IO.FileMode.OpenOrCreate;
						break;

					case Internals.FileMode.Append:
						mode2 = System.IO.FileMode.Append;
						break;

					case Internals.FileMode.Open:
						mode2 = System.IO.FileMode.Open;
						break;

					default:
						throw new ArgumentException("mode was an invalid FileMode", "mode");
				}

				switch (access)
				{
					case Internals.FileAccess.Read:
						access2 = System.IO.FileAccess.Read;
						break;
					case Internals.FileAccess.Write:
						access2 = System.IO.FileAccess.Write;
						break;

					case Internals.FileAccess.ReadWrite:
						access2 = System.IO.FileAccess.ReadWrite;
						break;

					default:
						throw new ArgumentException("access was an invalid FileAccess", "access");
				}

				return new FileStream(path, mode2, access2);
			});
		}

		public Task<Stream> OpenFileAsync(string path, Internals.FileMode mode, Internals.FileAccess access, Internals.FileShare share)
		{
			return OpenFileAsync(path, mode, access);
		}
	}
}