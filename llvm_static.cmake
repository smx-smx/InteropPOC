find_package(LLVM REQUIRED CONFIG)

message(STATUS "Found LLVM ${LLVM_PACKAGE_VERSION}")
message(STATUS "Using LLVMConfig.cmake in: ${LLVM_DIR}")

include_directories(${LLVM_INCLUDE_DIRS})
add_definitions(${LLVM_DEFINITIONS})

llvm_map_components_to_libnames(llvm_libs
	aarch64disassembler
	armdisassembler
	hexagondisassembler
	mcdisassembler
	mipsdisassembler
	powerpcdisassembler
	sparcdisassembler
	systemzdisassembler
	x86disassembler
	xcoredisassembler
)