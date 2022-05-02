# Copyright 1994-2018 The MathWorks, Inc.
#
# File    : grt_lcc64.tmf   
#
# Abstract:
#	Template makefile for building a PC-based stand-alone generic real-time 
#       version of Simulink model using	generated C code and LCC compiler 
#       Version 2.4
#
# 	This makefile attempts to conform to the guidelines specified in the
# 	IEEE Std 1003.2-1992 (POSIX) standard. It is designed to be used
#       with GNU Make (gmake) which is located in matlabroot/bin/win64.
#
# 	Note that this template is automatically customized by the build 
#       procedure to create "<model>.mk"
#
#       The following defines can be used to modify the behavior of the
#	build:
#	  OPT_OPTS       - Optimization options. Default is none. To enable
#                          debugging specify as OPT_OPTS=-g4.
#	  OPTS           - User specific compile options.
#	  USER_SRCS      - Additional user sources, such as files needed by
#			   S-functions.
#	  USER_INCLUDES  - Additional include paths
#			   (i.e. USER_INCLUDES="-Iwhere-ever -Iwhere-ever2")
#			   (For Lcc, have a '/'as file separator before the
#			   file name instead of a '\' .
#			   i.e.,  d:\work\proj1/myfile.c - required for 'gmake')
#
#       This template makefile is designed to be used with a system target
#       file that contains 'rtwgensettings.BuildDirSuffix' see grt.tlc

#------------------------ Macros read by make_rtw ------------------------------
#
# The following macros are read by the build procedure:
#
#  MAKECMD         - This is the command used to invoke the make utility
#  HOST            - What platform this template makefile is targeted for
#                    (i.e. PC or UNIX)
#  BUILD           - Invoke make from the build procedure (yes/no)?
#  SYS_TARGET_FILE - Name of system target file.

MAKECMD         = "%MATLAB%\bin\win64\gmake"
SHELL           = cmd
HOST            = PC
BUILD           = yes
SYS_TARGET_FILE = grt.tlc
BUILD_SUCCESS	= *** Created

# Opt in to simplified format by specifying compatible Toolchain
TOOLCHAIN_NAME = "LCC-win64 v2.4.1 | gmake (64-bit Windows)"

MAKEFILE_FILESEP = /

#---------------------- Tokens expanded by make_rtw ----------------------------
#
# The following tokens, when wrapped with "|>" and "<|" are expanded by the
# build procedure.
#
#  MODEL_NAME          - Name of the Simulink block diagram
#  MODEL_MODULES       - Any additional generated source modules
#  MAKEFILE_NAME       - Name of makefile created from template makefile <model>.mk
#  MATLAB_ROOT         - Path to where MATLAB is installed.
#  MATLAB_BIN          - Path to MATLAB executable.
#  S_FUNCTIONS_LIB     - List of S-functions libraries to link.
#  NUMST               - Number of sample times
#  TID01EQ             - yes (1) or no (0): Are sampling rates of continuous task
#                        (tid=0) and 1st discrete task equal.
#  NCSTATES            - Number of continuous states
#  BUILDARGS           - Options passed in at the command line.
#  MULTITASKING        - yes (1) or no (0): Is solver mode multitasking
#  MAT_FILE            - yes (1) or no (0): Should mat file logging be done


MODEL                = random_sample
MODULES              = rt_logging.c random_sample.c random_sample_data.c rtGetInf.c rtGetNaN.c rt_nonfinite.c rt_main.c
PRODUCT              = $(RELATIVE_PATH_TO_ANCHOR)/random_sample.exe
MAKEFILE             = random_sample.mk
MATLAB_ROOT          = C:/Program Files/Polyspace/R2019b
ALT_MATLAB_ROOT      = C:/PROGRA~1/POLYSP~1/R2019b
MATLAB_BIN           = C:/Program Files/Polyspace/R2019b/bin
ALT_MATLAB_BIN       = C:/PROGRA~1/POLYSP~1/R2019b/bin
MASTER_ANCHOR_DIR    = 
START_DIR            = B:/Ucheba/Диплом/Soft_from_work/matlab_scripts/bogdan
S_FUNCTIONS_LIB      = 
NUMST                = 1
TID01EQ              = 0
NCSTATES             = 0
BUILDARGS            =  EXTMODE_STATIC_ALLOC=0 TMW_EXTMODE_TESTING=0 EXTMODE_STATIC_ALLOC_SIZE=1000000 EXTMODE_TRANSPORT=0 INCLUDE_MDL_TERMINATE_FCN=1 ISPROTECTINGMODEL=NOTPROTECTING
MULTITASKING         = 0
MAT_FILE             = 1

