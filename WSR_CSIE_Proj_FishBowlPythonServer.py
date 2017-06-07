import socket
import threading
import RPi.GPIO as GPIO
import time,math
import datetime
from threading import *
class timesmeg:
    def __init__(self):
        self.Ledtimeflag=[False,False,False,False,False]
        self.LedHour=["0","0","0","0","0"]
        self.LedMin=["0","0","0","0","0"]
        self.Ledonoff=[True,True,True,True,True]
        self.Feedtimeflag=[False,False,False,False,False]
        self.FeedHour=["0","0","0","0","0"]
        self.FeedMin=["0","0","0","0","0"]
        self.Light=False
        
    def setLedtimeflag(self,z,tmp):
        self.Ledtimeflag[z]=tmp
    def getLedtimeflag(self,z):
        return self.Ledtimeflag[z]
    def setLight(self,tmp):
        self.Light=tmp
    def getLight(self):
        return self.Light
    def setbookledtime(self,z,tmp,tmp2):
        self.LedHour[z]=tmp
        self.LedMin[z]=tmp2
    def getbookledtime(self,z):
        return self.LedHour[z]+self.LedMin[z]
    def getFir(self):
        fir=""
        for k in range(5):
            fir+=str(self.Ledtimeflag[k])+":"
            fir+=str(self.LedHour[k])+":"
            fir+=str(self.LedMin[k])+":"
            fir+=str(self.Ledonoff[k])+":"
            fir+=str(self.Feedtimeflag[k])+":"
            fir+=str(self.FeedHour[k])+":"
            fir+=str(self.FeedMin[k])+":"
        return str(fir)
    def setFeedtimeflag(self,z,tmp):
        self.Feedtimeflag[z]=tmp
    def getFeedtimeflag(self,z):
        return self.Feedtimeflag[z]
    def setbookfeedtime(self,z,tmp,tmp2):
        self.FeedHour[z]=tmp
        self.FeedMin[z]=tmp2
    def getbookfeedtime(self,z):
        return self.FeedHour[z]+self.FeedMin[z]
    def setLedonoff(self,z,tmp):
        self.Ledonoff[z]=tmp
    def getLedonoff(self,z):
        return self.Ledonoff[z]
    

def ana(meg,light):
    if(meg==b'LED\n'or meg==b'LED'):
        print("LED meg:",meg)
        LED_PIN=12
        GPIO.setup(LED_PIN,GPIO.OUT)
        if(light):
            GPIO.output(LED_PIN,GPIO.LOW)
            print("Led OFF")
            time.sleep(0.1)
            return False
        else:
            GPIO.output(LED_PIN, GPIO.HIGH)
            print("Led ON")
            time.sleep(0.1)
            return True
    elif(meg==b'FEED'or meg==b'FEED\n'):
        print("FEED meg:",meg)
        LED_PIN=11
        GPIO.setup(LED_PIN,GPIO.OUT)
        GPIO.output(LED_PIN,GPIO.LOW)
        time.sleep(1)
        GPIO.output(LED_PIN,GPIO.HIGH)
        time.sleep(0.1)
        return light
    else:
        print("RECIEVED:",meg)
        return light
    

