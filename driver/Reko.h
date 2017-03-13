#ifndef REKO_MSG_H
#define REKO_MSG_H

#include <iostream>
#include <stdlib.h>
#include <stdint.h>
#include <string.h>
using namespace std;

template <typename E>
constexpr auto to_underlying(E e) noexcept {
    return static_cast<std::underlying_type_t<E>>(e);
}

class Reko {
public:
	Reko();
	Reko(const Reko& orig);
	virtual ~Reko();
	
	static int Send(unsigned message, void *data, size_t size){	
	};
	
	static int Send(unsigned message){
		return Send(message, NULL, 0);
	}
	static int Send(unsigned message, uint8_t data){
		return Send(message, (void *)&data, sizeof(data));
	};
	static int Send(unsigned message, bool data){
		uint8_t bin = (data == true) ? 1 : 0;
		return Send(message, bin);
	};
	static int Send(unsigned message, uint16_t data){
		return Send(message, (void *)&data, sizeof(data));
	};
	static int Send(unsigned message, uint32_t data){
		return Send(message, (void *)&data, sizeof(data));
	};
	static int Send(unsigned message, uint64_t data){
		return Send(message, (void *)&data, sizeof(data));
	}
	static int Send(unsigned message, char *data){
		return Send(message, (void *)data, strlen(data) + 1);
	}
	static int Send(unsigned message, string data){
		return Send(message, data.c_str());
	}
	static int Send(unsigned message, float data){
		return Send(message, (void *)&data, sizeof(data));
	}
	static int Send(unsigned message, double data){
		return Send(message, (void *)&data, sizeof(data));
	}
private:

};

typedef enum {
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
} REKO_MSG;

#endif /* REKO_MSG_H */

