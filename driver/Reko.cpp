#include "Reko.h"

extern "C" {
    EXPORT int RekoSend(unsigned message, void *data, size_t size){
        return Reko::Send(message, data, size);
    }
}