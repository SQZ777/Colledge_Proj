package com.example.sqz777.fishbowl;

/**
 * Created by SQZ777 on 2016/6/22.
 */
import android.util.Log;
import java.io.*;
import java.net.InetAddress;
import java.net.Socket;
public class Client {
    public static String serverMessage=null;
    public static String SERVERIP; //your computer IP address
    public static final int SERVERPORT = 50008;
    private OnMessageReceived mMessageListener = null;
    public boolean mRun = false;
    PrintWriter out;
    BufferedReader in;
    public Socket socket;
    public static int LedStatus =3,PsStatus=3;
    public static boolean firstlogin=true,FirstLed=true,Pas=false;
    public static boolean[] lflag=new boolean[5];
    public static boolean[] fflag=new boolean[5];
    public static boolean[] onoflag=new boolean[5];
    public static String[] lh=new String[5];
    public static String[] lm=new String[5];
    public static String[] fh=new String[5];
    public static String[] fm=new String[5];
            /**
             *  Constructor of the class. OnMessagedReceived listens for the messages received from server
             */
            public Client(OnMessageReceived listener) {
                mMessageListener = listener;
    }
    /**
     * Sends the message entered by client to the server
     * @param message text entered by client
     */
    public void sendMessage(String message){
        if (out != null && !out.checkError()) {
            out.println(message);
            out.flush();
        }
    }
    public void stopClient(){
        mRun = false;
    }
    public void run() {
        mRun = true;
        try {
            //here you must put your computer's IP address.
            InetAddress serverAddr = InetAddress.getByName(SERVERIP);  //此處的SERVERIP由MainActivity Class的EditText決定(使用者決定)
            Log.e("TCP Client", "C: Connecting...");
            //create a socket to make the connection with the server
            socket = new Socket(serverAddr, SERVERPORT);
            try {
                //send the message to the server
                out = new PrintWriter(new BufferedWriter(new OutputStreamWriter(socket.getOutputStream())), true);
                //receive the message which the server sends back
                in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                //in this while the client listens for the messages sent by the server
                while (mRun/*socket.isConnected()*/) {
                    char []cha = new char[1024];
                    int len = in.read(cha);
                    serverMessage=new String(cha,0,len);
                    Log.e("",serverMessage);
                    if(serverMessage!=null) //Judge reception is empty or not
                    {
                        if(serverMessage.indexOf("Connect Success...")!=-1)
                        {
                            PsStatus=1;
                            serverMessage=null;
                        }
                        else if(serverMessage.indexOf("Password Error...")!=-1)
                        {
                            PsStatus=2;
                            serverMessage=null;
                        }
                        else if (serverMessage.indexOf("Led True")!=-1)
                        {
                            LedStatus=1;
                            String[] array=serverMessage.split(":");
                            int j=1;
                            for(int i=0;i<5;i++)    //TimeList update function
                            {
                                if(array[j].equals("True"))
                                    lflag[i]=true;
                                else  if(array[j].equals("False"))
                                    lflag[i]=false;
                                lh[i]=array[j+1];
                                lm[i]=array[j+2];
                                if(array[j+3].equals("True"))
                                    onoflag[i]=true;
                                else if(array[j+3].equals("False"))
                                    onoflag[i]=false;
                                if(array[j+4].equals("True"))
                                    fflag[i]=true;
                                else if(array[j+4].equals("False"))
                                    fflag[i]=false;
                                fh[i]=array[j+5];
                                fm[i]=array[j+6];
                                j+=7;
                            }
                            serverMessage=null;
                        }
                        else if(serverMessage.indexOf("Led False")!=-1)//TimeList update function
                        {
                            LedStatus=2;
                            String[] array=serverMessage.split(":");
                            int j=1;
                            for(int i=0;i<5;i++)
                            {
                                if(array[j].equals("True"))
                                    lflag[i]=true;
                                else  if(array[j].equals("False"))
                                    lflag[i]=false;
                                lh[i]=array[j+1];
                                lm[i]=array[j+2];
                                if(array[j+3].equals("True"))
                                    onoflag[i]=true;
                                else if(array[j+3].equals("False"))
                                    onoflag[i]=false;
                                if(array[j+4].equals("True"))
                                    fflag[i]=true;
                                else if(array[j+4].equals("False"))
                                    fflag[i]=false;
                                fh[i]=array[j+5];
                                fm[i]=array[j+6];
                                j+=7;
                            }
                            serverMessage=null;
                        }
                        else if(serverMessage.indexOf("Password...?")!=-1)
                        {
                            Pas=true;
                            serverMessage=null;
                        }
                        else if(serverMessage.indexOf("Refresh")!=-1)
                        {
                            if(serverMessage.indexOf("True")!=-1)
                                LedStatus=1;
                            else
                                LedStatus=2;
                            serverMessage=null;
                        }
                    }
                }
                socket.close();

            } catch (Exception e) {
            } finally {
                //the socket must be closed. It is not possible to reconnect to this socket
                // after it is closed, which means a new socket instance has to be created.
                socket.close();
            }
        } catch (Exception e) {
            Log.e("TCP", "C: Error", e);
        }
    }
    //Declare the interface. The method messageReceived(String message) will must be implemented in the MyActivity
    //class at on asynckTask doInBackground
    public interface OnMessageReceived {
        public void messageReceived(String message);
    }
}