using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace Interop
{
	public class TypeConverter<T> {
		private TextWriter tw;

		private CodeCompileUnit unit;

		public TypeConverter(TextWriter w){
			tw = w;
		}

		private void AppendPrologue() {
			
		}

		private void AppendEpilogue() {

		}

		private void GenerateClass(Type t) {

		}

		private void GenerateMember(MemberInfo m) {

		}

		private void GenerateMethod(MethodInfo m) {

		}

		public void Generate(T obj) {
			Type type = typeof(T);

			foreach(MemberInfo m in type.GetMembers(BindingFlags.Public | BindingFlags.Static)) {
				GenerateMember(m);
			}

			foreach(MethodInfo m in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
				GenerateMethod(m);
			}
		}
	}
}
