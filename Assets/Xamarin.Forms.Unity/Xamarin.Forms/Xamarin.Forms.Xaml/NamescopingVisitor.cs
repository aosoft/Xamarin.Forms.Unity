using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Xaml
{
	internal class NamescopingVisitor : IXamlNodeVisitor
	{
		private readonly Dictionary<INode, INameScope> scopes = new Dictionary<INode, INameScope>();

		public NamescopingVisitor(HydrationContext context)
		{
			Values = context.Values;
		}

		private Dictionary<INode, object> Values { get; set; }

		public TreeVisitingMode VisitingMode => TreeVisitingMode.TopDown;
		public bool StopOnDataTemplate => false;
		public bool StopOnResourceDictionary => false;
		public bool VisitNodeOnDataTemplate => true;

		public void Visit(ValueNode node, INode parentNode)
		{
			scopes[node] = scopes[parentNode];
		}

		public void Visit(MarkupNode node, INode parentNode)
		{
			scopes[node] = scopes[parentNode];
		}

		public void Visit(ElementNode node, INode parentNode)
		{
			var ns = parentNode == null || IsDataTemplate(node, parentNode) || IsStyle(node, parentNode)
				? new NameScope()
				: scopes[parentNode];
			node.Namescope = ns;
			scopes[node] = ns;
		}

		public void Visit(RootNode node, INode parentNode)
		{
			var ns = new NameScope();
			node.Namescope = ns;
			scopes[node] = ns;
		}

		public void Visit(ListNode node, INode parentNode)
		{
			scopes[node] = scopes[parentNode];
		}

		private static bool IsDataTemplate(INode node, INode parentNode)
		{
			var parentElement = parentNode as IElementNode;
			INode createContent;
			if (parentElement != null && parentElement.Properties.TryGetValue(XmlName._CreateContent, out createContent) &&
				createContent == node)
				return true;
			return false;
		}

		private static bool IsStyle(INode node, INode parentNode)
		{
			var pnode = parentNode as ElementNode;
			return pnode != null && pnode.XmlType.Name == "Style";
		}
	}
}