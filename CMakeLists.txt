cmake_minimum_required(VERSION 3.7)
project(InteropPOC)

if( NOT CMAKE_BUILD_TYPE )
	set(CMAKE_BUILD_TYPE Debug)
endif()

set(MONO_BUILD_FLAGS Interop.sln /p:Configuration)
set(MONO_BUILD_TYPE "Debug")

if( CMAKE_BUILD_TYPE EQUAL "RELEASE" )
	set(MONO_BUILD_TYPE "Release")
endif()

set(MONO_BUILD_FLAGS "${MONO_BUILD_FLAGS}=${MONO_BUILD_TYPE}")

add_library(driver SHARED driver/driver.cpp)

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
