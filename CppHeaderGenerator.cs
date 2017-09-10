using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interop
{
    public class CppHeaderGenerator : HeaderGenerator
    {
		public CppHeaderGenerator(TextWriter w) : base(w){ }

		public override void EmitHeaderGuardStart(string guard) {
			w.WriteLine("#pragma once");
		}

        public override void GenerateInterface(Type itf)
        {		
            w.WriteLine("class {0} : public IUnknown {{", itf.Name);
            w.WriteLine("public:");

            foreach (var method in itf.GetMethods())
            {
                w.Write("    virtual void STDAPICALLTYPE {0}(", method.Name);
                GenerateArgs(method, w);
                w.WriteLine(") = 0;");
            }
            w.WriteLine("};");
            w.WriteLine();

			w.WriteLine("extern {0} *f;", itf.Name);
        }

        private void GenerateEnum(Type enumeration, TextWriter w)
        {
            w.WriteLine("enum {0} {{", enumeration.Name);
            foreach (var mem in Enum.GetNames(enumeration))
            {
                w.WriteLine("    {0} = {1},", mem, Convert.ToUInt64(Enum.Parse(enumeration, mem)));
            }
            w.WriteLine("};");
            w.WriteLine();
        }
    }
}
