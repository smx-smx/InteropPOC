using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Interop
{
	public enum RekoMsg
	{
		/** Instruction **/
		MSG_INSN_START,
		MSG_INSN_NUM,
		MSG_INSN_NAME,
		MSG_INSN_OPND_NUM,
		MSG_INSN_OPCODE,
		MSG_INSN_SIZE,
		MSG_INSN_OPND_TYPE,
		MSG_INSN_END,

		/** Registers **/
		MSG_REG_START,
		MSG_REG_NAME,
		MSG_REG_END,

		/** Asm Info **/
		MSG_ASMINFO_START,
		MSG_ASMINFO_POINTERSIZE,
		MSG_ASMINFO_MAXINSTLEN,
		MSG_ASMINFO_MININSTALIGN,
		MSG_ASMINFO_END,

		/** Subtarget Info **/
		MSG_SUBTGT_START,
		MSG_SUBTGT_CPUNAME,
		MSG_SUBTGT_FEATURES,
		MSG_SUBTGT_END,

		/** Register Classes */
		MSG_REGCLASS_START,
		MSG_REGCLASS_ALIGNMENT,
		MSG_REGCLASS_SETSIZE,
		MSG_REGCLASS_SIZE,
		MSG_REGCLASS_END,

		/** Opcode **/
		MSG_OP_START,
		MSG_OP_OPCODE,
		MSG_OP_SIZE,
		MSG_OP_OPND_NUM,
		MSG_OP_END,

		/** Operand **/
		MSG_OPND_START,
		MSG_OPND_VALID,
		//MSG_OPND_TYPE,

		MSG_OPND_EXPR_START,
		MSG_OPND_EXPR_KIND,
		MSG_OPND_EXPR_ABSVALUE,
		MSG_OPND_EXPR_VALUE,
		MSG_OPND_EXPR_END,

		MSG_OPND_FPIMM_START,
		MSG_OPND_FPIMM_VALUE,
		MSG_OPND_FPIMM_END,

		MSG_OPND_IMM_START,
		MSG_OPND_IMM_VALUE,
		MSG_OPND_IMM_END,

		MSG_OPND_INSN_START,
		MSG_OPND_INSN_END,

		MSG_OPND_REG_START,
		MSG_OPND_REG_NUM,
		MSG_OPND_REG_END,

		MSG_OPND_END
	}
    public enum PrimitiveOp
    {
        Not,        // C/C++ !
        Cmp,        // ~
        Neg,        // Unary -
        AddrOf,     // &

        IAdd,
        ISub,
        IMul,
        SMul,
        UMul,
        IDiv,
        SDiv,
        UDiv,

        Shl,
        Shr,
        Sar,

        And,
        Or,
        Xor,

        FAdd,
        FSub,
        FMul,
        FDiv,

        Eq,
        Ne,
        Lt,
        Le,
        Ge,
        Gt,
        Ult,
        Ule,
        Uge,
        Ugt,
    }

    public enum DataTypeEnum
    {
        Void,

        Bool,

        Byte,
        Int8,

        Word16,
        Int16,
        UInt16,
        Ptr16,

        Word32,
        Int32,
        UInt32,
        Real32,
        Ptr32,
        FarPtr32,

        Word64,
        Int64,
        UInt64,
        Real64,
        Ptr64,
    }

	public delegate void NativeCallback(IntPtr type);

    [ComVisible(true)]
    [Guid("E40FFD0D-3019-4ADF-AC48-800F3ACFA360")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFactory
    {
		void Send(uint message, IntPtr pData, uint size);
		void Send(uint message, byte data);
		void Send(uint message, UInt32 data);
		void Send(uint message, UInt64 data);
		void GetType(
			[MarshalAs(UnmanagedType.LPWStr)] string typeName,
			[MarshalAs(UnmanagedType.FunctionPtr)] NativeCallback cb
		);
	}

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Factory : MarshalByRefObject, IFactory
    {
        private Stack<Exp> stack;
        public readonly List<Stmt> stmts;

        public Factory()
        {
            this.stack = new Stack<Exp>();
            this.stmts = new List<Stmt>();
        }

		public void Send(uint message, IntPtr pData, uint size) {
			Console.WriteLine("Received Message {0}", Enum.GetName(typeof(RekoMsg), message));
			if (size > 0) {
				byte[] data = new byte[size];
				Marshal.Copy(pData, data, 0, (int)size);
			}
		}
		public void Send(uint message, byte data) {
			Console.WriteLine("Received byte");
		}
		public void Send(uint message, UInt32 data) {
			Console.WriteLine("Received dword");
		}
		public void Send(uint message, UInt64 data) {
			Console.WriteLine("Received qword");
		}

		[DllImport("driver")]
		public static extern IntPtr calloc(int num, uint size);

		[DllImport("driver")]
		public static extern IntPtr memcpy(IntPtr dest, IntPtr src, uint size);

		[DllImport("driver")]
		public static extern void free(IntPtr mem);

		private object BuildStructType(Type t) {
			StringBuilder sb = new StringBuilder();


			foreach (MemberInfo m in t.GetMembers(BindingFlags.Public)) {
				sb.AppendFormat("public {0} {1};", m.Name, m.Name);
				sb.AppendLine();
			}

			foreach(FieldInfo f in t.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
				var mt = f.FieldType;
				sb.AppendFormat("public {0} {1};", mt.Name, f.Name);
				sb.AppendLine();
			}

			foreach(PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
				var mt = p.PropertyType;
				sb.AppendFormat("public {0} {1};", mt.Name, p.Name);
				sb.AppendLine();
			}

			string code = string.Format(@"
				using System;
				using System.Runtime.InteropServices;
				namespace RunTimeCompile
				{{
					[StructLayoutAttribute(LayoutKind.Sequential)]
					public struct {0}
					{{
						{1}
					}}
				}}
			", t.Name, sb.ToString());

			var oCodeDomProvider = CodeDomProvider.CreateProvider("CSharp");
			CompilerParameters oCompilerParameters = new CompilerParameters();
			oCompilerParameters.ReferencedAssemblies.Add("system.dll");
			oCompilerParameters.GenerateExecutable = false;
			oCompilerParameters.GenerateInMemory = true;
			var oCompilerResults = oCodeDomProvider.CompileAssemblyFromSource(oCompilerParameters, code);

			var oAssembly = oCompilerResults.CompiledAssembly;
			var oObject = oAssembly.CreateInstance(string.Format("RunTimeCompile.{0}", t.Name));
			return oObject;
		}

		private Dictionary<string, object> types = new Dictionary<string, object>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="cb"></param>
		public void GetType(string typeName, NativeCallback cb) {
			object obj;
			if (types.ContainsKey(typeName)) {
				obj = types[typeName];
			} else {
				Type type = Type.GetType(typeName);
				if (type == null)
					throw new ArgumentNullException("Can't find type " + typeName);

				obj = BuildStructType(type);
				types[typeName] = obj;
			}

			int size = Marshal.SizeOf(obj);

			IntPtr unmanagedAddr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(obj, unmanagedAddr, false);
			cb(unmanagedAddr);
			Marshal.FreeHGlobal(unmanagedAddr);
		}
	}
}