CLASSIC_INTERFACE    = 0
# Optional for GRT
ALLOCATIONFCN        = 0
ONESTEPFCN           = 1
TERMFCN              = 1
MULTI_INSTANCE_CODE  = 0

MODELREFS            = 
OPTIMIZATION_FLAGS   = 
ADDITIONAL_LDFLAGS   = 
DEFINES_CUSTOM       = 

#--------------------------- Model and reference models -----------------------
MODELLIB                  = 
MODELREF_LINK_LIBS        = 
MODELREF_LINK_RSPFILE     = random_sample_ref.rsp
RELATIVE_PATH_TO_ANCHOR   = ..
# NONE: standalone, SIM: modelref sim, RTW: modelref coder target
MODELREF_TARGET_TYPE       = NONE

#-- In the case when directory name contains space ---
ifneq ($(MATLAB_ROOT),$(ALT_MATLAB_ROOT))
MATLAB_ROOT := $(ALT_MATLAB_ROOT)
endif
ifneq ($(MATLAB_BIN),$(ALT_MATLAB_BIN))
MATLAB_BIN := $(ALT_MATLAB_BIN)
endif

#--------------------------- Tool Specifications ------------------------------

LCC = $(MATLAB_ROOT)\sys\lcc64\lcc64
include $(MATLAB_ROOT)\rtw\c\tools\lcc64tools.mak

CMD_FILE             = $(MODEL).rsp

#------------------------------ Include Path ----------------------------------

# see COMPILER_INCLUDES from lcc64tools.mak

ADD_INCLUDES = \
	-I$(START_DIR) \
	-I$(START_DIR)/random_sample_grt_rtw \
	-I$(MATLAB_ROOT)/extern/include \
	-I$(MATLAB_ROOT)/simulink/include \
	-I$(MATLAB_ROOT)/rtw/c/src \
	-I$(MATLAB_ROOT)/rtw/c/src/ext_mode/common \


INCLUDES = -I. -I$(RELATIVE_PATH_TO_ANCHOR) $(ADD_INCLUDES) \
           $(COMPILER_INCLUDES) $(USER_INCLUDES)

#------------------------ rtModel ----------------------------------------------
RTM_CC_OPTS = -DUSE_RTMODEL

#-------------------------------- C Flags --------------------------------------

# Optimization Options
OPT_OPTS = $(DEFAULT_OPT_OPTS)

# General User Options
OPTS =

# Compiler options, etc:
ifneq ($(OPTIMIZATION_FLAGS),)
CC_OPTS = $(OPTS) $(ANSI_OPTS) $(RTM_CC_OPTS) $(OPTIMIZATION_FLAGS)
else
CC_OPTS = $(OPT_OPTS) $(OPTS) $(ANSI_OPTS) $(RTM_CC_OPTS)
endif

CPP_REQ_DEFINES = -DMODEL=$(MODEL) -DRT -DNUMST=$(NUMST) \
                  -DTID01EQ=$(TID01EQ) -DNCSTATES=$(NCSTATES) \
                  -DMT=$(MULTITASKING) -DHAVESTDIO -DMAT_FILE=$(MAT_FILE) \
		  -DONESTEPFCN=$(ONESTEPFCN) -DTERMFCN=$(TERMFCN) \
		  -DMULTI_INSTANCE_CODE=$(MULTI_INSTANCE_CODE) \
		  -DCLASSIC_INTERFACE=$(CLASSIC_INTERFACE) \
		  -DALLOCATIONFCN=$(ALLOCATIONFCN)

