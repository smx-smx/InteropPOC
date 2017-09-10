cmake_minimum_required(VERSION 3.6)
project(InteropPOC)

set(CMAKE_CXX_STANDARD 14)

if(LLVM_TEST)
	if(USE_LLVM_SHARED)
		include("llvm_shared.cmake")
	else()
		include("llvm_static.cmake")
	endif(USE_LLVM_SHARED)
endif()

if( NOT CMAKE_BUILD_TYPE )
	set(CMAKE_BUILD_TYPE Debug)
endif()

set(sources "")
list(APPEND sources
	driver/driver.cpp
	driver/Reko.cpp
)
if(WIN32)
    list(APPEND sources driver/dllmain.cpp)
endif(WIN32)

add_library(driver SHARED ${sources})
if(LLVM_TEST)
	target_link_libraries(driver ${llvm_libs})
endif()

set(CSHARP_BUILD_FLAGS //m //p:Configuration=${CMAKE_BUILD_TYPE})
if(WIN32)
	get_filename_component(msbuildcmd
		"[HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\MSBuild\\ToolsVersions\\4.0;MSBuildToolsPath]"
		ABSOLUTE CACHE
	)
	set(msbuildcmd "${msbuildcmd}/msbuild.exe")

	add_custom_target(
		msbuild ALL
		DEPENDS Interop.sln
	)
	add_custom_command(
		TARGET msbuild
		PRE_BUILD
		COMMAND ${msbuildcmd}
		ARGS ${CSHARP_BUILD_FLAGS} Interop.sln
		WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
	)
elseif(UNIX)
	add_custom_target(
		xbuild ALL
		DEPENDS Interop.sln
	)
	add_custom_command(
		TARGET xbuild
		PRE_BUILD
		COMMAND xbuild ${CSHARP_BUILD_FLAGS}
	)
endif()

add_custom_command(
	TARGET driver
	POST_BUILD
	COMMAND ln -sf $<TARGET_FILE:driver> ${CMAKE_SOURCE_DIR}/bin/${CMAKE_BUILD_TYPE}/$<TARGET_FILE_NAME:driver>
)
