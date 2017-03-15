// driver.cpp : Defines the exported functions for the DLL application.
//

#include "types.h"
#include "driver.h"
#include "Reko.h"
#include "RekoInterfaces.h"

#include <stdio.h>
#include <iostream>
#include <bitset>


using namespace std;
using namespace llvm;

IFactory *f;

MCInstance::MCInstance(Triple Triple){
	//TODO: Error Checking?
	this->TT = Triple;
	this->TheTarget = TargetRegistry::lookupTarget(this->TT.getTriple(), this->LastError);
	if(!this->TheTarget){
		cerr << "Target Lookup failed!" << endl;
		cerr << this->LastError << endl;
		throw "";
	}
	this->RegInfo = this->TheTarget->createMCRegInfo(this->TT.getTriple());	
	if(!this->RegInfo){
		cerr << "createMCRegInfo failed!" << endl;
		throw "";
	}
	this->AsmInfo = this->TheTarget->createMCAsmInfo(*this->RegInfo, this->TT.getTriple());
	if(!this->AsmInfo){
		cerr << "createMCAsmInfo failed!" << endl;
		throw "";
	}
	this->InstrInfo = this->TheTarget->createMCInstrInfo();
	if(!this->InstrInfo){
		cerr << "createMCInstrInfo failed!" << endl;
		throw "";
	}
	this->SubInfo = this->TheTarget->createMCSubtargetInfo(this->TT.getTriple(), this->CPU, this->FeatureStr);
	if(!this->SubInfo){
		cerr << "createMCSubtargetInfo failed!" << endl;
		throw "";
	}
	this->MCCtx = new MCContext(this->AsmInfo, this->RegInfo, 0);
	if(!this->MCCtx){
		cerr << "Cannot create MCContext!" << endl;
		throw "";
	}
	this->DisAsm = this->TheTarget->createMCDisassembler(*this->SubInfo, *this->MCCtx);
	if(!this->DisAsm){
		cerr << "createMCDisassembler failed!" << endl;
		throw "";
	}
	this->RelInfo = this->TheTarget->createMCRelocationInfo(this->TT.getTriple(), *this->MCCtx);
	if(!this->RelInfo){
		cerr << "createMCRelocationInfo failed!" << endl;
		throw "";
	}
	this->InstPrinter = this->TheTarget->createMCInstPrinter(this->TT, this->AsmInfo->getAssemblerDialect(), *this->AsmInfo, *this->InstrInfo, *this->RegInfo);
	if(!this->InstPrinter){
		cerr << "createMCInstPrinter failed!" << endl;
		throw "";
	}
}
MCInstance::MCInstance(string TargetName) : MCInstance(Triple(TargetName)){}
static MCInstance *Ctx;

void SendMCInstrInfo();
void SendMCRegInfo();
void SendMCAsmInfo();
void SendSubTargetInfo();
void SendMCInst(const MCInst& inst);
void SendMCOperand(const MCOperand& opnd);

void SendSubTargetInfo(){
	Reko::Send(MSG_SUBTGT_START);
	Reko::Send(MSG_SUBTGT_CPUNAME, Ctx->SubInfo->getCPU());
	Reko::Send(MSG_SUBTGT_FEATURES, Ctx->SubInfo->getFeatureBits().to_ulong());
	Reko::Send(MSG_SUBTGT_END);
}

void SendMCAsmInfo(){
	const MCAsmInfo *info = Ctx->AsmInfo;
	Reko::Send(MSG_ASMINFO_START);
	Reko::Send(MSG_ASMINFO_POINTERSIZE, info->getPointerSize());
	Reko::Send(MSG_ASMINFO_MAXINSTLEN, info->getMaxInstLength());
	Reko::Send(MSG_ASMINFO_MININSTALIGN, info->getMinInstAlignment());
	Reko::Send(MSG_ASMINFO_END);
}

void SendMCRegInfo(){
	for(unsigned int i=0; i<Ctx->RegInfo->getNumRegs(); i++){
		const MCRegisterDesc &regdesc = Ctx->RegInfo->get(i);
		Reko::Send(MSG_REG_START);
		Reko::Send(MSG_REG_NAME, Ctx->RegInfo->getName(i));
		Reko::Send(MSG_REG_END);
	}
	for(unsigned int i=0; i<Ctx->RegInfo->getNumRegClasses(); i++){
		const MCRegisterClass &regclass = Ctx->RegInfo->getRegClass(i);
		Reko::Send(MSG_REGCLASS_START);
		Reko::Send(MSG_REGCLASS_ALIGNMENT, regclass.Alignment);
		Reko::Send(MSG_REGCLASS_SETSIZE, regclass.RegSetSize);
		Reko::Send(MSG_REGCLASS_SIZE, regclass.RegSize);
		Reko::Send(MSG_REGCLASS_END);
	}
}

