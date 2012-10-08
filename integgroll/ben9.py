#!/usr/bin/python

import socket
import getpass
import time

xport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

xport.connect(('localhost',4444))
xport.send(getpass.getuser()+" clicked shit at "+time.strftime("%d/%m/%Y  %H:%M:%S",time.gmtime()))

xport.shutdown(1)
xport.close()

