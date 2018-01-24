namespace Xamarin.Forms
{
	public interface IItemsView<T> where T : BindableObject
	{
		T CreateDefault(object item);

		void SetupContent(T content, int index);

		void UnhookContent(T content);
	}
}