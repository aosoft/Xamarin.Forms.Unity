using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	internal class UnityIsolatedStorage : IIsolatedStorageFile
	{
		public UnityIsolatedStorage()
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

		public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
		{
			return Task.Run<Stream>(() =>
			{
				System.IO.FileMode mode2;
				System.IO.FileAccess access2;

				switch (mode)
				{
					case FileMode.CreateNew:
						mode2 = System.IO.FileMode.CreateNew;
						break;

					case FileMode.Create:
						mode2 = System.IO.FileMode.Create;
						break;

					case FileMode.Truncate:
						mode2 = System.IO.FileMode.Truncate;
						break;

					case FileMode.OpenOrCreate:
						mode2 = System.IO.FileMode.OpenOrCreate;
						break;

					case FileMode.Append:
						mode2 = System.IO.FileMode.Append;
						break;

					case FileMode.Open:
						mode2 = System.IO.FileMode.Open;
						break;

					default:
						throw new ArgumentException("mode was an invalid FileMode", "mode");
				}

				switch (access)
				{
					case FileAccess.Read:
						access2 = System.IO.FileAccess.Read;
						break;
					case FileAccess.Write:
						access2 = System.IO.FileAccess.Write;
						break;

					case FileAccess.ReadWrite:
						access2 = System.IO.FileAccess.ReadWrite;
						break;

					default:
						throw new ArgumentException("access was an invalid FileAccess", "access");
				}

				return new FileStream(path, mode2, access2);
			});
		}

		public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return OpenFileAsync(path, mode, access);
		}
	}
}