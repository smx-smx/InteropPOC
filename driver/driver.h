#ifndef DRIVER_H
#define DRIVER_H

#include <iostream>
#include <memory>

#include <llvm/MC/MCAsmInfo.h>
#include <llvm/MC/MCContext.h>
//#include <llvm/MC/MCDisassembler.h>
#include <llvm/MC/MCDisassembler/MCDisassembler.h>
#include <llvm/MC/MCInst.h>
#include <llvm/MC/MCInstPrinter.h>
#include <llvm/MC/MCInstrInfo.h>
#include <llvm/MC/MCInstrDesc.h>
#include <llvm/MC/MCRegisterInfo.h>
#include <llvm/MC/MCSubtargetInfo.h>
#include <llvm/Support/Format.h>
#include <llvm/Support/raw_ostream.h>
#include <llvm/Support/MemoryObject.h>
#include <llvm/Support/TargetRegistry.h>
#include <llvm/Support/TargetSelect.h>
#include <llvm/Support/ErrorHandling.h>
#include <llvm/Support/Debug.h>

using namespace std;
using namespace llvm;

class MCInstance {
private:
public:
	string LastError;
	string CPU;
	string FeatureStr;
	Triple TT;
	const Target *TheTarget;
	MCContext *MCCtx;
	const MCRegisterInfo *RegInfo;
	const MCAsmInfo *AsmInfo;
	const MCInstrInfo *InstrInfo;
	const MCSubtargetInfo *SubInfo;
	const MCDisassembler *DisAsm;
	const MCRelocationInfo *RelInfo;
	const MCInstPrinter *InstPrinter;
	MCInstance(Triple Triple);
	MCInstance(string TargetName);
	
	~MCInstance(){
		delete this->InstPrinter;
		delete this->RelInfo;
		delete this->DisAsm;
		delete this->SubInfo;
		delete this->InstrInfo;
		delete this->AsmInfo;
		delete this->RegInfo;
		delete this->MCCtx;
		delete this->TheTarget;
	}
};

typedef enum {
	R_OP_NUM = 0,
	R_OP_NAME,
	R_OP_NUMOPND,
} REKO_OP;

#endif /* DRIVER_H */

