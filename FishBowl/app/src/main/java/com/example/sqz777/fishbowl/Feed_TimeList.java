package com.example.sqz777.fishbowl;

import android.app.Activity;
import android.app.TimePickerDialog;
import android.graphics.Color;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.TimePicker;

import java.util.Calendar;
import java.util.GregorianCalendar;

public class Feed_TimeList extends Activity {
    TimePickerDialog timepick;
    private int qq;
    TextView[] showtime2=new TextView[5];
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_feed__time_list);
        final Button back_btn = (Button)findViewById(R.id.bbb);
        showtime2[0]=(TextView)findViewById(R.id.st1);
        showtime2[1]=(TextView)findViewById(R.id.st2);
        showtime2[2]=(TextView)findViewById(R.id.st3);
        showtime2[3]=(TextView)findViewById(R.id.st4);
        showtime2[4]=(TextView)findViewById(R.id.st5);
        for(int i=0;i<5;i++)
        {
            if(MainActivity.mClient.fflag[i])
            {
                showtime2[i].setText(MainActivity.mClient.fh[i]+"點"+MainActivity.mClient.fm[i]+"分");
            }
            else
            {
                showtime2[i].setText("未設定時間");
            }
        }
        GregorianCalendar calender=new GregorianCalendar();
        //back_btn.setBackgroundColor(Color.parseColor("#4fcecc"));
        timepick=new TimePickerDialog(this, new TimePickerDialog.OnTimeSetListener() {
            @Override
            public void onTimeSet(TimePicker view, int hourOfDay, int minute) {
                MainActivity.mClient.fh[qq]=String.valueOf(hourOfDay);
                MainActivity.mClient.fm[qq]=String.valueOf(minute);
                String message ="TimeFeed:"+qq+":"+String.valueOf(hourOfDay)+":"+String.valueOf(minute)+":"+MainActivity.mClient.fflag[qq];//下午3點07分
                if (MainActivity.mClient != null) {
                    MainActivity.mClient.sendMessage(message);
                }
                showtime2[qq].setText(MainActivity.mClient.fh[qq]+"點"+MainActivity.mClient.fm[qq]+"分");

            }
        }, calender.get(Calendar.HOUR_OF_DAY),calender.get(Calendar.MINUTE),false);

        back_btn.setOnTouchListener(new View.OnTouchListener() {
            public boolean onTouch(View v, MotionEvent event) {
                if (event.getAction() == MotionEvent.ACTION_UP) {
                    back_btn.setBackgroundColor(Color.parseColor("#4fcecc"));
                    Feed_TimeList.this.finish();
                    return true;
                }else if(event.getAction() == MotionEvent.ACTION_DOWN){
                    back_btn.setBackgroundColor(Color.parseColor("#2a6d6c"));
                }
                return false;
            }
        });
        for (int i = 0; i < 5; i++) {
            FeedSetShowTimeButtonFunction(i);
        }
    }

    private void FeedSetShowTimeButtonFunction(final int number) {
        showtime2[number].setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                qq=number;
                if(showtime2[qq].getText()!="未設定時間")
                {
                    MainActivity.mClient.fflag[qq]=false;
                    String message ="TimeFeed:"+qq+":"+"00"+":"+"00"+":"+MainActivity.mClient.fflag[qq];
                    if (MainActivity.mClient != null) {
                        MainActivity.mClient.sendMessage(message);
                        showtime2[qq].setText("未設定時間");
                    }
                }else {
                    MainActivity.mClient.fflag[qq]=true;
                    timepick.show();
                }
            }
        });
    }
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0) {
            Feed_TimeList.this.finish();
            return true;
        }
        return super.onKeyDown(keyCode, event);
    }
}
