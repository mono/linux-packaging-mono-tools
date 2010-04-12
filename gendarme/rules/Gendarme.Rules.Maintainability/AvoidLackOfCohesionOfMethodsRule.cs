//
// Gendarme.Rules.Maintainability.AvoidLackOfCohesionOfMethodsRule class
//
// Authors:
//	Cedric Vivier <cedricv@neonux.com>
//
// 	(C) 2008 Cedric Vivier
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
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Gendarme.Framework;
using Gendarme.Framework.Rocks;

namespace Gendarme.Rules.Maintainability {

	/// <summary>
	/// This rule checks every type to check for lack of cohesion between the fields and the methods. Low cohesion is often
	/// a sign that a type is doing too many, different an unrelated things. The cohesion score is given for each defect 
	/// (higher is better) and the success threshold can be configured. 
	/// </summary>
	/// <remarks>This rule is available since Gendarme 2.0</remarks>

	[Problem ("The methods in this class lacks cohesion (higher score is better). This leads to code harder to understand and maintain.")]
	[Solution ("You can apply the Extract Class or Extract Subclass refactoring.")]
	public class AvoidLackOfCohesionOfMethodsRule : Rule, ITypeRule {

		private double successCoh = 0.5;
		private double lowCoh = 0.4;
		private double medCoh = 0.2;
		private int method_minimum_count = 2;//set at 2 to remove 'uninteresting' types
		private int field_minimum_count = 1;//set at 1 to remove 'uninteresting' types
		                                 //this shouldn't be set to another value than MinimumMethodCount/2
		private Dictionary<FieldReference, int> F  = new Dictionary<FieldReference, int>();
		private List<FieldReference> Fset = new List<FieldReference>();

		public RuleResult CheckType (TypeDefinition type)
		{
			//does rule apply?
			if (type.IsEnum || type.IsInterface || type.IsAbstract || type.IsDelegate () || type.IsGeneratedCode ())
				return RuleResult.DoesNotApply;

			if (!type.HasFields || (type.Fields.Count < MinimumFieldCount))
				return RuleResult.DoesNotApply;
			if (!type.HasMethods || (type.Methods.Count < MinimumMethodCount))
				return RuleResult.DoesNotApply;

			//yay! rule do apply!
			double coh = GetCohesivenessForType (type);
			if (coh >= SuccessLowerLimit)
				return RuleResult.Success;
			if (0 == coh)
				return RuleResult.DoesNotApply;

			//how's severity?
			Severity sev = GetCohesivenessSeverity(coh);

			Runner.Report (type, sev, Confidence.Normal, String.Format ("Type cohesiveness : {0}%", (int) (coh * 100)));
			return RuleResult.Failure;
		}

		public double GetCohesivenessForType (TypeDefinition type)
		{
			if (type == null)
				return 0.0;

			int M = 0;//M is the number of methods in the type
			//F keeps the count of distinct-method accesses to the field
			F.Clear ();

			foreach (MethodDefinition method in type.Methods) {
				if (!method.HasBody || method.IsStatic)
					continue;

				//Mset is true if the method has already incremented M
				bool Mset = false;
				//Fset keeps the fields already addressed in the current method
				Fset.Clear ();

				foreach (Instruction inst in method.Body.Instructions)
				{
					if (OperandType.InlineField == inst.OpCode.OperandType)
					{
						FieldDefinition fd = inst.Operand as FieldDefinition;
						if (null == fd || !fd.IsPrivate || fd.IsStatic)
							continue; //does not make sense for LCOM calculation
						if (fd.DeclaringType == type && !Fset.Contains (fd))
						{
							if (!Mset) M++;
							Mset = true;
							if (F.ContainsKey(fd))
							{
								F[fd]++;
							}
							else
							{
								F.Add(fd, 1);
							}
							Fset.Add(fd);
						}
					}
				}
			}

			if (M > MinimumMethodCount && F.Count > MinimumFieldCount)
			{
				double r = 0;
				foreach (KeyValuePair<FieldReference,int> f in F)
				{
					r += f.Value;
				}
				double rm = r / F.Count;
				double coh = Math.Round(1 - ((M - rm) / M), 2);
				return coh;
			}

			return 0;
		}

		private Severity GetCohesivenessSeverity (double coh)
		{
			if (coh >= LowSeverityLowerLimit) return Severity.Low;
			if (coh >= MediumSeverityLowerLimit) return Severity.Medium;
			return Severity.High;//coh<MediumSeverityLowerLimit
		}


		public double SuccessLowerLimit
		{
			get { return successCoh; }
			set { successCoh = value; }
		}

		public double LowSeverityLowerLimit
		{
			get { return lowCoh; }
			set { lowCoh = value; }
		}

		public double MediumSeverityLowerLimit
		{
			get { return medCoh; }
			set { medCoh = value; }
		}

		public int MinimumMethodCount
		{
			get { return method_minimum_count; }
			set {
				if (value < 1)
					throw new ArgumentException ("MinimumMethodCount cannot be lesser than 1", "MinimumMethodCount");
				method_minimum_count = value;
			}
		}

		public int MinimumFieldCount
		{
			get { return field_minimum_count; }
			set {
				if (value < 0)
					throw new ArgumentException ("MinimumFieldCount cannot be lesser than 0", "MinimumFieldCount");
				field_minimum_count = value;
			}
		}
	}
}