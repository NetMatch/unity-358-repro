using System.Collections.Generic;
using NUnit.Framework;
using Unity;

namespace Repro
{
	public interface ITestType<T>
	{
		T Value { get; set; }
	}

	public class TestType<T> : ITestType<T>
	{
		public T Value { get; set; }
	}

	[TestFixture]
	public class Tests
	{
		public static IEnumerable<TestCaseData> Containers
		{
			get
			{
				IUnityContainer root = new UnityContainer().AddExtension(new Diagnostic());
				var child = root.CreateChildContainer();
				child.RegisterType(typeof(ITestType<>), typeof(TestType<>), TypeLifetime.PerContainer);

				yield return new TestCaseData(root, child);
			}
		}

		[TestCaseSource(typeof(Tests), "Containers")]
		public void Resolve_SucceedsOnChild(IUnityContainer root, IUnityContainer child)
		{
			Assert.That(() => child.Resolve<ITestType<int>>(), Throws.Nothing);
		}

		[TestCaseSource(typeof(Tests), "Containers")]
		public void Resolve_FailsOnRoot(IUnityContainer root, IUnityContainer child)
		{
			Assert.That(() => root.Resolve<ITestType<int>>(), Throws.InstanceOf<ResolutionFailedException>());
		}

		[TestCaseSource(typeof(Tests), "Containers")]
		public void Resolve_FailsOnRootThenSucceedsOnChild(IUnityContainer root, IUnityContainer child)
		{
			Assert.That(() => root.Resolve<ITestType<int>>(), Throws.InstanceOf<ResolutionFailedException>());
			Assert.That(() => child.Resolve<ITestType<int>>(), Throws.Nothing);
		}
	}
}