CFLAGS = $(DEFAULT_CFLAGS) $(CC_OPTS) $(DEFINES_CUSTOM) $(CPP_REQ_DEFINES) $(INCLUDES) -noregistrylookup -w

ifeq ($(OPT_OPTS),$(DEFAULT_OPT_OPTS))
LDFLAGS = -s -L$(LIB)
else
LDFLAGS = -L$(LIB)
endif


#------------------------- Additional Libraries -------------------------------

LIBS =


LIBS += $(S_FUNCTIONS_LIB)

#----------------------------- Source Files ------------------------------------
USER_SRCS =

USER_OBJS       = $(USER_SRCS:.c=.obj)
LOCAL_USER_OBJS = $(notdir $(USER_OBJS))

SRCS      = $(MODULES)
OBJS      = $(SRCS:.c=.obj)  $(USER_OBJS)

#--------------------------------- Rules ---------------------------------------

ifeq ($(MODELREF_TARGET_TYPE),NONE)
BIN_SETTING        = $(LD) $(LDFLAGS) $(ADDITIONAL_LDFLAGS) -o $(PRODUCT) $(SYSLIBS)
$(PRODUCT) : $(OBJS) $(LIBS) $(MODELREF_LINK_LIBS)
	$(BIN_SETTING) @$(CMD_FILE) $(LOCAL_USER_OBJS) @$(MODELREF_LINK_RSPFILE) $(LIBS) 
	@cmd /C "echo $(BUILD_SUCCESS) executable: $(PRODUCT)"
else

$(PRODUCT) : $(OBJS)
	$(LIBCMD) /out:$(MODELLIB) @$(CMD_FILE) $(LOCAL_USER_OBJS)
	@cmd /C "echo $(BUILD_SUCCESS) static library $(MODELLIB)"
endif

%.obj : %.c
	$(CC) -c -Fo$(@F) $(CFLAGS) $<

%.obj : %.C
	$(CC) -c -Fo$(@F) $(CFLAGS) $<

%.obj : $(RELATIVE_PATH_TO_ANCHOR)/%.c
	$(CC) -c -Fo$(@F) -I$(RELATIVE_PATH_TO_ANCHOR)/$(<F:.c=cn_rtw) $(CFLAGS)  $<

%.obj : $(RELATIVE_PATH_TO_ANCHOR)/%.C
	$(CC) -c -Fo$(@F) -I$(RELATIVE_PATH_TO_ANCHOR)/$(<F:.c=cn_rtw) $(CFLAGS)  $<

%.obj : $(MATLAB_ROOT)/rtw/c/src/%.c
	$(CC) -c -Fo$(@F) $(CFLAGS) $<

%.obj : $(MATLAB_ROOT)/simulink/src/%.c
	$(CC) -c -Fo$(@F) $(CFLAGS) $<

rt_logging.obj : C:\Program\ Files\Polyspace\R2019b\rtw\c\src/rt_logging.c
	$(CC) -c -Fo$(@F) $(CFLAGS) C:\Program\ Files\Polyspace\R2019b\rtw\c\src/rt_logging.c

rt_main.obj : C:\Program\ Files\Polyspace\R2019b\rtw\c\src\common/rt_main.c
	$(CC) -c -Fo$(@F) $(CFLAGS) C:\Program\ Files\Polyspace\R2019b\rtw\c\src\common/rt_main.c



%.obj : $(MATLAB_ROOT)/rtw/c/src/%.C
	$(CC) -c -Fo$(@F) $(CFLAGS) $<

%.obj : $(MATLAB_ROOT)/simulink/src/%.C
	$(CC) -c -Fo$(@F) $(CFLAGS) $<



# Libraries:





#----------------------------- Dependencies ------------------------------------

$(OBJS) : $(MAKEFILE) rtw_proj.tmw
