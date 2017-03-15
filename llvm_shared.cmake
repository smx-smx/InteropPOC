
set(LOCAL_LLVM_PREFIX "../llvm/build/out")
get_filename_component(LOCAL_LLVM_PREFIX "${LOCAL_LLVM_PREFIX}" REALPATH)
list(INSERT CMAKE_PREFIX_PATH 0 "${LOCAL_LLVM_PREFIX}/lib/cmake/llvm")
find_package(LLVM REQUIRED CONFIG)

message(STATUS "Found LLVM ${LLVM_PACKAGE_VERSION}")
message(STATUS "Using LLVMConfig.cmake in: ${LLVM_DIR}")

include_directories(BEFORE "${LOCAL_LLVM_PREFIX}/include")

#include_directories(${LLVM_INCLUDE_DIRS})
add_definitions(${LLVM_DEFINITIONS})

llvm_map_components_to_libnames(llvm_names
	# Core
	core
	target
	support
	mc
	mcdisassembler
	# Targets
AArch64AsmParser
AArch64AsmPrinter
AArch64CodeGen
AArch64Desc
AArch64Disassembler
AArch64Info
AMDGPUAsmParser
AMDGPUAsmPrinter
AMDGPUCodeGen
AMDGPUDesc
AMDGPUDisassembler
AMDGPUInfo
ARMAsmParser
ARMAsmPrinter
ARMCodeGen
ARMDesc
ARMDisassembler
ARMInfo
BPFAsmPrinter
BPFCodeGen
BPFDesc
BPFDisassembler
BPFInfo
HexagonAsmParser
HexagonCodeGen
HexagonDesc
HexagonDisassembler
HexagonInfo
LanaiAsmParser
LanaiAsmPrinter
LanaiCodeGen
LanaiDesc
LanaiDisassembler
LanaiInfo
MipsAsmParser
MipsAsmPrinter
MipsCodeGen
MipsDesc
MipsDisassembler
MipsInfo
MSP430AsmPrinter
MSP430CodeGen
MSP430Desc
MSP430Info
NVPTXAsmPrinter
NVPTXCodeGen
NVPTXDesc
NVPTXInfo
PowerPCAsmParser
PowerPCAsmPrinter
PowerPCCodeGen
PowerPCDesc
PowerPCDisassembler
PowerPCInfo
RISCVCodeGen
RISCVDesc
RISCVInfo
SparcAsmParser
SparcAsmPrinter
SparcCodeGen
SparcDesc
SparcDisassembler
SparcInfo
SystemZAsmParser
SystemZAsmPrinter
SystemZCodeGen
SystemZDesc
SystemZDisassembler
SystemZInfo
X86AsmParser
X86AsmPrinter
X86CodeGen
X86Desc
X86Disassembler
X86Info
XCoreAsmPrinter
XCoreCodeGen
XCoreDesc
XCoreDisassembler
XCoreInfo
)

set(llvm_libs "")

set(LIBS ${OSG_LIBRARIES})
set(SEARCH_PATHS "${LOCAL_LLVM_PREFIX}/bin")
foreach(LIB ${llvm_names})
	# Necessary to force find_library to NOT do a cached lookup
	set(FOUND_LIB "FOUND_LIB-NOTFOUND")

	find_library(
		FOUND_LIB ${LIB}
		PATHS ${SEARCH_PATHS}
		NO_DEFAULT_PATH
	)
	message("Found Lib: ${FOUND_LIB}")
	list(APPEND llvm_libs "${FOUND_LIB}")
endforeach(LIB)