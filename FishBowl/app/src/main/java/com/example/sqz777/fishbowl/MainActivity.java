package com.example.sqz777.fishbowl;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Typeface;
import android.util.Log;
import android.view.View;
import android.os.AsyncTask;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class MainActivity extends Activity
{
    public static Client mClient;
    EditText username,password;
    TextView alertext;
    TextView Smart,Aquarium;
    boolean conyon=false;
    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        final Button login = LoginInitial();

        TitleSetting();
        assert login != null;
        login.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                alertext.setText(""); //initial alar text
                conyon=true;
                new connectTask().execute("");
                Client.SERVERIP = username.getText().toString();
                mClient.Pas=false;
                //sends the message to the server
                try {
                    while (conyon)
                    {
                        if(mClient.Pas)
                        {
                            String message =password.getText().toString();
                            mClient.sendMessage(message);//傳送
                            Client.firstlogin =true;
                            conyon=false;
                        }
                        Thread.sleep(1);
                    }
                    while (Client.firstlogin) {
                        if(Client.PsStatus ==1)
                        {
                            Client.firstlogin =false;
                            Intent intent1 = new Intent(); //換頁
                            intent1.setClass(MainActivity.this , Main2.class);       //換頁
                            startActivity(intent1); //換頁
                            MainActivity.this.finish();
                            break;
                        }
                        else if(Client.PsStatus ==2){
                            alertext.setText("Password Error...");//密碼錯
                            password.setText("");
                            Client.PsStatus =3;
                            Client.firstlogin =false;
                            break;
                        }
                    }
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        });
    }

    private Button LoginInitial() {
        final Button login =(Button) findViewById(R.id.login );
        username = (EditText)findViewById(R.id.editTextUsername);
        password = (EditText)findViewById(R.id.editTextPassword);
        alertext = (TextView)findViewById(R.id.alert);
        return login;
    }

    private void TitleSetting() { //Title Settings
        Smart = (TextView)findViewById(R.id.textView2);
        Aquarium = (TextView)findViewById(R.id.textView3);
        Typeface custom_font = Typeface.createFromAsset(getAssets(), "fonts/comic.ttf");
        Smart.setTypeface(custom_font);
        Aquarium.setTypeface(custom_font);
    }
    public class connectTask extends AsyncTask<String,String,Client> { //connect Thread
        @Override
        protected Client doInBackground(String... message) {
            //we create a TCPClient object and
            mClient = new Client(new Client.OnMessageReceived() {
                @Override
                //here the messageReceived method is implemented
                public void messageReceived(String message) {
                    //this method calls the onProgressUpdate
                    publishProgress(message);
                }
            });
            mClient.run();
            return null;
        }
    }

}