void SendMCInstrInfo(){
	for(unsigned int i=0; i<Ctx->InstrInfo->getNumOpcodes(); i++){
		string OpName = Ctx->InstrInfo->getName(i).str();
		const MCInstrDesc &desc = Ctx->InstrInfo->get(i);
		const MCOperandInfo *opinfo = desc.OpInfo;


		Reko::Send(MSG_INSN_START);
		Reko::Send(MSG_INSN_NUM, i);
		Reko::Send(MSG_INSN_NAME, OpName);
		Reko::Send(MSG_INSN_OPND_NUM, desc.NumOperands);
		Reko::Send(MSG_INSN_OPCODE, desc.Opcode);
		Reko::Send(MSG_INSN_SIZE, desc.Size);
		Reko::Send(MSG_INSN_OPND_TYPE, opinfo->OperandType);
		Reko::Send(MSG_INSN_END);
	}
}

void SendMCOperand(const MCOperand &opnd){
	Reko::Send(MSG_OPND_START);
	Reko::Send(MSG_OPND_VALID, opnd.isValid());

	if(opnd.isValid()){
		if(opnd.isExpr()){
			int64_t AbsVal;
			const MCExpr *expr = opnd.getExpr();
			Reko::Send(MSG_OPND_EXPR_START);
			Reko::Send(MSG_OPND_EXPR_KIND, to_underlying<MCExpr::ExprKind>(expr->getKind()));				
			if(expr->evaluateAsAbsolute(AbsVal)){
				Reko::Send(MSG_OPND_EXPR_ABSVALUE, (uint64_t)AbsVal);
			}
			Reko::Send(MSG_OPND_EXPR_END);
		} else if(opnd.isFPImm()){
			Reko::Send(MSG_OPND_FPIMM_START);
			Reko::Send(MSG_OPND_FPIMM_VALUE, opnd.getFPImm());
			Reko::Send(MSG_OPND_FPIMM_END);
		} else if(opnd.isImm()){
			Reko::Send(MSG_OPND_IMM_START);
			Reko::Send(MSG_OPND_IMM_VALUE, (uint64_t)opnd.getImm());
			Reko::Send(MSG_OPND_IMM_END);
		} else if(opnd.isInst()){
			Reko::Send(MSG_OPND_INSN_START);
			SendMCInst(*opnd.getInst());
			Reko::Send(MSG_OPND_INSN_END);
		} else if(opnd.isReg()){
			Reko::Send(MSG_OPND_REG_START);
			Reko::Send(MSG_OPND_REG_NUM, opnd.getReg());
			Reko::Send(MSG_OPND_REG_END);
		}
	}

	Reko::Send(MSG_OPND_END);
}

void SendMCInst(const MCInst &instr){
	Reko::Send(MSG_OP_START);
	Reko::Send(MSG_OP_OPCODE, instr.getOpcode());
	Reko::Send(MSG_OP_SIZE, instr.size());

	for(int i=0; i<instr.getNumOperands(); i++){
		MCOperand opnd = instr.getOperand(i);
		SendMCOperand(opnd);
	}

	Reko::Send(MSG_OP_END);
}

extern "C"
{	
	EXPORT void Initialize(IFactory *f){
		::f = f;
	}

	EXPORT void SetupTarget(const char *TripleName){
		InitializeAllTargetInfos();
		InitializeAllTargets();
		InitializeAllTargetMCs();
		InitializeAllDisassemblers();
		InitializeAllAsmParsers();
		InitializeAllAsmPrinters();
		
		if(Ctx){
			delete Ctx;
		}

		Ctx = new MCInstance(Triple(TripleName));
	}

	EXPORT void Disasm(uint64_t PC, uint8_t *pData, size_t dataSize){
		size_t remaining = dataSize;
		ArrayRef<uint8_t> Bytes(pData, dataSize);
		while(remaining > 0){
			cout << "Loop with " << remaining << endl;
			MCInst instr;
			uint64_t instrSz = 0;
			//MCDisassembler::DecodeStatus result = Ctx->DisAsm->getInstruction(instr, instrSz, Bytes, PC, outs(), outs());
			MCDisassembler::DecodeStatus result = Ctx->DisAsm->getInstruction(instr, instrSz, Bytes, 0, nulls(), nulls());
			switch(result){
				case MCDisassembler::Success:{
					remaining -= instrSz;
					SendMCInst(instr);

					SmallVector<char, 64> InsnStr;
					raw_svector_ostream OS(InsnStr);
					formatted_raw_ostream FormattedOS(OS);
					raw_svector_ostream Annotations(InsnStr);
					StringRef AnnotationsStr = Annotations.str();
					Ctx->InstPrinter->printInst(&instr, FormattedOS, AnnotationsStr, *Ctx->SubInfo);

					cout << InsnStr.data() << endl;

					break;
				} default:
					cerr << "Instruction decoding failed!" << endl;
					break;
			}
		}
	}
}
