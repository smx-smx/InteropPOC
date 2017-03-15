cmake_minimum_required(VERSION 3.7)
project(InteropPOC)

set(CMAKE_CXX_STANDARD 14)

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

if( NOT CMAKE_BUILD_TYPE )
	set(CMAKE_BUILD_TYPE Debug)
endif()

set(MONO_BUILD_FLAGS Interop.sln /p:Configuration)
set(MONO_BUILD_TYPE "Debug")

if( CMAKE_BUILD_TYPE EQUAL "RELEASE" )
	set(MONO_BUILD_TYPE "Release")
endif()

set(MONO_BUILD_FLAGS "${MONO_BUILD_FLAGS}=${MONO_BUILD_TYPE}")


set(sources "")
list(APPEND sources
	driver/driver.cpp
	driver/Reko.cpp
)
if(WIN32)
	list(APPEND sources driver/dllmain.cpp)
endif(WIN32)
add_library(driver SHARED ${sources})

target_link_libraries(driver ${llvm_libs})


if(UNIX AND NOT WIN32)
	add_custom_target(
		xbuild ALL
		DEPENDS Interop.sln
	)

	add_custom_command(
		TARGET xbuild
		PRE_BUILD
		COMMAND xbuild ${MONO_BUILD_FLAGS}
	)

	add_custom_command(
		TARGET driver
		POST_BUILD
		COMMAND ln -sf $<TARGET_FILE:driver> bin/${MONO_BUILD_TYPE}/$<TARGET_FILE_NAME:driver>
	)
endif()
