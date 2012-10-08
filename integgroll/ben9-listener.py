#!/usr/bin/python


import socket
import time
import select


inport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

inport.bind(("localhost",4444))
inport.setblocking(0)
inport.listen(500)

while 7:
  readers,writers,errors = select.select([inport],[],[])
  for read in readers:
    csocket, address = read.accept()
    text = csocket.recv(100)
    print text + " from " + str(address)

