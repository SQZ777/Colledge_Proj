package com.example.sqz777.fishbowl;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Environment;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.os.AsyncTask;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.Toast;
import android.net.Uri;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.text.SimpleDateFormat;
import java.util.Date;

public class Main2 extends Activity {

    public Bitmap Saveit,Shareit;
    boolean Checkstatus=true;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main2);
       final ImageButton Led =(ImageButton) findViewById(R.id.LedBtn );
       final ImageButton Feed = (ImageButton)findViewById(R.id.FeedBtn);
       final ImageButton ScreenShot = (ImageButton)findViewById(R.id.ScreenShotBtn);
       final ImageButton BookLed =(ImageButton) findViewById(R.id.BookLed );
       final ImageButton BookFeed =(ImageButton) findViewById(R.id.BookFeed );
       final ImageButton ChangePW=(ImageButton)findViewById(R.id.ChangePW);
       final String  myURL ="http://"+String.valueOf(MainActivity.mClient.SERVERIP)+":8080/stream";
        WebViewSetting(myURL);
        CheckLedStatusFunction(Led);
        //below this line's Objects are Button's function
        assert Led != null;
        Led.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if(MainActivity.mClient.LedStatus==1){
                    Led.setImageDrawable(getResources().getDrawable(R.drawable.touchchange1_2));//關燈圖案
                    MainActivity.mClient.LedStatus=2;
                }else if(MainActivity.mClient.LedStatus==2){
                    Led.setImageDrawable(getResources().getDrawable(R.drawable.touchchange1));//開燈圖案
                    MainActivity.mClient.LedStatus=1;
                }
                String message ="LED";
                //sends the message to the server
                if (MainActivity.mClient != null) {
                    MainActivity.mClient.sendMessage(message);
                }
            }
        });
        assert Feed  != null;
        Feed.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Feed.setImageDrawable(getResources().getDrawable(R.drawable.touchchange2));
                String message ="FEED";
                //sends the message to the server
                if (MainActivity.mClient != null) {
                    MainActivity.mClient.sendMessage(message);
                }
            }
        });
        assert ChangePW !=null;
        ChangePW.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {    //Change Password's function
                View item = getLayoutInflater().inflate(R.layout.changepw, null); //create a panel to input new password
                final Button chanclebtn=(Button)item.findViewById(R.id.canbtn);
                final Button changebtn=(Button)item.findViewById(R.id.changebtn);
                final EditText np = (EditText) item.findViewById(R.id.NP);
                final EditText np2 = (EditText) item.findViewById(R.id.NP2);
                android.app.AlertDialog.Builder mbul=new android.app.AlertDialog.Builder(Main2.this);
                mbul.setView(item);
                final android.app.AlertDialog dia=mbul.create();
                dia.show();
                chanclebtn.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View view) {
                        dia.dismiss();
                    }
                });
                changebtn.setOnClickListener(new View.OnClickListener() {
                    @Override
                    public void onClick(View v) {
                        final String message ="Change Password:"+np.getText().toString()+":";
                        if(np.getText().toString().equals(np2.getText().toString()) && !(np.getText().toString().equals("")))
                        {
                            if (MainActivity.mClient != null) {
                                MainActivity.mClient.sendMessage(message);
                                Toast.makeText(getApplicationContext(), "更改成功！", Toast.LENGTH_LONG).show();
                            }
                        }
                        else
                        {
                            Toast.makeText(getApplicationContext(), "請確認兩次輸入的密碼一樣！", Toast.LENGTH_LONG).show();
                        }
                        dia.dismiss();
                    }
                });
            }
        });

        assert BookLed  != null; //Show Led time list's panel
        BookLed.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent1 = new Intent(); //換頁
                intent1.setClass( Main2.this,TimeList.class);//換頁
                startActivity(intent1); //換頁
            }
        });

        assert BookFeed  != null; //Show Feed time list's panel
        BookFeed.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent1 = new Intent(); //換頁
                intent1.setClass( Main2.this,Feed_TimeList.class);       //換頁
                startActivity(intent1); //換頁
            }
        });
