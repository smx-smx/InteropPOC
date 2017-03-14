using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interop
{
    public class CppHeaderGenerator
    {
        public void Generate(string [] types, TextWriter w)
        {
			w.WriteLine("#pragma once");
			w.WriteLine("#include <stdint.h>");
			w.WriteLine();

			foreach (var t in types)
            {
                var type = Type.GetType(t);
                if (type == null)
                    continue;
                if (type.IsInterface)
                {
                    GenerateInterface(type, w);
                } else if (type.IsEnum)
                {
                    GenerateEnum(type, w);
                }
            }
        }

        private void GenerateInterface(Type itf, TextWriter w)
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

        private void GenerateArgs(MethodInfo method, TextWriter w)
        {
            var sep = "";
            foreach (var param in method.GetParameters())
            {
                w.Write(sep);
                sep = ", ";
                WriteParameterType(param.ParameterType, w);
                w.Write(" ");
                w.Write(param.Name);
            }
        }

        private void WriteParameterType(Type type, TextWriter w)
        {
			Type elementType = type.GetElementType();
			Type parameterType = type;
			if (type.IsArray) {
				type = elementType;
			}

			if (type == typeof(UInt16))
				w.Write("uint16_t");
			else if (type == typeof(UInt32))
				w.Write("uint32_t");
			else if (type == typeof(UInt64))
				w.Write("uint64_t");
			else if (type == typeof(int))
				w.Write("int");
			else if (type == typeof(uint))
				w.Write("unsigned int");
			else if (type == typeof(byte))
				w.Write("unsigned char");
			else if (type == typeof(ulong))
				w.Write("unsigned long");
			else if (type == typeof(string))
				w.Write("const wchar_t * ");
			else if (type.IsEnum)
				w.Write(type.Name);
			else
				w.Write("??unknown??");

			if (parameterType.IsArray) {
				w.Write("*");
			}
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
