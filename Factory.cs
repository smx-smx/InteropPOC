using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    [ComVisible(true)]
    [Guid("E40FFD0D-3019-4ADF-AC48-800F3ACFA360")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFactory
    {
		void Send(uint message, IntPtr pData, uint size);
		void Send(uint message, byte data);
		void Send(uint message, UInt32 data);
		void Send(uint message, UInt64 data);
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
			byte[] data = new byte[size];
			Marshal.Copy(pData, data, 0, (int)size);
			Console.WriteLine("Received Message {0}", Enum.GetName(typeof(RekoMsg), message));
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
    }
}
