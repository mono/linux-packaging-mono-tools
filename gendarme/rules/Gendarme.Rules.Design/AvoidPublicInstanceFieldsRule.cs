//
// Gendarme.Rules.Design.AvoidPublicInstanceFieldsRule
//
// Authors:
//	Adrian Tsai <adrian_tsai@hotmail.com>
//
// Copyright (c) 2007 Adrian Tsai
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using Mono.Cecil;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.Design {

	/// <summary>
	/// This rule checks every type for externally visible fields. Using a property offers
	/// much more future freedom to adjust the behavior and validations on a field value
	/// without breaking binary compatibility.
	/// </summary>
	/// <example>
	/// Bad example:
	/// <code>
	/// public class Foo {
	///	public int Value;
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// Good example:
	/// <code>
	/// public class Foo {
	///	private int v;
	///	public int Value {
	///	get {
	///		return v;
	///	}
	///	set {
	///		v = value;
	///	}
	/// }
	/// </code>
	/// </example>
	/// <remarks>Prior to Gendarme 2.2 this rule was named AvoidPublicInstanceFieldsRule.</remarks>

	[Problem ("This type contains visible instance fields. A field should be a local implementation detail.")]
	[Solution ("If possible change the field visibility to private or internal.")]
	[FxCopCompatibility ("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
	public class AvoidVisibleFieldsRule : Rule, ITypeRule {

		public RuleResult CheckType (TypeDefinition type)
		{
			// rule doesn't apply on enums, interfaces, delegates or to compiler/tools-generated code
			// e.g. CSC compiles anonymous methods as an inner type that expose public fields
			if (type.IsEnum || type.IsInterface || type.IsDelegate () || type.IsGeneratedCode ())
				return RuleResult.DoesNotApply;
			
			// rule doesn't apply to type non (externally) visible
			if (!type.IsVisible ())
				return RuleResult.DoesNotApply;

			foreach (FieldDefinition fd in type.Fields) {
				if (!fd.IsVisible () || fd.IsSpecialName || fd.HasConstant || fd.IsInitOnly)
					continue;

				if (fd.FieldType.IsArray ()) {
					string s = String.Format ("Consider changing the field '{0}' to a private or internal field and add a 'Set{1}' method.",
						fd.Name, Char.ToUpper (fd.Name [0]) + fd.Name.Substring (1));
					Runner.Report (fd, Severity.Medium, Confidence.Total, s);
				} else {
					string s = String.Format ("Field '{0}' should be private or internal and its value accessed through a property.", fd.Name);
					Runner.Report (fd, Severity.Medium, Confidence.Total, s);
				}
			}
			return Runner.CurrentRuleResult;
		}
	}
}