using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interop
{
	public class CHeaderGenerator : HeaderGenerator
	{
		public CHeaderGenerator(TextWriter w) : base(w) { }

		public override void EmitHeaderGuardStart(string guard) {
			w.WriteLine("#ifndef __{0}", guard);
			w.WriteLine("#define __{0}", guard);
		}

		public override void EmitHeaderGuardEnd() {
			w.WriteLine("#endif");
		}


		public override void GenerateInterface(Type itf) {
			w.WriteLine("PACK(");
			w.WriteLine("struct {0} {{", itf.Name);
			foreach(var method in itf.GetMethods()) {
				w.Write("void (STDAPICALLTYPE *{0})(", method.Name);
				GenerateArgs(method, w);
				w.WriteLine(");");
			}
			w.WriteLine("})");
		}
	}
}
