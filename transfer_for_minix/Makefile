all:	clrbrgsave transfer readtape rcr

clrbrgsave:	clrbrgsave.c
	cc -o clrbrgsave clrbrgsave.c

test:	test.c duart.h
	cc -o test test.c

readtape: readtape.c
	cc -o readtape readtape.c

rcr: rcr.c
	cc -o rcr rcr.c

transfer: transfer.c duart.h
	cc -o transfer transfer.c