class server(Thread):
    def __init__(self,test):
        Thread.__init__(self)
        self.test=test    
    def run(self):
        server_socket=socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        server_socket.bind(("",50008))
        server_socket.listen(5)
        while True:
            pw=True
            print("Waiting Client")
            while pw:
                client_socket,address=server_socket.accept()
                client_socket.send(bytes("Password...?","utf_8"))
                data=client_socket.recv(1024)
                print ("Connect....",address)
                f=open('a.txt','r',encoding='UTF-8')
                i=f.readline()
                print (i)
                print (data)
                if(str(data)==str(i)):    
                    print ("Connect Success...",address)
                    client_socket.send(bytes("Connect Success...","utf_8"))
                    time.sleep(1)
                    pw=False
                    justdoit=True
                    f.close()
                    data="3345678"
                    if(self.test.getLight()):
                        Fir="Led True:"+str(self.test.getFir())
                        client_socket.send(bytes(Fir,"utf_8"))
                    else:
                        Fir="Led False:"+str(self.test.getFir())
                        client_socket.send(bytes(Fir,"utf_8"))
                    break;
                else:
                    f.close()
                    print("Password Error...")
                    client_socket.send(bytes("Password Error...","utf_8"))
                    client_socket.close();
            while justdoit:
                data=client_socket.recv(1024)
                if(data=="881"):
                    print("Connect is end ,bye....")
                    print("8888")
                    client_socket.close()
                    justdoit=False
                elif(data==b'881'or data==b'881\n'):
                    print("Connect is end ,bye....")
                    print("88888888888888888")
                    client_socket.close()
                    justdoit=False
                elif(data==b''or data==b'\n'):
                    print("Connect is end ,bye....")
                    print("886")
                    client_socket.close()
                    justdoit=False
                elif("TimeLed"in str(data)):
                    chs=str(data)
                    h=chs.split(":")
                    print(h)
                    self.test.setbookledtime(int(h[1]),str(h[2]),str(h[3]))
                    if("false"in str(data)):
                        self.test.setLedtimeflag(int(h[1]),False)
                        print("Stop checktime",int(h[1]))
                    else:
                        self.test.setLedtimeflag(int(h[1]),True)
                        print("Run checktime",int(h[1]))
                    if("on"in str(data)):
                        self.test.setLedonoff(int(h[1]),True)
                        print("Led will on",int(h[1]))
                    else:
                        self.test.setLedonoff(int(h[1]),False)
                        print("Led will off",int(h[1]))
                elif("TimeFeed"in str(data)):
                    chs=str(data)
                    h=chs.split(":")
                    print(h)
                    self.test.setbookfeedtime(int(h[1]),str(h[2]),str(h[3]))
                    if("false"in str(data)):
                        self.test.setFeedtimeflag(int(h[1]),False)
                        print("Stop checktime",int(h[1]))
                    else:
                        self.test.setFeedtimeflag(int(h[1]),True)
                        print("Run checktime",int(h[1]))
                elif("Check Status"in str(data)):
                    chm="Refresh"+str(self.test.getLight())
                    client_socket.send(bytes(chm,"utf_8"))
                elif("Change Password"in str(data)):
                    chs=str(data)
                    h=chs.split(":")
                    f=open('a.txt','w',encoding='UTF-8')
                    f.write("b'"+h[1]+"\\n'")
                    f.close()
                else:
                    client_socket.send(bytes("????????????","utf_8"))
                    self.test.setLight(ana(data,self.test.getLight()))
class checktime(Thread):
    def __init__(self,test):
        Thread.__init__(self)
        self.test=test
    def run(self):
        while True:
            sametime=datetime.datetime.today()
            time.sleep(1)
            d=datetime.datetime.today()
            if(sametime.minute!=d.minute):
                for q in range(5):
                    if(self.test.getLedtimeflag(q)):
                        print("check Led time:",q)
                        tmp1=str(d.hour)+str(d.minute)
                        print(tmp1)
                        ch=str(self.test.getbookledtime(q))
                        print(ch)
                        if(tmp1==ch):
                            print("Time's up")
                            if(self.test.getLedonoff(q)):
                                LED_PIN=12
                                GPIO.setmode(GPIO.BOARD)
                                GPIO.setup(LED_PIN,GPIO.OUT)
                                GPIO.output(LED_PIN, GPIO.HIGH)
                                self.test.setLight(True)
                                print("Led ON")
                            else:
                                LED_PIN=12
                                GPIO.setmode(GPIO.BOARD)
                                GPIO.setup(LED_PIN,GPIO.OUT)
                                GPIO.output(LED_PIN, GPIO.LOW)
                                self.test.setLight(False)
                                print("Led OFF")
                            print ("Led Time's up~~~~~~")
                    if(self.test.getFeedtimeflag(q)):
                        print("check Feed time:",q)
                        tmp1=str(d.hour)+str(d.minute)
                        print(tmp1)
                        ch=str(self.test.getbookfeedtime(q))
                        print(ch)
                        if(tmp1==ch):
                            print("Time's up")
                            LED_PIN=11
                            GPIO.setup(LED_PIN,GPIO.OUT)
                            GPIO.output(LED_PIN,GPIO.LOW)
                            time.sleep(1)
                            GPIO.output(LED_PIN,GPIO.HIGH)
                            time.sleep(0.1)
                            print ("Feed Time's up~~~~~~")
               
    
print('server start')
LED_PIN=12
GPIO.setmode(GPIO.BOARD)
GPIO.setup(LED_PIN,GPIO.OUT)
GPIO.output(LED_PIN,GPIO.LOW)
GPIO.cleanup()
GPIO.setmode(GPIO.BOARD)
x=timesmeg()
server(x).start()
checktime(x).start() 


