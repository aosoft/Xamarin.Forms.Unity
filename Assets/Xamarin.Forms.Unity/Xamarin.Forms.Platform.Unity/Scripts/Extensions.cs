using System.Collections.Specialized;

namespace Xamarin.Forms.Platform.Unity
{
    static public class Extensions
    {
        static public UnityEngine.Color ToUnityColor(this Color color)
        {
            return new UnityEngine.Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
        }

        static public UnityEngine.FontStyle ToUnityFontStyle(this FontAttributes attrs)
        {
            switch (attrs)
            {
                case FontAttributes.Bold:
                    {
                        return UnityEngine.FontStyle.Bold;
                    }

                case FontAttributes.Italic:
                    {
                        return UnityEngine.FontStyle.Italic;
                    }
            }
            return UnityEngine.FontStyle.Normal;
        }

        static public UnityEngine.HorizontalWrapMode ToUnityHorizontalWrapMode(this LineBreakMode mode)
        {
            return mode == LineBreakMode.CharacterWrap || mode == LineBreakMode.WordWrap ?
                    UnityEngine.HorizontalWrapMode.Wrap : UnityEngine.HorizontalWrapMode.Overflow;
        }

        private class EnterState
        {
            public bool Entered { get; set; }
        }

        static public IObservable<T> BlockReenter<T>(this IObservable<T> self)
        {
            return Observable.Create<T>(observer =>
            {
                var entered = new EnterState { Entered = false };
                return self.Subscribe(value =>
                {
                    //UnityEngine.Debug.Log(string.Format("BlockReenter: {0}, {1}", entered.GetHashCode(), entered.Entered));
                    if (!entered.Entered)
                    {
                        entered.Entered = true;
                        try
                        {
                            observer.OnNext(value);
                        }
                        finally
                        {
                            entered.Entered = false;
                        }
                    }
                });
            });
        }

        static public bool AddCollectionChangedEvent(this System.Collections.IEnumerable coll, NotifyCollectionChangedEventHandler handler)
        {
            var cc = coll as INotifyCollectionChanged;
            if (cc != null)
            {
                cc.CollectionChanged += handler;
                return true;
            }
            return false;
        }

        static public bool RemoveCollectionChangedEvent(this System.Collections.IEnumerable coll, NotifyCollectionChangedEventHandler handler)
        {
            var cc = coll as INotifyCollectionChanged;
            if (cc != null)
            {
                cc.CollectionChanged -= handler;
                return true;
            }
            return false;
        }
    }
}