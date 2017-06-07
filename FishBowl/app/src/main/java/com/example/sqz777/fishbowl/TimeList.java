package com.example.sqz777.fishbowl;

import android.app.Activity;
import android.app.TimePickerDialog;
import android.graphics.Color;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.TimePicker;

import java.util.Calendar;
import java.util.GregorianCalendar;

public class TimeList extends Activity {
    TimePickerDialog timepick;
    private int qq;
    TextView[] showtime=new TextView[5];
    ImageButton[] ib=new ImageButton[5];
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_time_list);
        final Button back_btn = LedTimeListInitial();
        for(int i=0;i<5;i++)
        {
            if(MainActivity.mClient.lflag[i])
            {
                showtime[i].setText(MainActivity.mClient.lh[i]+"點"+MainActivity.mClient.lm[i]+"分");
            }
            else
            {
                showtime[i].setText("未設定時間");
            }
            if(MainActivity.mClient.onoflag[i])
            {
                ib[i].setImageDrawable(getResources().getDrawable(R.drawable.touchchange1));
                ib[i].setTag("on");
            }
            else
            {
                ib[i].setImageDrawable(getResources().getDrawable(R.drawable.touchchange1_2));
                ib[i].setTag("off");
            }
        }
        GregorianCalendar calender=new GregorianCalendar();//setting time's menu
        timepick=new TimePickerDialog(this, new TimePickerDialog.OnTimeSetListener() {
            @Override
            public void onTimeSet(TimePicker view, int hourOfDay, int minute) {
                MainActivity.mClient.lh[qq]=String.valueOf(hourOfDay);
                MainActivity.mClient.lm[qq]=String.valueOf(minute);
                String message ="TimeLed:"+qq+":"+String.valueOf(hourOfDay)+":"+String.valueOf(minute)+":"+MainActivity.mClient.lflag[qq]+":"+ib[qq].getTag();//下午3點07分
                if (MainActivity.mClient != null) {
                    MainActivity.mClient.sendMessage(message);
                    showtime[qq].setText(MainActivity.mClient.lh[qq]+"點"+ MainActivity.mClient.lm[qq]+"分");
                }
            }
        }, calender.get(Calendar.HOUR_OF_DAY),calender.get(Calendar.MINUTE),false);

        back_btn.setOnTouchListener(new View.OnTouchListener() {
            public boolean onTouch(View v, MotionEvent event) {
                if (event.getAction() == MotionEvent.ACTION_UP) {
                    back_btn.setBackgroundColor(Color.parseColor("#4fcecc"));
                    TimeList.this.finish();
                    return true;
                }else if(event.getAction() == MotionEvent.ACTION_DOWN){
                    back_btn.setBackgroundColor(Color.parseColor("#2a6d6c"));
                }
                return false;
            }
        });
        for (int i = 0; i < 5; i++) {
            LedSetShowTimeButtonFunction(i);
            LedSetStatusButtonFunction(i);
        }
    }

    private Button LedTimeListInitial() {
        final Button back_btn = (Button)findViewById(R.id.back);
        showtime[0]=(TextView)findViewById(R.id.showtime1);
        showtime[1]=(TextView)findViewById(R.id.showtime2);
        showtime[2]=(TextView)findViewById(R.id.showtime3);
        showtime[3]=(TextView)findViewById(R.id.showtime4);
        showtime[4]=(TextView)findViewById(R.id.showtime5);
        ib[0]=(ImageButton)findViewById(R.id.ib1);
        ib[1]=(ImageButton)findViewById(R.id.ib2);
        ib[2]=(ImageButton)findViewById(R.id.ib3);
        ib[3]=(ImageButton)findViewById(R.id.ib4);
        ib[4]=(ImageButton)findViewById(R.id.ib5);
        return back_btn;
    }

    private void LedSetStatusButtonFunction(final int number) {
        ib[number].setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(ib[number].getTag()=="on")
                {
                    ib[number].setImageDrawable(getResources().getDrawable(R.drawable.touchchange1_2));
                    ib[number].setTag("off");
                    MainActivity.mClient.onoflag[number]=false;
                }
                else
                {
                    ib[number].setImageDrawable(getResources().getDrawable(R.drawable.touchchange1));
                    ib[number].setTag("on");
                    MainActivity.mClient.onoflag[4]=true;
                }
                String message ="TimeLed:"+number+":"+String.valueOf(MainActivity.mClient.lh[number])+":"+MainActivity.mClient.lm[number]+":"+MainActivity.mClient.lflag[number]+":"+ib[number].getTag();//下午3點07分
                if (MainActivity.mClient != null) {
                    MainActivity.mClient.sendMessage(message);
                }
            }
        });
    }
    private void LedSetShowTimeButtonFunction(final int number) {
        showtime[number].setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                qq=number;
                if(showtime[qq].getText()!="未設定時間")
                {
                    MainActivity.mClient.lflag[qq]=false;
                    String message ="TimeLed:"+qq+":"+"00"+":"+"00"+":"+MainActivity.mClient.lflag[qq]+":"+ib[qq].getTag();
                    if (MainActivity.mClient != null) {
                        MainActivity.mClient.sendMessage(message);
                        showtime[qq].setText("未設定時間");
                    }
                }else {
                    MainActivity.mClient.lflag[qq]=true;
                    timepick.show();
                }
            }
        });
    }

}
