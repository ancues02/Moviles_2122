package es.ucm.gdv.launcher.android;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        /*Logic logic = new Logic();
        logic.init(4);
        for(int i = 0; i < 4 ; ++i){
            for(int j = 0; j < 4 ; ++j){
                System.out.print(logic.board[i][j].solutionState + " ");
            }
            System.out.println();
        }

        System.out.println();
        for(int i = 0; i < 4 ; ++i){
            for(int j = 0; j < 4 ; ++j){
                System.out.print((logic.board[i][j].row + logic.board[i][j].column) + " " );
            }
            System.out.println();
        }
        System.out.println();*/

    }
}

