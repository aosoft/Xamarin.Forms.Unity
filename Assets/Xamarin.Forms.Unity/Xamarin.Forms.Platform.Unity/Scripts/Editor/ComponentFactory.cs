using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
    public static class ComponentFactory
    {
        [UnityEditor.MenuItem("GameObject/Xamarin.Forms Application Activity(User defined)", priority = 21)]
        private static void CreateFormsApplicationActivity()
        {
            var assem = typeof(UnityFormsApplicationActivity).Assembly;
            foreach (var t in assem.GetTypes())
            {
                if (t.IsAbstract == false && t.IsGenericType == false &&
                    t.IsSubclassOf(typeof(UnityFormsApplicationActivity)))
                {
                    var go = new GameObject();
                    go.name = string.Format("Xamarin.Forms Activity({0})", t.Name);
                    go.AddComponent(t);

                    return;
                }
            }
        }
    }
}