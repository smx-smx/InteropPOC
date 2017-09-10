using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interop
{
	public abstract class HeaderGenerator
	{
		protected TextWriter w;

		public HeaderGenerator(TextWriter w) {
			this.w = w;
		}

		public virtual void EmitHeaderGuardStart(string guard) { }
		public virtual void EmitHeaderGuardEnd() { }

		public abstract void GenerateInterface(Type itf);
		public virtual void GenerateArgs(MethodInfo method) {
			var sep = "";
			foreach (var param in method.GetParameters()) {
				w.Write(sep);
				sep = ", ";
				WriteParameterType(param.ParameterType);
				w.Write(" ");
				w.Write(param.Name);
			}
		}
		public virtual void WriteParameterType(Type type) {
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

		public virtual void GenerateArgs(MethodInfo method, TextWriter w) {
			var sep = "";
			foreach (var param in method.GetParameters()) {
				w.Write(sep);
				sep = ", ";
				WriteParameterType(param.ParameterType);
				w.Write(" ");
				w.Write(param.Name);
			}
		}

		public virtual void Generate(string[] types) {
			EmitHeaderGuardStart(string.Join("_", types));
			w.WriteLine("#include <stdint.h>");
			w.WriteLine();

			foreach (var t in types) {
				var type = Type.GetType(t);
				if (type == null)
					continue;
				if (type.IsInterface) {
					GenerateInterface(type);
				} else if (type.IsEnum) {
					GenerateEnum(type);
				}
			}

			EmitHeaderGuardEnd();
		}

		public virtual void GenerateEnum(Type enumeration) {
			w.WriteLine("enum {0} {{", enumeration.Name);
			foreach (var mem in Enum.GetNames(enumeration)) {
				w.WriteLine("    {0} = {1},", mem, Convert.ToUInt64(Enum.Parse(enumeration, mem)));
			}
			w.WriteLine("};");
			w.WriteLine();
		}
	}
}
