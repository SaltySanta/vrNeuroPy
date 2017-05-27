import socket
import struct
import sys
import os
import threading
import time

from annar4Interface import *
from annarProtoMain import *
from annarProtoRecv import *
from annarProtoSend import *
from MsgObject_pb2 import *
import sys

MONITOR_INTERVAL = 1000

class AnnarProtoMain(object):

    def __init__(self, srv_addr, remotePortNo, agentNo, agentOnly, softwareInterfaceTimeout = -1):

        if (not agentOnly):
       
        # create socket for VR
            try:

                self.socketVR = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                #timeval = struct.pack('ll', 1, 0)
                #self.socketVR.setsockopt(socket.SOL_SOCKET, socket.SO_RCVTIMEO, timeval)
                self.socketVR.settimeout(1.0)
                self.socketVR.connect((srv_addr, remotePortNo))
            except socket.error as e:
                print "ERROR CONNECTING: " + str(e)
                sys.exit(1)      
        else:
            self.socketVR = -1

        # create socket for Agent
        try:
            #print "creating socket"
            self.socketAgent = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            #timeval = struct.pack('ll', 1, 0)
            #self.socketAgent.setsockopt(socket.SOL_SOCKET, socket.SO_RCVTIMEO, timeval)
            self.socketAgent.settimeout(1.0)
            self.socketAgent.connect((srv_addr, remotePortNo + agentNo + 1))
        except socket.error as e:
            print "ERROR CONNECTING: " + str(e)
            sys.exit(1)



        # create sender and receiver

        self.sender = AnnarProtoSend(self.socketVR, self.socketAgent)
        self.receiver = AnnarProtoReceive(self.socketVR, self.socketAgent)


        if (softwareInterfaceTimeout == -1):
            self.softwareInterfaceTimeout = -1
        else:
            self.softwareInterfaceTimeout = softwareInterfaceTimeout*1000*1000/MONITOR_INTERVAL

        self.done = True
        self.interfaceNotUsed = 0



    def start(self):

        if (self.softwareInterfaceTimeout != -1) and (self.done):
            
            self.done = False
            self.thread = threading.Thread(target=mainLoop)


        self.sender.start()
        self.receiver.start()



    def stop(self, wait):

        if self.sender is not None:

            self.sender.stop(wait)

        if self.receiver is not None:

            self.receiver.stop(wait)

        if (self.softwareInterfaceTimeout != -1) and (not self.done):
            
            self.done = True
            
            if wait:
                self.thread.join()


    def mainLoop(self):

        while not done:

            if (softwareInterfaceTimeout != -1) and (interfaceNotUsed > softwareInterfaceTimeout):

                stop(False)

            

            time.sleep(MONITOR_INTERVAL/1000000.0)

            self.interfaceNotUsed = self.interfaceNotUsed + 1


    def getSender(self):

        self.interfaceNotUsed = 0
        return self.sender

    def getReceiver(self):

        self.interfaceNotUsed = 0
        return self.receiver
