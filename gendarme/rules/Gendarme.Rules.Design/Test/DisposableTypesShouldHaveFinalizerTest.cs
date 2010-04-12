//
// Unit tests for DisposableTypesShouldHaveFinalizerRule
//
// Authors:
//	Andreas Noever <andreas.noever@gmail.com>
//	Sebastien Pouliot  <sebastien@ximian.com>
//
//  (C) 2008 Andreas Noever
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using Gendarme.Rules.Design;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Test.Rules.Design {

	class HasFinalizer : IDisposable {
		IntPtr A;
		~HasFinalizer ()
		{
		}

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}

	class NoFinalizer : IDisposable {
		IntPtr A;
		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}
	
	class NotDisposable {
		IntPtr A;
	}

	class NoNativeField : IDisposable {
		object A;
		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}

	class NativeFieldArray : IDisposable {
		IntPtr [] A;
		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}

	class NotDisposableBecauseStatic : IDisposable {
		static IntPtr A;

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}

	[TestFixture]
	public class DisposableTypesShouldHaveFinalizerTest : TypeRuleTestFixture<DisposableTypesShouldHaveFinalizerRule> {

		[Test]
		public void DoesNotApply ()
		{
			AssertRuleDoesNotApply (SimpleTypes.Enum);
			AssertRuleDoesNotApply (SimpleTypes.Delegate);
		}

		[Test]
		public void TestHasFinalizer ()
		{
			AssertRuleSuccess<HasFinalizer> ();
		}

		[Test]
		public void TestNoFinalizer ()
		{
			AssertRuleFailure<NoFinalizer> (1);
		}

		[Test]
		public void TestNotDisposable ()
		{
			AssertRuleDoesNotApply<NotDisposable> ();
		}

		[Test]
		public void TestNoNativeFields ()
		{
			AssertRuleSuccess<NoNativeField> ();
		}

		[Test]
		public void TestNativeFieldArray ()
		{
			AssertRuleFailure<NativeFieldArray> (1);
		}

		[Test]
		public void TestNotDisposableBecauseStatic ()
		{
			AssertRuleSuccess<NotDisposableBecauseStatic> ();
		}
	}
}