//----------------------------------------------------
        final String finalMyURL = myURL; //Save Image and upload to another application
        ScreenShot.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                AsyncTask task = new takepic().executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR, finalMyURL +"/snapshot.jpeg?delay_s=0");
            }
        });
    }

    private void CheckLedStatusFunction(final ImageButton led) {
        new Thread(new Runnable() {
            public void run() {
                while (Checkstatus)
                {
                    try {
                        Thread.sleep(300);
                        String message ="Check Status";
                        //sends the message to the server
                        if (MainActivity.mClient != null) {
                            MainActivity.mClient.sendMessage(message);
                        }
                        runOnUiThread(new Runnable() {
                            public void run() {
                                if(MainActivity.mClient.LedStatus==1){
                                    led.setImageDrawable(getResources().getDrawable(R.drawable.touchchange1));
                                    MainActivity.mClient.FirstLed=false;
                                }else if(MainActivity.mClient.LedStatus==2){
                                    led.setImageDrawable(getResources().getDrawable(R.drawable.touchchange1_2));
                                    MainActivity.mClient.FirstLed=false;
                                }
                            }
                        });
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    };
                }
            }
        }).start();
    }
    private void WebViewSetting(String myURL) {
        WebView myBrowser=(WebView)findViewById(R.id.webView);
        WebSettings websettings = myBrowser.getSettings();
        websettings.setSupportZoom(true);
        websettings.setBuiltInZoomControls(true);
        websettings.setJavaScriptEnabled(true);
        myBrowser.setWebViewClient(new WebViewClient());
        myBrowser.loadUrl(myURL);
    }
    public void startShare(){
        Intent shareIntent = new Intent(Intent.ACTION_SEND);
        shareIntent.setType("image/jpeg");
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        Shareit.compress(Bitmap.CompressFormat.JPEG,100,byteArrayOutputStream);
        File file =new File (Environment.getExternalStorageDirectory()+File.separator+"ImageDemo.jpg");
        try {
            file.createNewFile();
            FileOutputStream fileOutputStream =new  FileOutputStream(file);
            fileOutputStream.write(byteArrayOutputStream.toByteArray());
        }catch (IOException e){
            e.printStackTrace();
        }
        shareIntent.putExtra(Intent.EXTRA_STREAM,Uri.parse("file:///sdcard/ImageDemo.jpg"));
        startActivity(Intent.createChooser(shareIntent,"Share Image"));
    }
    public void startSave(){
        FileOutputStream fileOutputStream = null;
        File file = getDisc();
        if(!file.exists()){
            file.mkdir();
            if(!file.exists())
            {
                Toast.makeText(this,"Can't Create it",Toast.LENGTH_SHORT).show();
                return;
            }

        }
        SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyymmsshhmmss");
        String date = simpleDateFormat.format(new Date());
        String name = "Img" +date +".jpg";
        String file_name = file.getAbsolutePath() +"/"+name;
        File new_file = new File(file_name);
        try{
            fileOutputStream = new FileOutputStream(new_file);
            //Bitmap bitmap = viewToBitmap(srn_img,srn_img.getWidth(),srn_img.getHeight());
            Saveit.compress(Bitmap.CompressFormat.JPEG,100,fileOutputStream);
            //bitmap.compress(Bitmap.CompressFormat.JPEG,100,fileOutputStream);
            fileOutputStream.flush();
            fileOutputStream.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
        reFreshGallery(new_file);
    }
    public void reFreshGallery(File file) {
        Intent intent = new Intent(Intent.ACTION_MEDIA_SCANNER_SCAN_FILE);
        intent.setData(Uri.fromFile(file));
        sendBroadcast(intent);
    }
    private File getDisc(){
        File file = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DCIM);

        return new File(file,"Image Demo");
    }
    public class takepic extends AsyncTask<String, Void, Bitmap> {
        @Override
        protected Bitmap doInBackground(String... params) {
            String url = params[0];
            return getBitmapFromURL(url);
        }
        @Override
        protected void onPostExecute(Bitmap result) {
            Saveit = result;
            Shareit = result;
            //srn_img.setImageBitmap(result);
            Log.e(" ","111111111111");
            startSave();
            Toast.makeText(Main2.this,"Image have saved",Toast.LENGTH_SHORT).show();

            startShare();
            super.onPostExecute(result);
        }
    }
    private static Bitmap getBitmapFromURL(String imageUrl)
    {
        try
        {
            URL url = new URL(imageUrl);
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setDoInput(true);
            connection.connect();
            InputStream input = connection.getInputStream();
            Bitmap bitmap = BitmapFactory.decodeStream(input);
            return bitmap;
        }
        catch (IOException e)
        {
            e.printStackTrace();
            return null;
        }
    }
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0) {
            String message ="881";
            //sends the message to the server
            if (MainActivity.mClient != null) {
                MainActivity.mClient.sendMessage(message);
                MainActivity.mClient.mRun=false;
                Checkstatus=false;
            }
            Main2.this.finish();
            return true;
        }
        return super.onKeyDown(keyCode, event);
    }
}