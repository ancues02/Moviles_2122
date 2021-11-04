package es.ucm.gdv.launcher.android;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;

import es.ucm.gdv.engine.android.AndroidEngine;

public class MainActivity extends AppCompatActivity {

    /**
     * Método llamado por Android como parte del ciclo de vida de
     * la actividad. Se llama en el momento de lanzarla.
     *
     * @param savedInstanceState Información de estado de la actividad
     *                           previamente serializada por ella misma
     *                           para reconstruirse en el mismo estado
     *                           tras un reinicio. Será null la primera
     *                           vez.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        /*
        AndroidEngine e = new AndroidEngine();
        setContentView(e.getSurfaceView());*/


    }
}


