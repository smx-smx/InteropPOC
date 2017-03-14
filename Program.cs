using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Interop
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
			SetDllDirectory(@"E:\reko\llvm\build\out\bin");
            var chg = new CppHeaderGenerator();
            chg.Generate(new[]
                {
                    typeof(PrimitiveOp).FullName,
                    typeof(DataTypeEnum).FullName,
                    typeof(IFactory).FullName,
                },
                Console.Out);

            Factory fac = new Factory();
            var factory = Marshal.GetIUnknownForObject(fac);
            var iid = new Guid("E40FFD0D-3019-4ADF-AC48-800F3ACFA360");
            IntPtr ifac;
            var hr = Marshal.QueryInterface(factory, ref iid, out ifac);
            var bytes = new byte[30];
            ulong addr = 0x00123400;
            bytes[0] = 0xC3;

			Initialize(ifac);
			SetupTarget("i386");
			Disasm(addr, bytes, 1);


			/*
            Console.WriteLine(fac.stmts[0].ToString());
            Debug.Print(fac.stmts[0].ToString());
            Debug.Assert(fac.stmts.Count == 1);
			*/
        }

#if __MonoCS__
        [DllImport("driver.so", CallingConvention = CallingConvention.Cdecl)]
#else
#endif

		[DllImport("driver.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Initialize([In] IntPtr factory);

		[DllImport("driver.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SetupTarget([MarshalAs(UnmanagedType.LPStr)] string TripleName);

		[DllImport("driver.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Disasm(UInt64 PC, [MarshalAs(UnmanagedType.LPArray)] byte[] bytes, int size);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool SetDllDirectory(string lpPathName);
	}
}
